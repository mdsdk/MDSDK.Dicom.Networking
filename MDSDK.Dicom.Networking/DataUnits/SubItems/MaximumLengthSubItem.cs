// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;
using System;

namespace MDSDK.Dicom.Networking.DataUnits.SubItems
{
    class MaximumLengthSubItem : SubItem
    {
        public MaximumLengthSubItem()
            : base(DataUnitType.MaximumLengthSubItem)
        {
        }

        public uint MaximumLength { get; set; }

        public override void ReadContentFrom(BinaryStreamReader input)
        {
            MaximumLength = input.Read<UInt32>();
        }

        public override void WriteContentTo(BinaryStreamWriter output)
        {
            output.Write<UInt32>(MaximumLength);
        }
    }
}
