// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;
using System;
using System.IO;

namespace MDSDK.Dicom.Networking.Net
{
    internal class FragmentHeader
    {
        public uint Length { get; set; }

        public byte PresentationContextID { get; set; }

        public byte MessageControlHeader { get; set; }

        public const int Size = 6;

        private const byte FragmentTypeBit = 0x01;
        private const byte IsLastFragmentBit = 0x02;

        public FragmentType FragmentType
        {
            get => ((MessageControlHeader & FragmentTypeBit) != 0) ? FragmentType.Command: FragmentType.Dataset;
            set
            {
                if (value != FragmentType)
                {
                    MessageControlHeader ^= FragmentTypeBit;
                }
            }
        }

        public bool IsLastFragment
        {
            get => (MessageControlHeader & IsLastFragmentBit) != 0;
            set
            {
                if (value != IsLastFragment)
                {
                    MessageControlHeader ^= IsLastFragmentBit;
                }
            }
        }

        public static FragmentHeader ReadFrom(BinaryStreamReader input)
        {
            var length = input.Read<UInt32>();
            if (length < 2)
            {
                throw new IOException("Invalid DICOM PDV length");
            }
            return new FragmentHeader
            {
                Length = length,
                PresentationContextID = input.ReadByte(),
                MessageControlHeader = input.ReadByte()
            };
        }

        public void WriteTo(BinaryStreamWriter output)
        {
            output.Write<UInt32>(Length);
            output.WriteByte(PresentationContextID);
            output.WriteByte(MessageControlHeader);
        }
    }
}
