// This is a generated file. Do not modify.

using MDSDK.Dicom.Serialization;

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.N_EVENT_REPORT_RQ, true)]
    public class NEventReportRequest : IRequest
    {
        public string AffectedSOPClassUID { get; set; }

        public CommandType CommandField { get; set; }

        public ushort MessageID { get; set; }

        public ushort CommandDataSetType { get; set; }

        public string AffectedSOPInstanceUID { get; set; }

        public ushort EventTypeID { get; set; }
    }
}
