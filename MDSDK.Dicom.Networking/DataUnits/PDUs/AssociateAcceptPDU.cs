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

        public override void ReadContentFrom(BinaryDataReader dataReader)
        {
            var protocolVersion = dataReader.Read<UInt16>();

            if ((protocolVersion & 1) == 0)
            {
                throw new NotSupportedException($"Unsupported protocol version {protocolVersion}");
            }

            dataReader.Input.SkipBytes(2);
            dataReader.Read(CopiedFromAssociateRequestPDU_1);
            dataReader.Read(CopiedFromAssociateRequestPDU_2);
            dataReader.Read(CopiedFromAssociateRequestPDU_3);

            Item.ReadItems(dataReader, Items);
        }
        
        public override void WriteContentTo(BinaryDataWriter dataWriter)
        {
            dataWriter.Write<UInt16>(ProtocolVersion);

            dataWriter.WriteZeros(2);
            dataWriter.Write(CopiedFromAssociateRequestPDU_1);
            dataWriter.Write(CopiedFromAssociateRequestPDU_2);
            dataWriter.Write(CopiedFromAssociateRequestPDU_3);

            WriteDataUnits(dataWriter, Items);
        }
    }
}
