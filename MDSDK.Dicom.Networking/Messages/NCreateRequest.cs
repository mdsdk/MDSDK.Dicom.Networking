// This is a generated file. Do not modify.

#pragma warning disable 1591

using MDSDK.Dicom.Serialization;

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.N_CREATE_RQ)]
    public class NCreateRequest : IRequest, IMayHaveDataSet
    {
        public string AffectedSOPClassUID { get; set; }

        public CommandType CommandField { get; set; }

        public ushort MessageID { get; set; }

        public ushort CommandDataSetType { get; set; }

        public string AffectedSOPInstanceUID { get; set; }
    }
}

#pragma warning restore 1591
