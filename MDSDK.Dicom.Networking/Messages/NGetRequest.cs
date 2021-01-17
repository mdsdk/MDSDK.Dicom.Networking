// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Serialization;

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.N_GET_RQ, false)]
    public sealed class NGetRequest : Request
    {
        public string RequestedSOPClassUID { get; set; }

        public string RequestedSOPInstanceUID { get; set; }

        public DicomTag[] AttributeIdentifierList { get; set; }
    }

    public class NGetRequestMessage : DicomMessage<NGetRequest>
    {
    }
}
