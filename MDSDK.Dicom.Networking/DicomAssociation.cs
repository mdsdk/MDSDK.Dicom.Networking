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
    /// <summary>An association between two DICOM application entities</summary>
    public sealed class DicomAssociation : IDisposable
    {
        private DicomConnection _connection;

        internal Dictionary<byte, DicomPresentationContext> PresentationContexts { get; } = new();

        internal DicomAssociation(DicomConnection connection, Dictionary<byte, DicomUID> presentationContextTransferSyntaxUID)
        {
            _connection = connection;

            foreach (var (presentationContextID, transferSyntaxUID) in presentationContextTransferSyntaxUID)
            {
                PresentationContexts[presentationContextID] = new DicomPresentationContext(presentationContextID, transferSyntaxUID);
            }
        }

        private ushort _requestCounter = 0;

        private void SendRequestCommand(byte presentationContextID, IRequest request, Action<Stream> dataSetWriter)
        {
            request.MessageID = _requestCounter++;
            _connection.SendCommand(presentationContextID, request, dataSetWriter);
        }

        private void SendResponseCommand(byte presentationContextID, IResponse response, ushort requestMessageID,
            Action<Stream> dataSetWriter)
        {
            response.MessageIDBeingRespondedTo = requestMessageID;
            _connection.SendCommand(presentationContextID, response, dataSetWriter);
        }

        private void WriteDataSet<TDataSet>(byte presentationContextID, TDataSet dataSet, Stream stream)
        {
            if (_connection.TraceWriter != null)
            {
                NetUtils.TraceOutput(_connection.TraceWriter, $"PC {presentationContextID} sending ", dataSet);
            }

            var presentationContext = PresentationContexts[presentationContextID];
            var output = new BufferedStreamWriter(stream);
            DicomSerializer.Serialize<TDataSet>(output, presentationContext.TransferSyntaxUID, dataSet);
        }

        private TResponse ReceiveResponseCommand<TResponse>(byte presentationContextID, ushort requestMessageID)
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

        private TDataSet ReadDataSet<TDataSet>(byte presentationContextID, Stream stream) where TDataSet : new()
        {
            var presentationContext = PresentationContexts[presentationContextID];
            var input = new BufferedStreamReader(stream);
            var dataSet = DicomSerializer.Deserialize<TDataSet>(input, presentationContext.TransferSyntaxUID);

            if (_connection.TraceWriter != null)
            {
                NetUtils.TraceOutput(_connection.TraceWriter, $"PC {presentationContextID} received ", dataSet);
            }

            return dataSet;
        }

        /// <summary>Sends a request command using the given presentation context</summary>
        public void SendRequest<TRequest>(byte presentationContextID, TRequest request) where TRequest : IRequest, IHasNoDataSet, new()
        {
            SendRequestCommand(presentationContextID, request, null);
        }

        /// <summary>Sends a request command and its associated data set using the given presentation context</summary>
        public void SendRequest<TRequest>(byte presentationContextID, TRequest request, Action<Stream> dataSetWriter)
            where TRequest : IRequest, IMayHaveDataSet, new()
        {
            SendRequestCommand(presentationContextID, request, dataSetWriter);
        }

        /// <summary>Sends a request command and its associated data set using the given presentation context</summary>
        public void SendRequest<TRequest, TDataSet>(byte presentationContextID, TRequest request, TDataSet dataSet)
            where TRequest : IRequest, IMayHaveDataSet, new()
        {
            Action<Stream> dataSetWriter = (dataSet == null) ? null : stream => WriteDataSet(presentationContextID, dataSet, stream);
            SendRequest(presentationContextID, request, dataSetWriter);
        }

        /// <summary>Sends a response command using the given presentation context</summary>
        public void SendResponse<TResponse>(byte presentationContextID, TResponse response, ushort requestMessageID)
            where TResponse : IResponse, IHasNoDataSet, new()
        {
            SendResponseCommand(presentationContextID, response, requestMessageID, null);
        }

        /// <summary>Sends a response command and its associated data set using the given presentation context</summary>
        public void SendResponse<TResponse>(byte presentationContextID, TResponse response, ushort requestMessageID, Action<Stream> dataSetWriter)
            where TResponse : IResponse, IMayHaveDataSet
        {
            SendResponseCommand(presentationContextID, response, requestMessageID, dataSetWriter);
        }

        /// <summary>Sends a response command and its associated data set using the given presentation context</summary>
        public void SendResponse<TResponse, TDataSet>(byte presentationContextID, TResponse response, ushort requestMessageID, TDataSet dataSet)
            where TResponse : IResponse, IMayHaveDataSet
        {
            Action<Stream> dataSetWriter = (dataSet == null) ? null : stream => WriteDataSet(presentationContextID, dataSet, stream);
            SendResponse(presentationContextID, response, requestMessageID, dataSetWriter);
        }

        /// <summary>Receives a response expected for a request sent using the given presentation context</summary>
        public TResponse ReceiveResponse<TResponse>(byte presentationContextID, ushort requestMessageID)
            where TResponse : IResponse, IHasNoDataSet, new()
        {
            return ReceiveResponseCommand<TResponse>(presentationContextID, requestMessageID);
        }
        
        /// <summary>Receives the data set that follows the given command</summary>
        public void ReceiveDataSet(ICommand command, byte presentationContextID, Action<Stream> dataSetReader) 
        {
            _connection.ReceiveDataSet(command, presentationContextID, dataSetReader);
        }

        /// <summary>Receives the data set that follows the given command</summary>
        public TDataSet ReceiveDataSet<TDataSet>(ICommand command, byte presentationContextID) where TDataSet : new()
        {
            TDataSet dataSet = default;

            ReceiveDataSet(command, presentationContextID, stream =>
            {
                var presentationContext = PresentationContexts[presentationContextID];
                var input = new BufferedStreamReader(stream);
                dataSet = DicomSerializer.Deserialize<TDataSet>(input, presentationContext.TransferSyntaxUID);
            });

            return dataSet;
        }

        /// <summary>Handler for commands received using ReceiveCommands</summary>
        public delegate void CommandHandler(DicomAssociation association, byte presentationContextID,
            DicomUID transferSyntaxUID, ICommand command, out bool stop);

        /// <summary>Receives commands and dispatches them to the given command handler until the command handler says stop</summary>
        public void ReceiveCommands(CommandHandler commandHandler)
        {
            var stop = false;
            while (!stop && _connection.TryReceiveCommand(out byte presentationContextID, out ICommand command))
            {
                if (!PresentationContexts.TryGetValue(presentationContextID, out DicomPresentationContext presentationContext))
                {
                    throw new IOException($"Unknown presentation context ID {presentationContextID} received");
                }
                commandHandler.Invoke(this, presentationContextID, presentationContext.TransferSyntaxUID, command, out stop);
            }
        }

        /// <summary>Releases the association</summary>
        public void Release()
        {
            _connection.SendReleaseRequest();
            _connection.ReceiveReleaseResponse();
        }

        /// <summary>Terminates the underlying TCP/IP connection</summary>
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
