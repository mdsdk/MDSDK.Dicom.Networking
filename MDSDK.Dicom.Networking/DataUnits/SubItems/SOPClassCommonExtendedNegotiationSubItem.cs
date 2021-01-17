// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;
using System.Collections.Generic;

namespace MDSDK.Dicom.Networking.DataUnits.SubItems
{
    class SOPClassCommonExtendedNegotiationSubItem : SubItem
    {
        public SOPClassCommonExtendedNegotiationSubItem()
            : base(DataUnitType.SOPClassCommonExtendedNegotiationSubItem)
        {
        }

        public string SOPClassUID { get; set; }
        
        public string ServiceClassUID { get; set; }

        public List<string> RelatedGeneralSOPClassUIDs { get; } = new List<string>();

        private void ReadRelatedGeneralSOPClassUIDs(BinaryStreamReader input)
        {
            while (!input.AtEnd)
            {
                var relatedGeneralSOPClassUID = Read16BitLengthPrefixedAsciiString(input);
                RelatedGeneralSOPClassUIDs.Add(relatedGeneralSOPClassUID);
            }
        }

        public override void ReadContentFrom(BinaryStreamReader input)
        {
            SOPClassUID = Read16BitLengthPrefixedAsciiString(input);
            ServiceClassUID = Read16BitLengthPrefixedAsciiString(input);

            Read16BitLengthPrefixedData(input, () => ReadRelatedGeneralSOPClassUIDs(input));
        }

        private void WriteGeneralSOPClassUIDs(BinaryStreamWriter output)
        {
            foreach (var relatedSOPClassUID in RelatedGeneralSOPClassUIDs)
            {
                Write16BitLengthPrefixedAsciiString(output, relatedSOPClassUID);
            }
        }

        public override void WriteContentTo(BinaryStreamWriter output)
        {
            Write16BitLengthPrefixedAsciiString(output, SOPClassUID);
            Write16BitLengthPrefixedAsciiString(output, ServiceClassUID);
            
            WriteLengthPrefixedData(output, Write16BitLength, WriteGeneralSOPClassUIDs);
        }
    }
}
