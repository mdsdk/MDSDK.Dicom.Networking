// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

#pragma warning disable 1591

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.C_CANCEL_RQ)]
    public sealed class CCancelRequest : ICommand
    {
        public uint CommandGroupLength { get; set; }

        public CommandType CommandField { get; set; }

        public ushort MessageIDBeingRespondedTo { get; set; }

        public ushort CommandDataSetType { get; set; }
    }
}

#pragma warning restore 1591
