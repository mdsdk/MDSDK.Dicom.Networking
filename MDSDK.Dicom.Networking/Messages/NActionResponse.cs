// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.N_ACTION_RSP, true)]
    public sealed class NActionResponse : Response
    {
        public string AffectedSOPInstanceUID { get; set; }

        public ushort ActionTypeID { get; set; }
    }
}
