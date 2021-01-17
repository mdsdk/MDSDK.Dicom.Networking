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

        public override void ReadContentFrom(BinaryStreamReader input)
        {
            input.SkipBytes(2);

            Source = input.ReadByte();
            Reason = input.ReadByte();
        }

        public override void WriteContentTo(BinaryStreamWriter output)
        {
            output.WriteZeros(2);

            output.WriteByte(Source);
            output.WriteByte(Reason);
        }
    }
}
