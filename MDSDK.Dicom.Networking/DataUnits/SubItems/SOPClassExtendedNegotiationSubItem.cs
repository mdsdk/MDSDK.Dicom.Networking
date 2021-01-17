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

        public override void ReadContentFrom(BinaryStreamReader input)
        {
            SOPClassUID = Read16BitLengthPrefixedAsciiString(input);
            ServiceClassApplictionInformation = input.ReadRemainingBytes();
        }

        public override void WriteContentTo(BinaryStreamWriter output)
        {
            Write16BitLengthPrefixedAsciiString(output, SOPClassUID);
            output.WriteBytes(ServiceClassApplictionInformation);
        }
    }
}
