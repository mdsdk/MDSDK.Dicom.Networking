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

        public override void ReadContentFrom(BinaryDataReader dataReader)
        {
            MaximumNumberOperationsInvoked = dataReader.Read<UInt16>();
            MaximumNumberOperationsPerformed = dataReader.Read<UInt16>();
        }

        public override void WriteContentTo(BinaryDataWriter dataWriter)
        {
            dataWriter.Write<UInt16>(MaximumNumberOperationsInvoked);
            dataWriter.Write<UInt16>(MaximumNumberOperationsPerformed);
        }
    }
}
