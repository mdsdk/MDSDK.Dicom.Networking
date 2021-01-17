// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;

namespace MDSDK.Dicom.Networking.DataUnits.SubItems
{
    class ImplementationClassUIDSubItem : SubItem
    {
        public ImplementationClassUIDSubItem()
            : base(DataUnitType.ImplementationClassUIDSubItem)
        {
        }

        public byte[] ImplementationClassUID { get; set; }

        public override void ReadContentFrom(BinaryStreamReader input)
        {
            ImplementationClassUID = input.ReadRemainingBytes();
        }

        public override void WriteContentTo(BinaryStreamWriter output)
        {
            output.WriteBytes(ImplementationClassUID);
        }
    }
}
