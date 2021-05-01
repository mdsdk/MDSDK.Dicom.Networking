// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    /// <summary>Request priorities as defined by DICOM</summary>
    public enum RequestPriority : ushort
    {
        /// <summary>Medium priority</summary>
        Medium = 0x0000,
        
        /// <summary>High priority</summary>
        High = 0x0001,
        
        /// <summary>Low priority</summary>
        Low = 0x0002,
    }
}
