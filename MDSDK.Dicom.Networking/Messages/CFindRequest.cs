// This is a generated file. Do not modify.

#pragma warning disable 1591

using MDSDK.Dicom.Serialization;

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.C_FIND_RQ)]
    public class CFindRequest : IRequest, IMayHaveDataSet
    {
        public string AffectedSOPClassUID { get; set; }

        public CommandType CommandField { get; set; }

        public ushort MessageID { get; set; }

        public RequestPriority Priority { get; set; }

        public ushort CommandDataSetType { get; set; }
    }
}

#pragma warning restore 1591
