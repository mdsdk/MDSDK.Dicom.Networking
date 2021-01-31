// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.C_GET_RQ, true)]
    public sealed class CGetRequest : Request
    {
        public RequestPriority Priority { get; set; }
    }
}
