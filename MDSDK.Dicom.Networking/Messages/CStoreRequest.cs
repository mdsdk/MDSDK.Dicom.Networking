// This is a generated file. Do not modify.

using MDSDK.Dicom.Serialization;

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.C_STORE_RQ, true)]
    public class CStoreRequest : IRequest
    {
        public string AffectedSOPClassUID { get; set; }

        public CommandType CommandField { get; set; }

        public ushort MessageID { get; set; }

        public RequestPriority Priority { get; set; }

        public ushort CommandDataSetType { get; set; }

        public string AffectedSOPInstanceUID { get; set; }

        public string MoveOriginatorApplicationEntityTitle { get; set; }

        public ushort MoveOriginatorMessageID { get; set; }
    }
}
