// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;
using MDSDK.Dicom.Networking.DataUnits;
using MDSDK.Dicom.Networking.DataUnits.PDUs;
using MDSDK.Dicom.Networking.Messages;
using MDSDK.Dicom.Networking.Net;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

namespace MDSDK.Dicom.Networking
{
    internal sealed class DicomConnection : IDisposable
    {
        internal Socket Socket { get; }

        internal SocketInputStream SocketInputStream { get; }

        internal SocketOutputStream SocketOutputStream { get; }

        internal BufferedStreamReader Input { get; }

        internal BufferedStreamWriter Output { get; }

        private DicomConnection(Socket socket, CancellationToken cancellationToken)
        {
            Socket = socket;
            SocketInputStream = new SocketInputStream(socket, cancellationToken);
            SocketOutputStream = new SocketOutputStream(socket, cancellationToken);
            Input = new BufferedStreamReader(SocketInputStream);
            Output = new BufferedStreamWriter(SocketOutputStream);
        }

        public void Dispose()
        {
            SocketOutputStream.Dispose();
            SocketInputStream.Dispose();
            Socket.Dispose();
        }

        public static DicomConnection Connect(string hostNameOrIPAddress, int port, CancellationToken cancellationToken)
        {
            var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(hostNameOrIPAddress, port, cancellationToken);
            }
            catch (Exception)
            {
                socket.Dispose();
                throw;
            }

            return new DicomConnection(socket, cancellationToken);
        }

        public static DicomConnection Accept(Socket listenSocket, CancellationToken cancellationToken)
        {
            return new DicomConnection(listenSocket.Accept(), cancellationToken);
        }

        public StreamWriter TraceWriter { get; set; }

        private void TraceSending(PDU pdu)
        {
            if (TraceWriter != null)
            {
                NetUtils.TraceOutput(TraceWriter, "Sending ", pdu);
            }
        }

        private void TraceReceived(PDU pdu)
        {
            if (TraceWriter != null)
            {
                NetUtils.TraceOutput(TraceWriter, "Received ", pdu);
            }
        }

        private void SendPDU(PDU pdu)
        {
            TraceSending(pdu);
            var dataWriter = new BinaryDataWriter(Output, ByteOrder.BigEndian);
            pdu.WriteTo(dataWriter);
            Output.Flush(FlushMode.Deep);
        }

        internal long EndOfDataTransferPDUPosition { get; private set; }

        private PDU ReadNextPDU(params DataUnitType[] expectedPduTypes)
        {
            var pdu = PDU.ReadHeaderFrom(Input);

            foreach (var expectedPDUType in expectedPduTypes)
            {
                Debug.Assert(expectedPDUType != DataUnitType.AbortPDU);

                if (pdu.DataUnitType == expectedPDUType)
                {
                    if (pdu.DataUnitType == DataUnitType.DataTransferPDU)
                    {
                        EndOfDataTransferPDUPosition = Input.Position + pdu.Length;
                    }
                    else
                    {
                        Input.Read(pdu.Length, () =>
                        {
                            var dataReader = new BinaryDataReader(Input, ByteOrder.BigEndian);
                            pdu.ReadContentFrom(dataReader);
                        });
                    }
                    TraceReceived(pdu);
                    return pdu;
                }
            }

            if (pdu is AbortPDU abortPDU)
            {
                Input.Read(pdu.Length, () =>
                {
                    var dataReader = new BinaryDataReader(Input, ByteOrder.BigEndian);
                    abortPDU.ReadContentFrom(dataReader);
                });
                TraceReceived(pdu);
                throw new AbortException(abortPDU);
            }

            TraceReceived(pdu);

            throw new IOException($"Expected {(string.Join(" or ", expectedPduTypes))} but got {pdu.DataUnitType}");
        }

        internal void ReadNextDataTransferPDU()
        {
            if (Input.Position != EndOfDataTransferPDUPosition)
            {
                throw new Exception("Logic error");
            }
            ReadNextPDU(DataUnitType.DataTransferPDU);
        }

        public void SendAssociationRequest(AssociationRequest associationRequest) => SendPDU(associationRequest.ToPDU());

        public AssociationResponse ReceiveAssociationResponse()
        {
            var pdu = ReadNextPDU(DataUnitType.AssociateAcceptPDU, DataUnitType.AssociateRejectPDU);
            if (pdu is AssociateAcceptPDU associateAcceptPDU)
            {
                var associationResponse = AssociationResponse.FromPDU(associateAcceptPDU);
                MaxDataTransferPDULengthRequestedByPeer = associationResponse.MaxDataTransferPDULength;
                return associationResponse;
            }
            else if (pdu is AssociateRejectPDU associateRejectPDU)
            {
                throw new AssociationRejectedException(associateRejectPDU);
            }
            else
            {
                throw new Exception("Logic error");
            }
        }

        private AssociateRequestPDU _associateRequestPDU;

        public AssociationRequest ReceiveAssociationRequest()
        {
            _associateRequestPDU = (AssociateRequestPDU)ReadNextPDU(DataUnitType.AssociateRequestPDU);
            var associationRequest = AssociationRequest.FromPDU(_associateRequestPDU);
            MaxDataTransferPDULengthRequestedByPeer = associationRequest.MaxDataTransferPDULength;
            return associationRequest;
        }

        public void SendAssociationResponse(AssociationResponse associationResponse)
        {
            var associateAcceptPDU = associationResponse.ToPDU(_associateRequestPDU);
            SendPDU(associateAcceptPDU);
        }

        public void SendAssociationResponse(AssociationRejectedException associationException)
        {
            SendPDU(associationException.ToPDU());
            Socket.Shutdown(SocketShutdown.Send);
        }

        public void SendCommand(byte presentationContextID, ICommand command, Action<Stream> dataSetWriter) 
        {
            if (command is IMayHaveDataSet mayHaveDataSet)
            {
                if ((dataSetWriter == null) && mayHaveDataSet.IsDataSetRequired())
                {
                    throw new ArgumentException($"{command} must be followed by a data set");
                }
            }
            else if (dataSetWriter != null)
            {
                throw new ArgumentException($"{command} must not be followed by a data set");
            }

            var commandAttribute = command.GetType().GetCustomAttribute<CommandAttribute>();
            
            command.CommandField = commandAttribute.CommandType;
            command.CommandDataSetType = (dataSetWriter == null) ? 0x0101 : 0xFEFE;

            if (TraceWriter != null)
            {
                NetUtils.TraceOutput(TraceWriter, $"PC {presentationContextID} sending ", command);
            }

            using (var stream = new PresentationContextOutputStream(this, presentationContextID, FragmentType.Command))
            {
                var output = new BufferedStreamWriter(stream);
                CommandSerialization.WriteTo(output, command);
                output.Flush(FlushMode.Deep);
            }

            if (dataSetWriter != null)
            {
                using (var stream = new PresentationContextOutputStream(this, presentationContextID, FragmentType.DataSet))
                {
                    dataSetWriter.Invoke(stream);
                    stream.Flush();
                }
            }
        }

        private ICommand _pendingCommand;

        private byte _pendingPresentationContext;

        private void EnsureNoPendingDataSet()
        {
            if (_pendingCommand != null)
            {
                throw new InvalidOperationException($"Cannot receive new command before data set of {_pendingCommand} has been received");
            }
        }

        private ICommand ReadCommandFrom(PresentationContextInputStream stream)
        {
            var input = new BufferedStreamReader(stream);
            
            var command = CommandSerialization.ReadFrom(input);
            
            if (command is IMayHaveDataSet mayHaveDataSet)
            {
                if (command.IsFollowedByDataSet())
                {
                    _pendingCommand = command;
                    _pendingPresentationContext = stream.PresentationContextID;
                } 
                else if (mayHaveDataSet.IsDataSetRequired())
                {
                    throw new IOException($"{command} must be followed by a data set");
                }
            }
            else if (command.IsFollowedByDataSet())
            {
                throw new IOException($"Unexpected data set received for command {command}");
            }
            
            stream.SkipToEnd();
            
            if (TraceWriter != null)
            {
                NetUtils.TraceOutput(TraceWriter, $"PC {stream.PresentationContextID} received ", command);
            }

            return command;
        }

        public ICommand ReceiveCommand(byte presentationContextID)
        {
            EnsureNoPendingDataSet();

            ReadNextPDU(DataUnitType.DataTransferPDU);

            using (var stream = new PresentationContextInputStream(this, FragmentType.Command))
            {
                if (stream.PresentationContextID != presentationContextID)
                {
                    throw new IOException($"Expected PCID {presentationContextID} but got {stream.PresentationContextID}");
                }
                return ReadCommandFrom(stream);
            }
        }

        public bool TryReceiveCommand(out byte presentationContextID, out ICommand command)
        {
            EnsureNoPendingDataSet();

            if (ReadNextPDU(DataUnitType.DataTransferPDU, DataUnitType.ReleaseRequestPDU) is DataTransferPDUHeader)
            {
                using (var stream = new PresentationContextInputStream(this, FragmentType.Command))
                {
                    presentationContextID = stream.PresentationContextID;
                    command = ReadCommandFrom(stream);
                }
                return true;
            }
            else
            {
                presentationContextID = 0;
                command = null;
                return false;
            }
        }

        public void ReceiveDataSet(ICommand command, byte presentationContextID, Action<Stream> dataSetReader)
        {
            if ((_pendingCommand == null) || (_pendingPresentationContext == 0))
            {
                throw new InvalidOperationException("No pending data set");
            }

            if (command != _pendingCommand)
            {
                throw new ArgumentException($"{command} does not match pending command {_pendingCommand}");
            }

            if (presentationContextID != _pendingPresentationContext)
            {
                throw new ArgumentException($"{presentationContextID} does not match pending presentation context {_pendingPresentationContext}");
            }

            if (Input.Position == EndOfDataTransferPDUPosition)
            {
                ReadNextDataTransferPDU();
            }

            using (var stream = new PresentationContextInputStream(this, FragmentType.DataSet))
            {
                if (stream.PresentationContextID != presentationContextID)
                {
                    throw new IOException($"Expected PCID {presentationContextID} but got {stream.PresentationContextID}");
                }
                dataSetReader.Invoke(stream);
                stream.SkipToEnd();
            }

            _pendingCommand = null;
            _pendingPresentationContext = 0;
        }

        public uint? MaxDataTransferPDULengthRequestedByPeer { get; private set; }

        public void SendReleaseRequest() => SendPDU(new ReleaseRequestPDU());

        public void ReceiveReleaseResponse() => ReadNextPDU(DataUnitType.ReleaseResponsePDU);
    }
}