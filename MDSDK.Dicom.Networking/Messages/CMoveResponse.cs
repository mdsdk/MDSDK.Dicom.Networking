// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.C_MOVE_RSP, true)]
    public sealed class CMoveResponse : Response
    {
        public string AffectedSOPClassUID { get; set; }

        public ushort Status { get; set; }

        public ushort NumberOfRemainingSuboperations { get; set; }

        public ushort NumberOfCompletedSuboperations { get; set; }

        public ushort NumberOfFailedSuboperations { get; set; }

        public ushort NumberOfWarningSuboperations { get; set; }
    }

    public class CMoveResponseMessage<TIdentifier> : DicomMessage<CMoveResponse, TIdentifier>
        where TIdentifier : new()
    {
    }
}
