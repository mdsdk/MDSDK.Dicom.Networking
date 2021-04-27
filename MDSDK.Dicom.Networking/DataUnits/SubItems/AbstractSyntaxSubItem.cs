// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;

namespace MDSDK.Dicom.Networking.DataUnits.SubItems
{
    class AbstractSyntaxSubItem : SubItem
    {
        public AbstractSyntaxSubItem()
            : base(DataUnitType.AbstractSyntaxSubItem)
        {
        }

        public string AbstractSyntaxName { get; set; }

        public override void ReadContentFrom(BinaryDataReader dataReader)
        {
            AbstractSyntaxName = ReadNonLengthPrefixedAsciiString(dataReader);
        }

        public override void WriteContentTo(BinaryDataWriter dataWriter)
        {
            WriteNonLengthPrefixedAsciiString(dataWriter, AbstractSyntaxName);
        }
    }
}
