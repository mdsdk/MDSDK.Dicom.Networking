// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;
using System;
using System.Collections.Generic;

namespace MDSDK.Dicom.Networking.DataUnits.Items
{
    abstract class Item : DataUnit
    {
        protected Item(DataUnitType itemType)
            : base(itemType)
        {
        }

        internal override long ReadContentLength(BinaryDataReader dataReader)
        {
            return dataReader.Read<UInt16>();
        }

        internal override void WriteContentLength(BinaryDataWriter dataWriter, long length)
        {
            dataWriter.Write<UInt16>(checked((ushort)length));
        }

        public static Item ReadFrom(BinaryDataReader dataReader)
        {
            var dataUnitType = (DataUnitType)dataReader.ReadByte();
            var item = ItemFactory.Instance.Create(dataUnitType);
            dataReader.Input.SkipBytes(1);
            Read16BitLengthPrefixedData(dataReader, () => item.ReadContentFrom(dataReader));
            return item;
        }

        public static void ReadItems(BinaryDataReader dataReader, ICollection<Item> items)
        {
            while (!dataReader.Input.AtEnd)
            {
                var item = ReadFrom(dataReader);
                items.Add(item);
            }
        }
    }
}
