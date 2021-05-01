// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

#pragma warning disable 1591

namespace MDSDK.Dicom.Networking.Messages
{
    public interface IRequest : ICommand
    {
        ushort MessageID { get; set; }
    }
}

#pragma warning restore 1591
