// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Serialization;

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.C_ECHO_RQ, false)]
    public sealed class CEchoRequest : Request
    {
        public string AffectedSOPClassUID { get; set; }

        public static CEchoRequest Create(ushort messageID)
        {
            var cEchoRequest = Create<CEchoRequest>();
            cEchoRequest.AffectedSOPClassUID = DicomUID.VerificationSOPClass.UID;
            cEchoRequest.MessageID = messageID;
            return cEchoRequest;
        }
    }
}
