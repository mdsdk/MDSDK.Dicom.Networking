// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.C_CANCEL_RQ, false)]
    public sealed class CCancelRequest : Command
    {
        public ushort MessageIDBeingRespondedTo { get; set; }
    }
}
