// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;

namespace MDSDK.Dicom.Networking.DataUnits.SubItems
{
    class SOPClassExtendedNegotiationSubItem : SubItem
    {
        public SOPClassExtendedNegotiationSubItem()
            : base(DataUnitType.SOPClassExtendedNegotiationSubItem)
        {
        }

        public string SOPClassUID { get; set; }

        public byte[] ServiceClassApplictionInformation { get; set; }

        public override void ReadContentFrom(BinaryDataReader dataReader)
        {
            SOPClassUID = Read16BitLengthPrefixedAsciiString(dataReader);
            ServiceClassApplictionInformation = dataReader.Input.ReadRemainingBytes();
        }

        public override void WriteContentTo(BinaryDataWriter dataWriter)
        {
            Write16BitLengthPrefixedAsciiString(dataWriter, SOPClassUID);
            dataWriter.Output.WriteBytes(ServiceClassApplictionInformation);
        }
    }
}
