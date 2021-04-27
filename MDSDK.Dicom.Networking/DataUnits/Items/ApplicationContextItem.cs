// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;

namespace MDSDK.Dicom.Networking.DataUnits.Items
{
    class ApplicationContextItem : Item
    {
        public ApplicationContextItem()
            : base(DataUnitType.ApplicationContextItem)
        {
        }

        public byte[] ApplicationContextName { get; set; }

        public override void ReadContentFrom(BinaryDataReader dataReader)
        {
            ApplicationContextName = dataReader.Input.ReadRemainingBytes();
        }

        public override void WriteContentTo(BinaryDataWriter dataWriter)
        {
            dataWriter.Output.WriteBytes(ApplicationContextName);
        }
    }
}
