// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;
using MDSDK.Dicom.Networking.DataUnits.Items;
using System;
using System.Collections.Generic;

namespace MDSDK.Dicom.Networking.DataUnits.PDUs
{
    class AssociateRequestPDU : PDU
    {
        public AssociateRequestPDU()
            : base(DataUnitType.AssociateRequestPDU)
        {
        }

        public ushort ProtocolVersion { get; } = 1;

        public byte[] CalledAETitle { get; } = new byte[16];

        public byte[] CallingAETitle { get; } = new byte[16];

        public byte[] Reserved { get; } = new byte[32];

        public List<Item> Items { get; } = new List<Item>();

        public override void ReadContentFrom(BinaryStreamReader input)
        {
            var protocolVersion = input.Read<UInt16>();
            
            if ((protocolVersion & 1) == 0)
            {
                throw new NotSupportedException($"Unsupported protocol version {protocolVersion}");
            }

            input.SkipBytes(2);
            input.ReadAll(CalledAETitle);
            input.ReadAll(CallingAETitle);
            input.ReadAll(Reserved);

            Item.ReadItems(input, Items);
        }

        public override void WriteContentTo(BinaryStreamWriter output)
        {
            output.Write<UInt16>(ProtocolVersion);

            output.WriteZeros(2);
            output.WriteBytes(CalledAETitle);
            output.WriteBytes(CallingAETitle);
            output.WriteBytes(Reserved);

            WriteDataUnits(output, Items);
        }
    }
}
