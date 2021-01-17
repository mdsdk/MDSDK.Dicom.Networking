// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace MDSDK.Dicom.Networking.DataUnits
{
    abstract class DataUnit
    {
        public DataUnitType DataUnitType { get; }

        protected DataUnit(DataUnitType dataUnitType)
        {
            DataUnitType = dataUnitType;
        }

        protected abstract long ReadContentLength(BinaryStreamReader input);

        protected abstract void WriteContentLength(BinaryStreamWriter output, long length);

        public abstract void ReadContentFrom(BinaryStreamReader input);

        public abstract void WriteContentTo(BinaryStreamWriter output);

        protected static void WriteDataUnits<T>(BinaryStreamWriter output, IReadOnlyList<T> dataUnits)
            where T : DataUnit
        {
            foreach (var dataUnit in dataUnits)
            {
                dataUnit.WriteTo(output);
            }
        }

        protected static void Read16BitLengthPrefixedData(BinaryStreamReader input, Action readAction)
        {
            var length = input.Read<UInt16>();
            input.Read(length, readAction);
        }

        protected static void Write16BitLength(BinaryStreamWriter output, long length)
        {
            output.Write<UInt16>(checked((ushort)length));
        }

        protected static void Write32BitLength(BinaryStreamWriter output, long length)
        {
            output.Write<UInt32>(checked((uint)length));
        }

        protected static void WriteLengthPrefixedData(BinaryStreamWriter output, Action<BinaryStreamWriter, long> lengthWriter,
            Action<BinaryStreamWriter> dataWriter)
        {
            using (var dataStream = new MemoryStream())
            {
                var dataOutput = new BinaryStreamWriter(dataStream, output.ByteOrder);
                dataWriter.Invoke(dataOutput);
                dataOutput.Flush(FlushMode.Deep);
                lengthWriter.Invoke(output, dataStream.Length);
                output.WriteBytes(dataStream.GetBuffer().AsSpan(0, checked((int)dataStream.Length)));
            }
        }

        internal void WriteTo(BinaryStreamWriter output)
        {
            output.WriteByte((byte)DataUnitType);
            output.WriteByte(0);
            WriteLengthPrefixedData(output, WriteContentLength, WriteContentTo);
        }
    }
}
