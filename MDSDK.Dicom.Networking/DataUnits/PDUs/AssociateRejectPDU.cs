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

        public override void ReadContentFrom(BinaryDataReader dataReader)
        {
            dataReader.Input.SkipBytes(1);

            Result = dataReader.ReadByte();
            Source = dataReader.ReadByte();
            Reason = dataReader.ReadByte();
        }

        public override void WriteContentTo(BinaryDataWriter dataWriter)
        {
            dataWriter.Write((byte)0);

            dataWriter.Write(Result);
            dataWriter.Write(Source);
            dataWriter.Write(Reason);
        }
    }
}
