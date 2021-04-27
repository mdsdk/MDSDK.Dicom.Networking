// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;
using MDSDK.Dicom.Networking.Messages;
using MDSDK.Dicom.Networking.Net;
using MDSDK.Dicom.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace MDSDK.Dicom.Networking
{
    public sealed class DicomAssociation : IDisposable
    {
        private DicomConnection _connection;

        internal Dictionary<byte, DicomPresentationContext> PresentationContexts { get; } = new();

        public DicomAssociation(DicomConnection connection, Dictionary<byte, DicomUID> presentationContextTransferSyntaxUID)
        {
            _connection = connection;

            foreach (var (presentationContextID, transferSyntaxUID) in presentationContextTransferSyntaxUID)
            {
                PresentationContexts[presentationContextID] = new DicomPresentationContext(presentationContextID, transferSyntaxUID);
            }
        }

        private ushort _requestCounter = 0;

        public void SendRequest<TRequest>(byte presentationContextID, TRequest request, CommandIsFollowedByDataSet isFollowedByDataSet) 
            where TRequest : IRequest, new()
        {
            request.MessageID = _requestCounter++;
            _connection.SendCommand(presentationContextID, request, isFollowedByDataSet);
        }

        public void SendResponse<TResponse>(byte presentationContextID, TResponse response, ushort requestMessageID,
            CommandIsFollowedByDataSet isFollowedByDataSet) where TResponse : IResponse, new()
        {
            response.MessageIDBeingRespondedTo = requestMessageID;
            _connection.SendCommand(presentationContextID, response, isFollowedByDataSet);
        }

        public void SendDataSet<TDataSet>(byte presentationContextID, TDataSet dataSet)
        {
            _connection.SendDataSet(presentationContextID, stream =>
            {
                var presentationContext = PresentationContexts[presentationContextID];
                var output = new BufferedStreamWriter(stream);
                DicomSerializer.Serialize<TDataSet>(output, presentationContext.TransferSyntaxUID, dataSet);
            });
        }

        public TResponse ReceiveResponse<TResponse>(byte presentationContextID, ushort requestMessageID)
            where TResponse : IResponse, new()
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
                throw new IOException($"Expected {CommandSerialization.GetCommandType<TResponse>()} but got {command.CommandField}");
            }
        }

        public void ReceiveDataSet(byte presentationContextID, Action<Stream> readAction)
        {
            _connection.ReceiveDataSet(presentationContextID, readAction);
        }

        public TDataSet ReceiveDataSet<TDataSet>(byte presentationContextID) where TDataSet : new()
        {
            TDataSet dataSet = default;

            ReceiveDataSet(presentationContextID, stream =>
            {
                var presentationContext = PresentationContexts[presentationContextID];
                var input = new BufferedStreamReader(stream);
                dataSet = DicomSerializer.Deserialize<TDataSet>(input, presentationContext.TransferSyntaxUID);
            });

            if (_connection.TraceWriter != null)
            {
                NetUtils.TraceOutput(_connection.TraceWriter, $"PC {presentationContextID} received ", dataSet);
            }

            return dataSet;
        }

        public delegate void MessageHandler(DicomAssociation association, DicomPresentationContext presentationContext, 
            ICommand command, out bool stop);

        public void DispatchIncomingMessages(MessageHandler messageHandler)
        {
            var stop = false;
            while (!stop && _connection.TryReceiveCommand(out byte presentationContextID, out ICommand command))
            {
                if (!PresentationContexts.TryGetValue(presentationContextID, out DicomPresentationContext presentationContext))
                {
                    throw new IOException($"Unknown presentation context ID {presentationContextID} received");
                }
                messageHandler.Invoke(this, presentationContext, command, out stop);
            }
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
