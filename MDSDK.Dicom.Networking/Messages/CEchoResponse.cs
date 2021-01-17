// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.C_ECHO_RSP, false)]
    public sealed class CEchoResponse : Response
    {
        public string AffectedSOPClassUID { get; set; }

        public ushort Status { get; set; }
    }

    public class CEchoResponseMessage : DicomMessage<CEchoResponse>
    {
    }
}
