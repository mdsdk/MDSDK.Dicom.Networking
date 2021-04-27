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

        public override void ReadContentFrom(BinaryDataReader dataReader)
        {
            PresentationContextID = dataReader.ReadByte();
            dataReader.Input.SkipBytes(3);
            SubItem.ReadSubItems(dataReader, SubItems);
        }

        public override void WriteContentTo(BinaryDataWriter dataWriter)
        {
            dataWriter.Write(PresentationContextID);
            dataWriter.WriteZeros(3);
            WriteDataUnits(dataWriter, SubItems);
        }
    }
}
