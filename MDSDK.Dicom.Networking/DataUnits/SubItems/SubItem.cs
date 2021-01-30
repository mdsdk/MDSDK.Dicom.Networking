// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;
using System;
using System.Collections.Generic;
using System.Text;

namespace MDSDK.Dicom.Networking.DataUnits.SubItems
{
    abstract class SubItem : DataUnit
    {
        internal SubItem(DataUnitType subItemType)
            : base(subItemType)
        {
        }

        internal override long ReadContentLength(BinaryStreamReader input)
        {
            return input.Read<UInt16>();
        }

        internal override void WriteContentLength(BinaryStreamWriter output, long length)
        {
            output.Write<UInt16>(checked((ushort)length));
        }

        public static bool IsUserInformationSubItemType(DataUnitType dataUnitType)
        {
            return (dataUnitType >= DataUnitType.UserInformationSubItemRangeStart)
                && (dataUnitType <= DataUnitType.UserInformationSubItemRangeEnd);
        }

        protected static byte[] Read16BitLengthPrefixedByteArray(BinaryStreamReader input)
        {
            var length = input.Read<UInt16>();
            return input.ReadBytes(length);
        }

        protected static void Write16BitLengthPrefixedByteArray(BinaryStreamWriter output, byte[] bytes)
        {
            output.Write<UInt16>(checked((ushort)bytes.Length));
            output.WriteBytes(bytes);
        }

        protected static string Read16BitLengthPrefixedAsciiString(BinaryStreamReader input)
        {
            var bytes = Read16BitLengthPrefixedByteArray(input);
            return Encoding.ASCII.GetString(bytes).Trim();
        }

        protected static void Write16BitLengthPrefixedAsciiString(BinaryStreamWriter output, string s)
        {
            var bytes = Encoding.ASCII.GetBytes(s);
            Write16BitLengthPrefixedByteArray(output, bytes);
        }

        protected static string ReadNonLengthPrefixedAsciiString(BinaryStreamReader input)
        {
            var bytes = input.ReadRemainingBytes();
            return Encoding.ASCII.GetString(bytes).Trim();
        }

        protected static void WriteNonLengthPrefixedAsciiString(BinaryStreamWriter output, string s)
        {
            var bytes = Encoding.ASCII.GetBytes(s);
            output.WriteBytes(bytes);
        }

        public static SubItem ReadFrom(BinaryStreamReader input)
        {
            var dataUnitType = (DataUnitType)input.ReadByte();
            var subItem = SubItemFactory.Instance.Create(dataUnitType);
            input.SkipBytes(1);
            Read16BitLengthPrefixedData(input, () => subItem.ReadContentFrom(input));
            return subItem;
        }

        public static void ReadSubItems(BinaryStreamReader input, ICollection<SubItem> subItems)
        {
            while (!input.AtEnd)
            {
                var subItem = ReadFrom(input);
                subItems.Add(subItem);
            }
        }
    }
}
