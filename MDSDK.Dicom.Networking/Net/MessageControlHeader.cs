// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using System;

namespace MDSDK.Dicom.Networking.Net
{
    [Flags]
    internal enum MessageControlHeader : byte
    {
        IsCommand = 0x01,
        IsLastFragment = 0x02
    }
}
