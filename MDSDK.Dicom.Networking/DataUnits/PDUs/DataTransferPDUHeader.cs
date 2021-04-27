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

        public override void ReadContentFrom(BinaryDataReader dataReader)
        {
            throw new NotSupportedException();
        }

        public override void WriteContentTo(BinaryDataWriter dataWriter)
        {
            throw new NotSupportedException();
        }

        public new void WriteTo(BinaryDataWriter dataWriter)
        {
            dataWriter.Write((byte)DataUnitType);
            dataWriter.Write((byte)0);
            dataWriter.Write<UInt32>(Length);
        }
    }
}
