// This is a generated file. Do not modify.

using MDSDK.Dicom.Serialization;

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.C_STORE_RSP, false)]
    public class CStoreResponse : IResponse
    {
        public string AffectedSOPClassUID { get; set; }

        public CommandType CommandField { get; set; }

        public ushort MessageIDBeingRespondedTo { get; set; }

        public ushort CommandDataSetType { get; set; }

        public ushort Status { get; set; }

        public string AffectedSOPInstanceUID { get; set; }
    }
}
