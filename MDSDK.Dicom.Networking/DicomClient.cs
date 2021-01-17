// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Networking.Messages;
using MDSDK.Dicom.Networking.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace MDSDK.Dicom.Networking.SCUs
{
    public abstract class DicomClient : IDisposable
    {
        private CancellationTokenSource _cancellationTokenSource;

        protected DicomClient()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void CancelAfter(TimeSpan timeSpan)
        {
            _cancellationTokenSource.CancelAfter(timeSpan);
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }
        
        public string AETitle { get; set; }

        protected abstract void WritePresentationContextRequests(IList<PresentationContextRequest> presentationContextRequests);

        protected abstract void ReadPresentationContextResponses(IReadOnlyList<PresentationContextResponse> presentationContextResponses);

        internal DicomConnection _connection;

        public StreamWriter TraceWriter { get; set; }

        public void ConnectTo(DicomNetworkAddress ae)
        {
            var connection = DicomConnection.Connect(ae.HostNameOrIPAddress, ae.Port, _cancellationTokenSource.Token);
            try
            {
                connection.TraceWriter = TraceWriter;

                var associationRequest = new AssociationRequest
                {
                    CalledAETitle = ae.AETitle,
                    CallingAETitle = AETitle,
                };
                WritePresentationContextRequests(associationRequest.PresentationContextRequests);
                connection.SendAssociationRequest(associationRequest);
                
                var associationResponse = connection.ReceiveAssociationResponse();
                ReadPresentationContextResponses(associationResponse.PresentationContextResponses);

                _connection = connection;
            }
            catch (Exception)
            {
                connection.Dispose();
                throw;
            }
        }

        public void Send<T>(int presentationContextID, T command) where T : Command, new()
        {
            if (TraceWriter != null)
            {
                NetUtils.TraceOutput(TraceWriter, $"PC {presentationContextID} sending ", command);
            }
            _connection.SendCommand(1, stream => Command.WriteTo(stream, command));
        }

        public T Receive<T>(int expectedPresentationContextID) where T : Command, new()
        {
            T command = null;

            _connection.ReceiveCommand((byte presentationContextID, Stream stream) =>
            {
                if (presentationContextID != expectedPresentationContextID)
                {
                    throw new IOException($"Unexpected presentation context ID {presentationContextID}");
                }
                command = Command.ReadFrom<T>(stream);
                if (TraceWriter != null)
                {
                    NetUtils.TraceOutput(TraceWriter, $"PC {presentationContextID} received ", command);
                }
            });

            return command;
        }

        public void DisconnectGracefully()
        {
            _connection.SendReleaseRequest();
            _connection.ReceiveReleaseResponse();
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }

            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }
    }
}
