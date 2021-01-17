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

        public override void ReadContentFrom(BinaryStreamReader input)
        {
            PresentationContextID = input.ReadByte();
            input.SkipBytes(1);
            Result = input.ReadByte();
            input.SkipBytes(1);

            if (Result == 0)
            {
                TransferSyntaxSubItem = (TransferSyntaxSubItem)SubItem.ReadFrom(input);
            }
            
            input.SkipRemainingBytes();
        }

        public override void WriteContentTo(BinaryStreamWriter output)
        {
            output.WriteByte(PresentationContextID);
            output.WriteZeros(1);
            output.WriteByte(Result);
            output.WriteZeros(1);

            if (Result == 0)
            {
                TransferSyntaxSubItem.WriteTo(output);
            }
        }
    }
}
