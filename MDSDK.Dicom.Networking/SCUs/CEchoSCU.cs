// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Networking.Messages;
using MDSDK.Dicom.Networking.Net;
using MDSDK.Dicom.Serialization;
using System.Collections.Generic;

namespace MDSDK.Dicom.Networking.SCUs
{
    public class CEchoSCU : DicomClient
    {
        protected override void WritePresentationContextRequests(IList<PresentationContextRequest> presentationContextRequests)
        {
            presentationContextRequests.Add(new PresentationContextRequest
            {
                PresentationContextID = 1,
                AbstractSyntaxName = DicomUID.VerificationSOPClass.UID,
                TransferSyntaxNames = new[] { DicomUID.ImplicitVRLittleEndian.UID }
            });
        }

        protected override void ReadPresentationContextResponses(IReadOnlyList<PresentationContextResponse> presentationContextResponses)
        {
            NetUtils.ThrowIf(presentationContextResponses.Count != 1);
            NetUtils.ThrowIf(presentationContextResponses[0].PresentationContextID != 1);
            NetUtils.ThrowIf(presentationContextResponses[0].TransferSyntaxName != DicomUID.ImplicitVRLittleEndian.UID);
        }

        public CEchoResponse Call(CEchoRequest cEchoRequest)
        {
            Send(1, cEchoRequest);
            return Receive<CEchoResponse>(1);
        }
    }
}
