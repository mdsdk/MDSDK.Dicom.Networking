// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;

namespace MDSDK.Dicom.Networking.DataUnits.SubItems
{
    class TransferSyntaxSubItem : SubItem
    {
        public TransferSyntaxSubItem()
            : base(DataUnitType.TransferSyntaxSubItem)
        {
        }

        public string TransferSyntaxName { get; set; }

        public override void ReadContentFrom(BinaryDataReader dataReader)
        {
            TransferSyntaxName = ReadNonLengthPrefixedAsciiString(dataReader);
        }

        public override void WriteContentTo(BinaryDataWriter dataWriter)
        {
            WriteNonLengthPrefixedAsciiString(dataWriter, TransferSyntaxName);
        }
    }
}
