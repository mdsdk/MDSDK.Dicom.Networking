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

        protected override long ReadContentLength(BinaryStreamReader input)
        {
            return input.Read<UInt32>();
        }

        protected override void WriteContentLength(BinaryStreamWriter output, long length)
        {
            output.Write<UInt32>(checked((uint)length));
        }

        public static PDU ReadHeaderFrom(BinaryStreamReader input)
        {
            var dataUnitType = (DataUnitType)input.ReadByte();
            var pdu = PDUFactory.Instance.Create(dataUnitType);
            input.SkipBytes(1);
            pdu.Length = input.Read<UInt32>();
            return pdu;
        }

        public static void ReadFrom(BinaryStreamReader input, Action<PDU> readAction)
        {
            var pdu = ReadHeaderFrom(input);
            input.Read(pdu.Length, () => readAction.Invoke(pdu));
        }
    }
}
