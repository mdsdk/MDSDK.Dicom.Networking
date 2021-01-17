// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;

namespace MDSDK.Dicom.Networking.DataUnits.PDUs
{
    class ReleaseResponsePDU : PDU
    {
        public ReleaseResponsePDU()
            : base(DataUnitType.ReleaseResponsePDU)
        {
        }

        public override void ReadContentFrom(BinaryStreamReader input)
        {
            input.SkipBytes(4);
        }

        public override void WriteContentTo(BinaryStreamWriter output)
        {
            output.WriteZeros(4);
        }
    }
}
