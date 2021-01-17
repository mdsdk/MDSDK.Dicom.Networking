// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.C_MOVE_RQ, true)]
    public sealed class CMoveRequest : Request
    {
        public string AffectedSOPClassUID { get; set; }

        public ushort Priority { get; set; }

        public string MoveDestination { get; set; }
    }

    public class CMoveRequestMessage<TIdentifier> : DicomMessage<CMoveRequest, TIdentifier>
        where TIdentifier : new()
    {
    }
}
