// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.C_GET_RQ, true)]
    public sealed class CGetRequest : Request
    {
        public string AffectedSOPClassUID { get; set; }

        public ushort Priority { get; set; }
    }

    public class CGetRequestMessage<TIdentifier> : DicomMessage<CGetRequest, TIdentifier>
        where TIdentifier : new()
    {
    }
}
