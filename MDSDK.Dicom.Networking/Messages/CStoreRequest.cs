// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.C_STORE_RQ, true)]
    public sealed class CStoreRequest : Request
    {
        public string AffectedSOPClassUID { get; set; }

        public ushort Priority { get; set; }

        public string AffectedSOPInstanceUID { get; set; }

        public string MoveOriginatorApplicationEntityTitle { get; set; }

        public ushort MoveOriginatorMessageID { get; set; }
    }

    public class CStoreRequestMessage<TDataSet> : DicomMessage<CStoreRequest, TDataSet>
        where TDataSet : new()
    {
    }
}
