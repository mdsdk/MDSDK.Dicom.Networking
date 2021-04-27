// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;
using System;
using System.Collections.Generic;
using System.IO;

namespace MDSDK.Dicom.Networking.DataUnits
{
    abstract class DataUnit
    {
        public DataUnitType DataUnitType { get; }

        internal DataUnit(DataUnitType dataUnitType)
        {
            DataUnitType = dataUnitType;
        }

        internal abstract long ReadContentLength(BinaryDataReader dataReader);

        internal abstract void WriteContentLength(BinaryDataWriter dataWriter, long length);

        public abstract void ReadContentFrom(BinaryDataReader dataReader);

        public abstract void WriteContentTo(BinaryDataWriter dataWriter);

        protected static void WriteDataUnits<T>(BinaryDataWriter dataWriter, IReadOnlyList<T> dataUnits)
            where T : DataUnit
        {
            foreach (var dataUnit in dataUnits)
            {
                dataUnit.WriteTo(dataWriter);
            }
        }

        protected static void Read16BitLengthPrefixedData(BinaryDataReader dataReader, Action readAction)
        {
            var length = dataReader.Read<UInt16>();
            dataReader.Input.Read(length, readAction);
        }

        protected static void Write16BitLength(BinaryDataWriter dataWriter, long length)
        {
            dataWriter.Write<UInt16>(checked((ushort)length));
        }

        protected static void Write32BitLength(BinaryDataWriter dataWriter, long length)
        {
            dataWriter.Write<UInt32>(checked((uint)length));
        }

        protected static void WriteLengthPrefixedData(BinaryDataWriter dataWriter, Action<BinaryDataWriter, long> lengthWriter,
            Action<BinaryDataWriter> writeDataAction)
        {
            using (var dataStream = new MemoryStream())
            {
                var dataOutput = new BufferedStreamWriter(dataStream);
                writeDataAction.Invoke(new BinaryDataWriter(dataOutput, ByteOrder.BigEndian));
                dataOutput.Flush(FlushMode.Shallow);
                lengthWriter.Invoke(dataWriter, dataStream.Length);
                dataWriter.Write(dataStream.GetBuffer().AsSpan(0, checked((int)dataStream.Length)));
            }
        }

        internal void WriteTo(BinaryDataWriter dataWriter)
        {
            dataWriter.Write((byte)DataUnitType);
            dataWriter.Write((byte)0);
            WriteLengthPrefixedData(dataWriter, WriteContentLength, WriteContentTo);
        }
    }
}
