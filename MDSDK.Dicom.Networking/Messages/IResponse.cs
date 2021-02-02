// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    public interface IResponse : ICommand
    {
        ushort MessageIDBeingRespondedTo { get; set; }

        ushort Status { get; set; }
    }
}
