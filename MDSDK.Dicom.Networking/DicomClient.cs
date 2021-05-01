// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Networking.Net;
using MDSDK.Dicom.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace MDSDK.Dicom.Networking
{
    /// <summary>Enables a local application entity to connect to a remote application entity</summary>
    public class DicomClient
    {
        /// <summary>The AE title of the local application entity</summary>
        public string AETitle { get; }

        /// <summary>Constructor</summary>
        public DicomClient(string aeTitle)
        {
            AETitle = aeTitle;
        }

        private List<PresentationContextRequest> _presentationContextRequests = new List<PresentationContextRequest>();

        /// <summary>Registers a proposed presentation context and returns the reserved presentation context ID</summary>
        public byte ProposePresentationContext(DicomUID sopClassUID, IEnumerable<DicomUID> proposedTransferSyntaxUIDs)
        {
            var presentationContextID = 1 + 2 * _presentationContextRequests.Count;
            if (presentationContextID > byte.MaxValue)
            {
                throw new Exception("Out of presentation context IDs");
            }

            var presentationContextRequest = new PresentationContextRequest
            {
                PresentationContextID = (byte)presentationContextID,
                AbstractSyntaxName = sopClassUID,
            };

            presentationContextRequest.TransferSyntaxNames.AddRange(proposedTransferSyntaxUIDs.Select(o => (string)o));

            _presentationContextRequests.Add(presentationContextRequest);

            return presentationContextRequest.PresentationContextID;
        }

        /// <summary>Registers a proposed presentation context and returns the reserved presentation context ID</summary>
        public byte ProposePresentationContext(DicomUID sopClassUID, params DicomUID[] proposedTransferSyntaxUIDs)
        {
            return ProposePresentationContext(sopClassUID, (IEnumerable<DicomUID>)proposedTransferSyntaxUIDs);
        }

        /// <summary>Cancellation token that can be set to enable interruption of blocking socket operations</summary>
        public CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        /// <summary>Connects to a remote application entity and establishes an association using the proposed presentation contexts</summary>
        public DicomAssociation ConnectTo(DicomNetworkAddress ae)
        {
            if (_presentationContextRequests.Count == 0)
            {
                throw new InvalidOperationException("Missing presentation context proposals");
            }

            var connection = DicomConnection.Connect(ae.HostNameOrIPAddress, ae.PortNumber, CancellationToken);
            try
            {
                connection.TraceWriter = TraceWriter;

                var associationRequest = new AssociationRequest
                {
                    CalledAETitle = ae.AETitle,
                    CallingAETitle = AETitle,
                };

                associationRequest.PresentationContextRequests.AddRange(_presentationContextRequests);

                connection.SendAssociationRequest(associationRequest);

                var associationResponse = connection.ReceiveAssociationResponse();

                var presentationContextTransferSyntaxes = new Dictionary<byte, DicomUID>();

                foreach (var presentationContextResponse in associationResponse.PresentationContextResponses)
                {
                    var presentationContextID = presentationContextResponse.PresentationContextID;
                    
                    if (presentationContextResponse.Result == PresentationContextResponse.ResultCode.Acceptance)
                    {
                        var transferSyntaxUID = presentationContextResponse.TransferSyntaxName;
                        presentationContextTransferSyntaxes[presentationContextID] = transferSyntaxUID;
                    }
                }

                return new DicomAssociation(connection, presentationContextTransferSyntaxes);
            }
            catch (Exception)
            {
                connection.Dispose();
                throw;
            }
        }

#pragma warning disable 1591

        public StreamWriter TraceWriter { get; set; }

#pragma warning restore 1591
    }
}
