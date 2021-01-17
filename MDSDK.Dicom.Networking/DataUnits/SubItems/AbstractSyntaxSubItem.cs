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

        public override void ReadContentFrom(BinaryStreamReader input)
        {
            AbstractSyntaxName = ReadNonLengthPrefixedAsciiString(input);
        }

        public override void WriteContentTo(BinaryStreamWriter output)
        {
            WriteNonLengthPrefixedAsciiString(output, AbstractSyntaxName);
        }
    }
}
