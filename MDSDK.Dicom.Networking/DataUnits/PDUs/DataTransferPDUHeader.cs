// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;
using System;

namespace MDSDK.Dicom.Networking.DataUnits.PDUs
{
    class DataTransferPDUHeader : PDU
    {
        public DataTransferPDUHeader()
            : base(DataUnitType.DataTransferPDU)
        {
        }

        public const int Size = 6;

        public override void ReadContentFrom(BinaryStreamReader input)
        {
            throw new NotSupportedException();
        }

        public override void WriteContentTo(BinaryStreamWriter output)
        {
            throw new NotSupportedException();
        }

        public new void WriteTo(BinaryStreamWriter output)
        {
            output.WriteByte((byte)DataUnitType);
            output.WriteZeros(1);
            output.Write<UInt32>(Length);
        }
    }
}
