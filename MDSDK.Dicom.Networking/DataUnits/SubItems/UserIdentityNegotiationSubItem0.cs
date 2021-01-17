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

        public override void ReadContentFrom(BinaryStreamReader input)
        {
            UserIdentityType = input.ReadByte();
            PositiveResponseRequested = input.ReadByte();
            PrimaryField = Read16BitLengthPrefixedByteArray(input);
            SecondaryField = Read16BitLengthPrefixedByteArray(input);
        }

        public override void WriteContentTo(BinaryStreamWriter output)
        {
            output.WriteByte(UserIdentityType);
            output.WriteByte(PositiveResponseRequested);
            Write16BitLengthPrefixedByteArray(output, PrimaryField);
            Write16BitLengthPrefixedByteArray(output, SecondaryField);
        }
    }
}
