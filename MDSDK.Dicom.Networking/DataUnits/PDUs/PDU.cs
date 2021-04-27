// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;
using System;

namespace MDSDK.Dicom.Networking.DataUnits.PDUs
{
    abstract class PDU : DataUnit
    {
        public uint Length { get; set; }

        protected PDU(DataUnitType pduType)
            : base(pduType)
        {
        }

        internal override long ReadContentLength(BinaryDataReader dataReader)
        {
            return dataReader.Read<UInt32>();
        }

        internal override void WriteContentLength(BinaryDataWriter dataWriter, long length)
        {
            dataWriter.Write(checked((uint)length));
        }

        public static PDU ReadHeaderFrom(BufferedStreamReader input)
        {
            var dataReader = new BinaryDataReader(input, ByteOrder.BigEndian);
            var dataUnitType = (DataUnitType)dataReader.ReadByte();
            var pdu = PDUFactory.Instance.Create(dataUnitType);
            dataReader.Input.SkipBytes(1);
            pdu.Length = dataReader.Read<UInt32>();
            return pdu;
        }

        public static void ReadFrom(BufferedStreamReader input, Action<PDU> readAction)
        {
            var pdu = ReadHeaderFrom(input);
            input.Read(pdu.Length, () => readAction.Invoke(pdu));
        }
    }
}
