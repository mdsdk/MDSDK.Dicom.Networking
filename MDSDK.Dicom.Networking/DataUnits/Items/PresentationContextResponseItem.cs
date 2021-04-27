// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;
using MDSDK.Dicom.Networking.DataUnits.SubItems;

namespace MDSDK.Dicom.Networking.DataUnits.Items
{
    class PresentationContextResponseItem : Item
    {
        public PresentationContextResponseItem()
            : base(DataUnitType.PresentationContextItemResponse)
        {
        }

        public byte PresentationContextID { get; set; }

        public byte Result { get; set; }

        public TransferSyntaxSubItem TransferSyntaxSubItem { get; set; }

        public override void ReadContentFrom(BinaryDataReader dataReader)
        {
            PresentationContextID = dataReader.ReadByte();
            dataReader.Input.SkipBytes(1);
            Result = dataReader.ReadByte();
            dataReader.Input.SkipBytes(1);

            if (Result == 0)
            {
                TransferSyntaxSubItem = (TransferSyntaxSubItem)SubItem.ReadFrom(dataReader);
            }
            
            dataReader.Input.SkipRemainingBytes();
        }

        public override void WriteContentTo(BinaryDataWriter dataWriter)
        {
            dataWriter.Write(PresentationContextID);
            dataWriter.Write((byte)0);
            dataWriter.Write(Result);
            dataWriter.Write((byte)0);

            if (Result == 0)
            {
                TransferSyntaxSubItem.WriteTo(dataWriter);
            }
        }
    }
}
