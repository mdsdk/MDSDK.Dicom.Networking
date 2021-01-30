// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    public abstract class Response : Command
    {
        internal Response() { }

        public ushort MessageIDBeingRespondedTo { get; set; }
        
        public ushort Status { get; set; }

        public bool IsSuccess() => Status == 0x000;

        public bool IsPending() => Status == 0xFF00 || Status == 0xFF01;

        public bool IsCancel() => Status == 0xFE00;
    }
}
