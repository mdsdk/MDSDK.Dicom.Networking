// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.N_CREATE_RQ, true)]
    public sealed class NCreateRequest : Request
    {
        public string AffectedSOPClassUID { get; set; }

        public string AffectedSOPInstanceUID { get; set; }
    }

    public class NCreateRequestMessage<TAttributeList> : DicomMessage<NCreateRequest, TAttributeList>
        where TAttributeList : new()
    {
    }
}
