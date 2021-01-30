// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.N_GET_RSP, true)]
    public sealed class NGetResponse : Response
    {
        public string AffectedSOPInstanceUID { get; set; }
    }
}
