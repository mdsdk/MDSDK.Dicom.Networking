// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;
using MDSDK.Dicom.Networking.DataUnits.SubItems;
using System.Collections.Generic;

namespace MDSDK.Dicom.Networking.DataUnits.Items
{
    class PresentationContextRequestItem : Item
    {
        public PresentationContextRequestItem()
            : base(DataUnitType.PresentationContextItemRequest) 
        {
        }

        public byte PresentationContextID { get; set; }

        public List<SubItem> SubItems { get; } = new List<SubItem>();

        public override void ReadContentFrom(BinaryStreamReader input)
        {
            PresentationContextID = input.ReadByte();
            input.SkipBytes(3);
            SubItem.ReadSubItems(input, SubItems);
        }

        public override void WriteContentTo(BinaryStreamWriter output)
        {
            output.WriteByte(PresentationContextID);
            output.WriteZeros(3);
            WriteDataUnits(output, SubItems);
        }
    }
}
