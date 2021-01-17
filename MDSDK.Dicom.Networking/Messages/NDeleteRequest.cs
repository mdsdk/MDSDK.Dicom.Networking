// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.N_DELETE_RQ, false)]
    public sealed class NDeleteRequest : Request
    {
        public string RequestedSOPClassUID { get; set; }

        public string RequestedSOPInstanceUID { get; set; }
    }

    public class NDeleteRequestMessage : DicomMessage<NDeleteRequest>
    {
    }
}
