// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    class CommandHeader
    {
        public string AffectedSOPClassUID { get; set; }

        public string RequestedSOPClassUID { get; set; }

        public ushort CommandField { get; set; }
    }
}
