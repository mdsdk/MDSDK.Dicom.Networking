// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;

namespace MDSDK.Dicom.Networking.DataUnits.PDUs
{
    class ReleaseRequestPDU : PDU
    {
        public ReleaseRequestPDU()
            : base(DataUnitType.ReleaseRequestPDU)
        {
        }

        public override void ReadContentFrom(BinaryDataReader dataReader)
        {
            dataReader.Input.SkipBytes(4);
        }

        public override void WriteContentTo(BinaryDataWriter dataWriter)
        {
            dataWriter.WriteZeros(4);
        }
    }
}
