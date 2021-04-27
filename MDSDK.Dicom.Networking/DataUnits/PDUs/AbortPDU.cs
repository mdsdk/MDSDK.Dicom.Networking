// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;

namespace MDSDK.Dicom.Networking.DataUnits.PDUs
{
    class AbortPDU : PDU
    {
        public AbortPDU()
            : base(DataUnitType.AbortPDU)
        {
        }

        public byte Source { get; set; }

        public byte Reason { get; set; }

        public override void ReadContentFrom(BinaryDataReader dataReader)
        {
            dataReader.Input.SkipBytes(2);

            Source = dataReader.ReadByte();
            Reason = dataReader.ReadByte();
        }

        public override void WriteContentTo(BinaryDataWriter dataWriter)
        {
            dataWriter.WriteZeros(2);

            dataWriter.Write(Source);
            dataWriter.Write(Reason);
        }
    }
}
