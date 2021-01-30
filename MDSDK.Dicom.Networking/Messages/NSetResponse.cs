// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.N_SET_RSP, true)]
    public sealed class NSetResponse : Response
    {
        public string AffectedSOPInstanceUID { get; set; }
    }
}
