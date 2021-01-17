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

        public override void ReadContentFrom(BinaryStreamReader input)
        {
            ServerResponse = Read16BitLengthPrefixedByteArray(input);
        }

        public override void WriteContentTo(BinaryStreamWriter output)
        {
            Write16BitLengthPrefixedByteArray(output, ServerResponse);
        }
    }
}
