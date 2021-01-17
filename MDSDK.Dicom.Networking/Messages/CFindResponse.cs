// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.C_FIND_RSP, true)]
    public sealed class CFindResponse : Response
    {
        public string AffectedSOPClassUID { get; set; }

        public ushort Status { get; set; }
    }

    public class CFindResponseMessage<TIdentifier> : DicomMessage<CFindResponse, TIdentifier>
        where TIdentifier : new()
    {
    }
}
