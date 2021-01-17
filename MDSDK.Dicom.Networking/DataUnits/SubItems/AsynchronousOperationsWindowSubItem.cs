// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;
using System;

namespace MDSDK.Dicom.Networking.DataUnits.SubItems
{
    class AsynchronousOperationsWindowSubItem : SubItem
    {
        public AsynchronousOperationsWindowSubItem()
            : base(DataUnitType.AsynchronousOperationsWindowSubItem)
        {
        }

        public ushort MaximumNumberOperationsInvoked { get; set; }

        public ushort MaximumNumberOperationsPerformed { get; set; }

        public override void ReadContentFrom(BinaryStreamReader input)
        {
            MaximumNumberOperationsInvoked = input.Read<UInt16>();
            MaximumNumberOperationsPerformed = input.Read<UInt16>();
        }

        public override void WriteContentTo(BinaryStreamWriter output)
        {
            output.Write<UInt16>(MaximumNumberOperationsInvoked);
            output.Write<UInt16>(MaximumNumberOperationsPerformed);
        }
    }
}
