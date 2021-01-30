// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Networking.Messages;
using MDSDK.Dicom.Networking.Net;
using MDSDK.Dicom.Serialization;
using MDSDK.Dicom.Serialization.TransferSyntaxes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace MDSDK.Dicom.Networking.SCUs
{
    public class DicomClient
    {
        public string AETitle { get; set; }

        public CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        private List<PresentationContextRequest> _presentationContextRequests = new List<PresentationContextRequest>();
        
        public byte ProposePresentationContext(DicomUID sopClassUID, params TransferSyntax[] proposedTransferSyntaxes)
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

            presentationContextRequest.TransferSyntaxNames.AddRange(proposedTransferSyntaxes.Select(o => o.DicomUID.UID));

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

            var connection = DicomConnection.Connect(ae.HostNameOrIPAddress, ae.Port, CancellationToken);
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

                var presentationContextTransferSyntaxes = new Dictionary<byte, TransferSyntax>();

                foreach (var presentationContextResponse in associationResponse.PresentationContextResponses)
                {
                    var presentationContextID = presentationContextResponse.PresentationContextID;
                    
                    if (presentationContextResponse.Result == PresentationContextResponse.ResultCode.Acceptance)
                    {
                        DicomTransferSyntax.TryLookup(presentationContextResponse.TransferSyntaxName, out TransferSyntax transferSyntax);
                        presentationContextTransferSyntaxes[presentationContextID] = transferSyntax;
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
