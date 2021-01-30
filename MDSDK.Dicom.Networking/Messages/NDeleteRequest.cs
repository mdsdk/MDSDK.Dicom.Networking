// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.N_DELETE_RQ, false)]
    public sealed class NDeleteRequest : Request
    {
        public string RequestedSOPInstanceUID { get; set; }
    }
}
