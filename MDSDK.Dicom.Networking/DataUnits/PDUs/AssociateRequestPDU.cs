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

        public override void ReadContentFrom(BinaryDataReader dataReader)
        {
            var protocolVersion = dataReader.Read<UInt16>();
            
            if ((protocolVersion & 1) == 0)
            {
                throw new NotSupportedException($"Unsupported protocol version {protocolVersion}");
            }

            dataReader.Input.SkipBytes(2);
            dataReader.Read(CalledAETitle);
            dataReader.Read(CallingAETitle);
            dataReader.Read(Reserved);

            Item.ReadItems(dataReader, Items);
        }

        public override void WriteContentTo(BinaryDataWriter dataWriter)
        {
            dataWriter.Write<UInt16>(ProtocolVersion);

            dataWriter.WriteZeros(2);
            dataWriter.Write(CalledAETitle);
            dataWriter.Write(CallingAETitle);
            dataWriter.Write(Reserved);

            WriteDataUnits(dataWriter, Items);
        }
    }
}
