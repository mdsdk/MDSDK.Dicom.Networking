// This is a generated file. Do not modify.

#pragma warning disable 1591

using MDSDK.Dicom.Serialization;

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.C_MOVE_RSP)]
    public class CMoveResponse : IResponse, IMayHaveDataSet
    {
        public string AffectedSOPClassUID { get; set; }

        public CommandType CommandField { get; set; }

        public ushort MessageIDBeingRespondedTo { get; set; }

        public ushort CommandDataSetType { get; set; }

        public ushort Status { get; set; }

        public ushort NumberOfRemainingSuboperations { get; set; }

        public ushort NumberOfCompletedSuboperations { get; set; }

        public ushort NumberOfFailedSuboperations { get; set; }

        public ushort NumberOfWarningSuboperations { get; set; }
    }
}

#pragma warning restore 1591
