// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.C_STORE_RSP, false)]
    public sealed class CStoreResponse : Response
    {
        public string AffectedSOPClassUID { get; set; }

        public ushort Status { get; set; }

        public string AffectedSOPInstanceUID { get; set; }
    }

    public class CStoreResponseMessage : DicomMessage<CStoreResponse>
    {
    }
}
