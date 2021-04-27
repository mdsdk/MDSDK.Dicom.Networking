// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;

namespace MDSDK.Dicom.Networking.DataUnits.SubItems
{
    class ImplementationVersionNameSubItem : SubItem
    {
        public ImplementationVersionNameSubItem()
            : base(DataUnitType.ImplementationVersionNameSubItem)
        {
        }

        public byte[] ImplementationVersionName { get; set; }

        public override void ReadContentFrom(BinaryDataReader dataReader)
        {
            ImplementationVersionName = dataReader.Input.ReadRemainingBytes();
        }

        public override void WriteContentTo(BinaryDataWriter dataWriter)
        {
            dataWriter.Write(ImplementationVersionName);
        }
    }
}
