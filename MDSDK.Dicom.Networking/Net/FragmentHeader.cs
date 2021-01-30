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

        public MessageControlHeader MessageControlHeader { get; set; }

        public const int Size = 6;

        public FragmentType FragmentType
        {
            get => MessageControlHeader.HasFlag(MessageControlHeader.IsCommand) ? FragmentType.Command: FragmentType.Dataset;
            set
            {
                if (value != FragmentType)
                {
                    MessageControlHeader ^= MessageControlHeader.IsCommand;
                }
            }
        }

        public bool IsLastFragment
        {
            get => MessageControlHeader.HasFlag(MessageControlHeader.IsLastFragment);
            set
            {
                if (value != IsLastFragment)
                {
                    MessageControlHeader ^= MessageControlHeader.IsLastFragment;
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
                MessageControlHeader = (MessageControlHeader)input.ReadByte()
            };
        }

        public void WriteTo(BinaryStreamWriter output)
        {
            output.Write<UInt32>(Length);
            output.WriteByte(PresentationContextID);
            output.WriteByte((byte)MessageControlHeader);
        }
    }
}
