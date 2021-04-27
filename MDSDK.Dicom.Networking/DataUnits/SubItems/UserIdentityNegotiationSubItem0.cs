// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;

namespace MDSDK.Dicom.Networking.DataUnits.SubItems
{
    class UserIdentityNegotiationSubItem0 : SubItem
    {
        public UserIdentityNegotiationSubItem0()
            : base(DataUnitType.UserIdentityNegotiationSubItem0)
        {
        }

        public byte UserIdentityType { get; set; }
        
        public byte PositiveResponseRequested { get; set; }

        public byte[] PrimaryField { get; set; }

        public byte[] SecondaryField { get; set; }

        public override void ReadContentFrom(BinaryDataReader dataReader)
        {
            UserIdentityType = dataReader.ReadByte();
            PositiveResponseRequested = dataReader.ReadByte();
            PrimaryField = Read16BitLengthPrefixedByteArray(dataReader);
            SecondaryField = Read16BitLengthPrefixedByteArray(dataReader);
        }

        public override void WriteContentTo(BinaryDataWriter dataWriter)
        {
            dataWriter.Write(UserIdentityType);
            dataWriter.Write(PositiveResponseRequested);
            Write16BitLengthPrefixedByteArray(dataWriter, PrimaryField);
            Write16BitLengthPrefixedByteArray(dataWriter, SecondaryField);
        }
    }
}
