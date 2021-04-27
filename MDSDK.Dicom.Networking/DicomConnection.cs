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
    public sealed class DicomConnection : IDisposable
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

        public void SendCommand(byte presentationContextID, ICommand command, CommandIsFollowedByDataSet commandIsFollowedByDataSet) 
        {
            var commandAttribute = command.GetType().GetCustomAttribute<CommandAttribute>();
            
            command.CommandField = commandAttribute.CommandType;
            command.CommandDataSetType = (commandIsFollowedByDataSet == CommandIsFollowedByDataSet.Yes) ? 0xFEFE : 0x0101;

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
        }

        public void SendDataSet(byte presentationContextID, Action<Stream> writeDataSetAction)
        {
            using (var stream = new PresentationContextOutputStream(this, presentationContextID, FragmentType.DataSet))
            {
                writeDataSetAction.Invoke(stream);
                stream.Flush();
            }
        }

        public bool TryReceiveCommand(out byte presentationContextID, out ICommand command)
        {
            if (ReadNextPDU(DataUnitType.DataTransferPDU, DataUnitType.ReleaseRequestPDU) is DataTransferPDUHeader)
            {
                using (var stream = new PresentationContextInputStream(this, FragmentType.Command))
                {
                    presentationContextID = stream.PresentationContextID;
                    var input = new BufferedStreamReader(stream);
                    command = CommandSerialization.ReadFrom(input);
                    stream.SkipToEnd();
                    if (TraceWriter != null)
                    {
                        NetUtils.TraceOutput(TraceWriter, $"PC {presentationContextID} received ", command);
                    }
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

        public ICommand ReceiveCommand(byte presentationContextID)
        {
            ReadNextPDU(DataUnitType.DataTransferPDU);

            using (var stream = new PresentationContextInputStream(this, FragmentType.Command))
            {
                if (stream.PresentationContextID != presentationContextID)
                {
                    throw new IOException($"Expected PCID {presentationContextID} but got {stream.PresentationContextID}");
                }
                var input = new BufferedStreamReader(stream);
                var command = CommandSerialization.ReadFrom(input);
                stream.SkipToEnd();
                if (TraceWriter != null)
                {
                    NetUtils.TraceOutput(TraceWriter, $"PC {presentationContextID} received ", command);
                }
                return command;
            }
        }

        public void ReceiveDataSet(byte presentationContextID, Action<Stream> readDataSetAction)
        {
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
                readDataSetAction.Invoke(stream);
                stream.SkipToEnd();
            }
        }

        public uint? MaxDataTransferPDULengthRequestedByPeer { get; private set; }

        public void SendReleaseRequest() => SendPDU(new ReleaseRequestPDU());

        public void ReceiveReleaseResponse() => ReadNextPDU(DataUnitType.ReleaseResponsePDU);
    }
}