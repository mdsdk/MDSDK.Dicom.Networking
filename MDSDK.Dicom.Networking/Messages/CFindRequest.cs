// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.C_FIND_RQ, true)]
    public sealed class CFindRequest : Request
    {
        public string AffectedSOPClassUID { get; set; }

        public ushort Priority { get; set; }
    }

    public class CFindRequestMessage<TIdentifier> : DicomMessage<CFindRequest, TIdentifier>
        where TIdentifier : new()
    {
    }
}
