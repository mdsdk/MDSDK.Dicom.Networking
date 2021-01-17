// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;
using MDSDK.Dicom.Networking.DataUnits;
using MDSDK.Dicom.Networking.DataUnits.PDUs;
using MDSDK.Dicom.Networking.Net;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace MDSDK.Dicom.Networking
{
    public sealed class DicomConnection : IDisposable
    {
        internal Socket Socket { get; }

        internal SocketStream SocketStream { get; }

        internal BinaryStreamReader Input { get; }

        internal BinaryStreamWriter Output { get; }

        private DicomConnection(Socket socket, CancellationToken cancellationToken)
        {
            Socket = socket;
            SocketStream = new SocketStream(socket, cancellationToken);
            Input = new BinaryStreamReader(SocketStream, ByteOrder.BigEndian);
            Output = new BinaryStreamWriter(SocketStream, ByteOrder.BigEndian);
        }

        public void Dispose()
        {
            SocketStream.Dispose();
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
            pdu.WriteTo(Output);
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
                        Input.Read(pdu.Length, () => pdu.ReadContentFrom(Input));
                    }
                    TraceReceived(pdu);
                    return pdu;
                }
            }

            if (pdu is AbortPDU abortPDU)
            {
                Input.Read(pdu.Length, () => abortPDU.ReadContentFrom(Input));
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

        public void SendCommand(byte presentationContextID, Action<Stream> writeCommandAction)
        {
            using (var stream = new PresentationContextOutputStream(this, presentationContextID, FragmentType.Command))
            {
                writeCommandAction.Invoke(stream);
                stream.Flush();
            }
        }

        public bool ReceiveCommand(Action<byte, Stream> readCommandAction)
        {
            if (ReadNextPDU(DataUnitType.DataTransferPDU, DataUnitType.ReleaseRequestPDU) is DataTransferPDUHeader)
            {
                using (var stream = new PresentationContextInputStream(this, FragmentType.Command))
                {
                    readCommandAction.Invoke(stream.PresentationContextID, stream);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public Stream OpenDatasetInputStream(out byte presentationContextID)
        {
            var datasetStream = new PresentationContextInputStream(this, FragmentType.Dataset);
            presentationContextID = datasetStream.PresentationContextID;
            return datasetStream;
        }

        public uint? MaxDataTransferPDULengthRequestedByPeer { get; private set; }

        public void SendReleaseRequest() => SendPDU(new ReleaseRequestPDU());

        public void ReceiveReleaseResponse() => ReadNextPDU(DataUnitType.ReleaseResponsePDU);
    }
}