// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;

namespace MDSDK.Dicom.Networking.DataUnits.SubItems
{
    class SCPSCURoleSelectionSubItem : SubItem
    {
        public SCPSCURoleSelectionSubItem()
            : base(DataUnitType.SCPSCURoleSelectionSubItem)
        {
        }

        public string SOPClassUID { get; set; }

        public byte SCURole { get; set; }

        public byte SCPRole { get; set; }

        public override void ReadContentFrom(BinaryStreamReader input)
        {
            SOPClassUID = Read16BitLengthPrefixedAsciiString(input);
            SCURole = input.ReadByte();
            SCPRole = input.ReadByte();
        }

        public override void WriteContentTo(BinaryStreamWriter output)
        {
            Write16BitLengthPrefixedAsciiString(output, SOPClassUID);
            output.WriteByte(SCURole);
            output.WriteByte(SCPRole);
        }
    }
}
