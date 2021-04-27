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

        public override void ReadContentFrom(BinaryDataReader dataReader)
        {
            ImplementationClassUID = dataReader.Input.ReadRemainingBytes();
        }

        public override void WriteContentTo(BinaryDataWriter dataWriter)
        {
            dataWriter.Write(ImplementationClassUID);
        }
    }
}
