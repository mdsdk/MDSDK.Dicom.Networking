// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.N_GET_RSP, true)]
    public sealed class NGetResponse : Response
    {
        public string AffectedSOPClassUID { get; set; }

        public ushort Status { get; set; }

        public string AffectedSOPInstanceUID { get; set; }
    }

    public class NGetResponseMessage<TAttributeList> : DicomMessage<NGetResponse, TAttributeList>
        where TAttributeList : new()
    {
    }
}
