// This is a generated file. Do not modify.

using MDSDK.Dicom.Serialization;

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.N_ACTION_RQ, true)]
    public class NActionRequest : IRequest
    {
        public string RequestedSOPClassUID { get; set; }

        public CommandType CommandField { get; set; }

        public ushort MessageID { get; set; }

        public ushort CommandDataSetType { get; set; }

        public string RequestedSOPInstanceUID { get; set; }

        public ushort ActionTypeID { get; set; }
    }
}
