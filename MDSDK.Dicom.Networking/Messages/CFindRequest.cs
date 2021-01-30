// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.C_FIND_RQ, true)]
    public sealed class CFindRequest : Request
    {
        public RequestPriority Priority { get; set; }
    }
}
