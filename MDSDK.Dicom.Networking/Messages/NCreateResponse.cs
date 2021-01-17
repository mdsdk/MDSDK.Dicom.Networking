// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.N_CREATE_RSP, true)]
    public sealed class NCreateResponse : Response
    {
        public string AffectedSOPClassUID { get; set; }

        public ushort Status { get; set; }

        public string AffectedSOPInstanceUID { get; set; }
    }

    public class NCreateResponseMessage<TAttributeList> : DicomMessage<NCreateResponse, TAttributeList>
        where TAttributeList : new()
    {
    }
}
