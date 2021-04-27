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

        private void ReadRelatedGeneralSOPClassUIDs(BinaryDataReader dataReader)
        {
            while (!dataReader.Input.AtEnd)
            {
                var relatedGeneralSOPClassUID = Read16BitLengthPrefixedAsciiString(dataReader);
                RelatedGeneralSOPClassUIDs.Add(relatedGeneralSOPClassUID);
            }
        }

        public override void ReadContentFrom(BinaryDataReader dataReader)
        {
            SOPClassUID = Read16BitLengthPrefixedAsciiString(dataReader);
            ServiceClassUID = Read16BitLengthPrefixedAsciiString(dataReader);

            Read16BitLengthPrefixedData(dataReader, () => ReadRelatedGeneralSOPClassUIDs(dataReader));
        }

        private void WriteGeneralSOPClassUIDs(BinaryDataWriter dataWriter)
        {
            foreach (var relatedSOPClassUID in RelatedGeneralSOPClassUIDs)
            {
                Write16BitLengthPrefixedAsciiString(dataWriter, relatedSOPClassUID);
            }
        }

        public override void WriteContentTo(BinaryDataWriter dataWriter)
        {
            Write16BitLengthPrefixedAsciiString(dataWriter, SOPClassUID);
            Write16BitLengthPrefixedAsciiString(dataWriter, ServiceClassUID);
            
            WriteLengthPrefixedData(dataWriter, Write16BitLength, WriteGeneralSOPClassUIDs);
        }
    }
}
