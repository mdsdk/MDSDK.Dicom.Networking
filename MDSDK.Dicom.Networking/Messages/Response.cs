// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    public abstract class Response : Command
    {
        internal Response() { }

        public ushort MessageIDBeingRespondedTo { get; set; }
    }
}
