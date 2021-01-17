// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;
using MDSDK.Dicom.Networking.DataUnits.Items;
using System;
using System.Collections.Generic;

namespace MDSDK.Dicom.Networking.DataUnits.PDUs
{
    class AssociateAcceptPDU : PDU
    {
        public AssociateAcceptPDU()
            : base(DataUnitType.AssociateAcceptPDU)
        {
        }

        public ushort ProtocolVersion { get; } = 1;

        public byte[] CopiedFromAssociateRequestPDU_1 { get; } = new byte[16];

        public byte[] CopiedFromAssociateRequestPDU_2 { get; } = new byte[16];

        public byte[] CopiedFromAssociateRequestPDU_3 { get; } = new byte[32];

        public List<Item> Items { get; } = new List<Item>();

        public override void ReadContentFrom(BinaryStreamReader input)
        {
            var protocolVersion = input.Read<UInt16>();

            if ((protocolVersion & 1) == 0)
            {
                throw new NotSupportedException($"Unsupported protocol version {protocolVersion}");
            }

            input.SkipBytes(2);
            input.ReadAll(CopiedFromAssociateRequestPDU_1);
            input.ReadAll(CopiedFromAssociateRequestPDU_2);
            input.ReadAll(CopiedFromAssociateRequestPDU_3);

            Item.ReadItems(input, Items);
        }
        
        public override void WriteContentTo(BinaryStreamWriter output)
        {
            output.Write<UInt16>(ProtocolVersion);

            output.WriteZeros(2);
            output.WriteBytes(CopiedFromAssociateRequestPDU_1);
            output.WriteBytes(CopiedFromAssociateRequestPDU_2);
            output.WriteBytes(CopiedFromAssociateRequestPDU_3);

            WriteDataUnits(output, Items);
        }
    }
}
