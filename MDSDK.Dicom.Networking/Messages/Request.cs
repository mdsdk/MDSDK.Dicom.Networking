// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    public abstract class Request : Command
    {
        internal Request() { }

        public ushort MessageID { get; set; }
    }
}
