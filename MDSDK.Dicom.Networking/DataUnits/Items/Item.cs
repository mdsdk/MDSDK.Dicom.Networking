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

        protected override long ReadContentLength(BinaryStreamReader input)
        {
            return input.Read<UInt16>();
        }

        protected override void WriteContentLength(BinaryStreamWriter output, long length)
        {
            output.Write<UInt16>(checked((ushort)length));
        }

        public static Item ReadFrom(BinaryStreamReader input)
        {
            var dataUnitType = (DataUnitType)input.ReadByte();
            var item = ItemFactory.Instance.Create(dataUnitType);
            input.SkipBytes(1);
            Read16BitLengthPrefixedData(input, () => item.ReadContentFrom(input));
            return item;
        }

        public static void ReadItems(BinaryStreamReader input, ICollection<Item> items)
        {
            while (!input.AtEnd)
            {
                var item = ReadFrom(input);
                items.Add(item);
            }
        }
    }
}
