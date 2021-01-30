// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.N_DELETE_RSP, false)]
    public sealed class NDeleteResponse : Response
    {
        public string AffectedSOPInstanceUID { get; set; }
    }
}
