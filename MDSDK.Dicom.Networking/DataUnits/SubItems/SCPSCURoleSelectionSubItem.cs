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

        public override void ReadContentFrom(BinaryDataReader dataReader)
        {
            SOPClassUID = Read16BitLengthPrefixedAsciiString(dataReader);
            SCURole = dataReader.ReadByte();
            SCPRole = dataReader.ReadByte();
        }

        public override void WriteContentTo(BinaryDataWriter dataWriter)
        {
            Write16BitLengthPrefixedAsciiString(dataWriter, SOPClassUID);
            dataWriter.Write(SCURole);
            dataWriter.Write(SCPRole);
        }
    }
}
