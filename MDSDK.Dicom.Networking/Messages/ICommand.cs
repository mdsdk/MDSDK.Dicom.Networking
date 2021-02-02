// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    public interface ICommand
    {
        CommandType CommandField { get; set; }
        
        ushort CommandDataSetType { get; set; }
    }
}
