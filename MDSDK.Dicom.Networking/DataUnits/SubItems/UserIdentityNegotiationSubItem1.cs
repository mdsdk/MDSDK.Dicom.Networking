// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;

namespace MDSDK.Dicom.Networking.DataUnits.SubItems
{
    class UserIdentityNegotiationSubItem1 : SubItem
    {
        public UserIdentityNegotiationSubItem1()
            : base(DataUnitType.UserIdentityNegotiationSubItem1)
        {
        }

        public byte[] ServerResponse { get; set; }

        public override void ReadContentFrom(BinaryDataReader dataReader)
        {
            ServerResponse = Read16BitLengthPrefixedByteArray(dataReader);
        }

        public override void WriteContentTo(BinaryDataWriter dataWriter)
        {
            Write16BitLengthPrefixedByteArray(dataWriter, ServerResponse);
        }
    }
}
