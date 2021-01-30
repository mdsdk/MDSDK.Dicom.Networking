// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.C_GET_RSP, true)]
    public sealed class CGetResponse : Response
    {
        public ushort NumberOfRemainingSuboperations { get; set; }

        public ushort NumberOfCompletedSuboperations { get; set; }

        public ushort NumberOfFailedSuboperations { get; set; }

        public ushort NumberOfWarningSuboperations { get; set; }
    }
}
