// This is a generated file. Do not modify.

using MDSDK.Dicom.Serialization;

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.C_MOVE_RSP, true)]
    public class CMoveResponse : IResponse
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
