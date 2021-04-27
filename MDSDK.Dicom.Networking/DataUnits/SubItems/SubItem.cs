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

        internal override long ReadContentLength(BinaryDataReader dataReader)
        {
            return dataReader.Read<UInt16>();
        }

        internal override void WriteContentLength(BinaryDataWriter dataWriter, long length)
        {
            dataWriter.Write<UInt16>(checked((ushort)length));
        }

        public static bool IsUserInformationSubItemType(DataUnitType dataUnitType)
        {
            return (dataUnitType >= DataUnitType.UserInformationSubItemRangeStart)
                && (dataUnitType <= DataUnitType.UserInformationSubItemRangeEnd);
        }

        protected static byte[] Read16BitLengthPrefixedByteArray(BinaryDataReader dataReader)
        {
            var length = dataReader.Read<UInt16>();
            return dataReader.Input.ReadBytes(length);
        }

        protected static void Write16BitLengthPrefixedByteArray(BinaryDataWriter dataWriter, byte[] bytes)
        {
            dataWriter.Write<UInt16>(checked((ushort)bytes.Length));
            dataWriter.Output.WriteBytes(bytes);
        }

        protected static string Read16BitLengthPrefixedAsciiString(BinaryDataReader dataReader)
        {
            var bytes = Read16BitLengthPrefixedByteArray(dataReader);
            return Encoding.ASCII.GetString(bytes).Trim();
        }

        protected static void Write16BitLengthPrefixedAsciiString(BinaryDataWriter dataWriter, string s)
        {
            var bytes = Encoding.ASCII.GetBytes(s);
            Write16BitLengthPrefixedByteArray(dataWriter, bytes);
        }

        protected static string ReadNonLengthPrefixedAsciiString(BinaryDataReader dataReader)
        {
            var bytes = dataReader.Input.ReadRemainingBytes();
            return Encoding.ASCII.GetString(bytes).Trim();
        }

        protected static void WriteNonLengthPrefixedAsciiString(BinaryDataWriter dataWriter, string s)
        {
            var bytes = Encoding.ASCII.GetBytes(s);
            dataWriter.Write(bytes);
        }

        public static SubItem ReadFrom(BinaryDataReader dataReader)
        {
            var dataUnitType = (DataUnitType)dataReader.ReadByte();
            var subItem = SubItemFactory.Instance.Create(dataUnitType);
            dataReader.Input.SkipBytes(1);
            Read16BitLengthPrefixedData(dataReader, () => subItem.ReadContentFrom(dataReader));
            return subItem;
        }

        public static void ReadSubItems(BinaryDataReader dataReader, ICollection<SubItem> subItems)
        {
            while (!dataReader.Input.AtEnd)
            {
                var subItem = ReadFrom(dataReader);
                subItems.Add(subItem);
            }
        }
    }
}
