// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Networking.Messages;
using MDSDK.Dicom.Networking.SCUs;
using MDSDK.Dicom.Serialization;
using System;

namespace MDSDK.Dicom.Networking.Examples.Echo
{
    public class CEchoSCU
    {
        private readonly byte _presentationContextID;

        public CEchoSCU(DicomClient client)
        {
            _presentationContextID = client.ProposePresentationContext(DicomUID.VerificationSOPClass,
                DicomTransferSyntax.ImplicitVRLittleEndian);
        }

        public void Ping(DicomAssociation association)
        {
            Console.WriteLine("Executing C-ECHO");
            
            var cEchoRequest = new CEchoRequest
            {
                AffectedSOPClassUID = DicomUID.VerificationSOPClass.UID
            };

            association.SendRequest(_presentationContextID, cEchoRequest);

            var cEchoResponse = association.ReceiveResponse<CEchoResponse>(_presentationContextID, cEchoRequest.MessageID);

            Console.WriteLine($"C-ECHO status = {cEchoResponse.Status}");
        }
    }
}
