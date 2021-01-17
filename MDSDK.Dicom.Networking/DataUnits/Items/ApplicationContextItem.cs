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

        public override void ReadContentFrom(BinaryStreamReader input)
        {
            ApplicationContextName = input.ReadRemainingBytes();
        }

        public override void WriteContentTo(BinaryStreamWriter output)
        {
            output.WriteBytes(ApplicationContextName);
        }
    }
}
