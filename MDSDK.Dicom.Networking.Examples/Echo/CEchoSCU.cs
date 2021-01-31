// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Networking.Messages;
using MDSDK.Dicom.Networking;
using MDSDK.Dicom.Serialization;
using System;

namespace MDSDK.Dicom.Networking.Examples.Echo
{
    public class CEchoSCU
    {
        public byte PresentationContextID { get; }

        public CEchoSCU(DicomClient client)
        {
            PresentationContextID = client.ProposePresentationContext(DicomUID.VerificationSOPClass,
                DicomTransferSyntax.ImplicitVRLittleEndian);
        }

        public void Ping(DicomAssociation association)
        {
            Console.WriteLine("Executing C-ECHO");
            
            var cEchoRequest = new CEchoRequest
            {
                AffectedSOPClassUID = DicomUID.VerificationSOPClass.UID
            };

            association.SendRequest(PresentationContextID, cEchoRequest, CommandIsFollowedByDataSet.No);

            var cEchoResponse = association.ReceiveResponse<CEchoResponse>(PresentationContextID, cEchoRequest.MessageID);

            Console.WriteLine($"C-ECHO status = {cEchoResponse.Status}");
        }
    }
}
