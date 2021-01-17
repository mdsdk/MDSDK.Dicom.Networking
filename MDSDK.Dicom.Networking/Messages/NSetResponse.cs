// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.N_SET_RSP, true)]
    public sealed class NSetResponse : Response
    {
        public string AffectedSOPClassUID { get; set; }

        public ushort Status { get; set; }

        public string AffectedSOPInstanceUID { get; set; }
    }

    public class NSetResponseMessage<TAttributeList> : DicomMessage<NSetResponse, TAttributeList>
        where TAttributeList : new()
    {
    }
}
