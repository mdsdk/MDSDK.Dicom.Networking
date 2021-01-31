// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Networking.Messages;
using MDSDK.Dicom.Networking.Net;
using MDSDK.Dicom.Serialization;
using MDSDK.Dicom.Serialization.TransferSyntaxes;
using System;
using System.Collections.Generic;
using System.IO;

namespace MDSDK.Dicom.Networking.SCUs
{
    public sealed class DicomAssociation : IDisposable
    {
        private DicomConnection _connection;

        internal Dictionary<byte, DicomPresentationContext> PresentationContexts { get; } = new();

        public DicomAssociation(DicomConnection connection, Dictionary<byte, TransferSyntax> presentationContextTransferSyntaxes)
        {
            _connection = connection;

            foreach (var (presentationContextID, transferSyntax) in presentationContextTransferSyntaxes)
            {
                PresentationContexts[presentationContextID] = new DicomPresentationContext(presentationContextID, transferSyntax);
            }
        }

        private ushort _requestCounter = 0;

        public void SendRequest<TRequest>(byte presentationContextID, TRequest request) where TRequest : Request, new()
        {
            request.MessageID = _requestCounter++;
            _connection.SendCommand(presentationContextID, request);
        }

        public void SendDataset<TDataset>(byte presentationContextID, TDataset dataset)
        {
            _connection.SendDataset(presentationContextID, stream =>
            {
                var presentationContext = PresentationContexts[presentationContextID];
                DicomSerializer.Serialize<TDataset>(stream, presentationContext.TransferSyntax, dataset);
            });
        }

        public TResponse ReceiveResponse<TResponse>(byte presentationContextID, ushort requestMessageID)
            where TResponse : Response, new()
        {
            var command = _connection.ReceiveCommand(presentationContextID);
            if (command is TResponse response)
            {
                if (response.MessageIDBeingRespondedTo != requestMessageID)
                {
                    throw new IOException($"Expected MessageIDBeingRespondedTo {requestMessageID} but got {response.MessageIDBeingRespondedTo}");
                }
                return response;
            }
            else
            {
                throw new IOException($"Expected {Command.GetCommandType<TResponse>()} but got {command.CommandField}");
            }
        }

        public TDataset ReceiveDataset<TDataset>(byte presentationContextID) where TDataset : new()
        {
            TDataset dataset = default;

            _connection.ReceiveDataset(presentationContextID, stream =>
            {
                var presentationContext = PresentationContexts[presentationContextID];
                dataset = DicomSerializer.Deserialize<TDataset>(stream, presentationContext.TransferSyntax);
            });

            return dataset;
        }

        public void Release()
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
        }
    }
}
