﻿// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Networking.Net;
using MDSDK.Dicom.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace MDSDK.Dicom.Networking
{
    public class DicomClient
    {
        public string AETitle { get; }

        public DicomClient(string aeTitle)
        {
            AETitle = aeTitle;
        }

        public CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        private List<PresentationContextRequest> _presentationContextRequests = new List<PresentationContextRequest>();
        
        public byte ProposePresentationContext(DicomUID sopClassUID, params DicomTransferSyntax[] proposedTransferSyntaxes)
        {
            var presentationContextID = 1 + 2 * _presentationContextRequests.Count;
            if (presentationContextID > byte.MaxValue)
            {
                throw new Exception("Out of presentation context IDs");
            }

            var presentationContextRequest = new PresentationContextRequest
            {
                PresentationContextID = (byte)presentationContextID,
                AbstractSyntaxName = sopClassUID.UID,
            };

            presentationContextRequest.TransferSyntaxNames.AddRange(proposedTransferSyntaxes.Select(o => o.UID.UID));

            _presentationContextRequests.Add(presentationContextRequest);

            return presentationContextRequest.PresentationContextID;
        }

        public StreamWriter TraceWriter { get; set; }

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

                var presentationContextTransferSyntaxes = new Dictionary<byte, DicomTransferSyntax>();

                foreach (var presentationContextResponse in associationResponse.PresentationContextResponses)
                {
                    var presentationContextID = presentationContextResponse.PresentationContextID;
                    
                    if (presentationContextResponse.Result == PresentationContextResponse.ResultCode.Acceptance)
                    {
                        var transferSyntaxUID = new DicomUID(presentationContextResponse.TransferSyntaxName);
                        presentationContextTransferSyntaxes[presentationContextID] = new DicomTransferSyntax(transferSyntaxUID);
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
    }
}
