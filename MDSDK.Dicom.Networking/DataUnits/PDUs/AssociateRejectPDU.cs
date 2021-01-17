// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;

namespace MDSDK.Dicom.Networking.DataUnits.PDUs
{
    class AssociateRejectPDU : PDU
    {
        public AssociateRejectPDU()
            : base(DataUnitType.AssociateRejectPDU)
        {
        }

        public byte Result { get; set; }

        public byte Source { get; set; }

        public byte Reason { get; set; }

        public override void ReadContentFrom(BinaryStreamReader input)
        {
            input.SkipBytes(1);

            Result = input.ReadByte();
            Source = input.ReadByte();
            Reason = input.ReadByte();
        }

        public override void WriteContentTo(BinaryStreamWriter output)
        {
            output.WriteZeros(1);

            output.WriteByte(Result);
            output.WriteByte(Source);
            output.WriteByte(Reason);
        }
    }
}
