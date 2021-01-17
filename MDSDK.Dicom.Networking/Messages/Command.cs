// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;
using MDSDK.Dicom.Serialization;
using System;
using System.IO;
using System.Reflection;

namespace MDSDK.Dicom.Networking.Messages
{
    public abstract class Command
    {
        public ushort CommandField { get; set; }

        public ushort CommandDataSetType { get; set; }

        internal Command() { }

        public static T ReadFrom<T>(Stream stream) where T : Command, new()
        {
            var serializer = DicomSerializer.GetSerializer<T>();
            
            var input = new BinaryStreamReader(stream, ByteOrder.LittleEndian);

            var commandGroupLengthTag = input.Read<UInt32>();
            var commandGroupLengthLength = input.Read<UInt32>();
            if ((commandGroupLengthTag != 0) || (commandGroupLengthLength != 4))
            {
                throw new IOException($"Expected command group length tag/4 but got {commandGroupLengthTag:X8}/{commandGroupLengthLength}");
            }

            var commandGroupLength = input.Read<UInt32>();
            
            return input.Read(commandGroupLength, () =>
            {
                var commandReader = new DicomStreamReader(input, DicomVRCoding.Implicit);
                return serializer.Deserialize(commandReader);
            });
        }

        public static T Create<T>() where T : Command, new()
        {
            var commandAttribute = typeof(T).GetCustomAttribute<CommandAttribute>();
            return new T
            {
                CommandField = (ushort)commandAttribute.CommandType,
                CommandDataSetType = commandAttribute.HasDataSet ? 0x0000 : 0x0101
            };
        }

        public static void WriteTo<T>(Stream stream, T command) where T : Command, new()
        {
            var serializer = DicomSerializer.GetSerializer<T>();
            if (!serializer.TryGetSerializedLength(command, DicomVRCoding.Implicit, out uint commandGroupLength))
            {
                throw new Exception($"Could not calculate command group length for {typeof(T)}");
            }

            var output = new BinaryStreamWriter(stream, ByteOrder.LittleEndian);
            
            output.Write<UInt32>(0);
            output.Write<UInt32>(4);
            output.Write<UInt32>(commandGroupLength);

            var writer = new DicomStreamWriter(output, DicomVRCoding.Implicit);
            serializer.Serialize(command, writer);

            output.Flush(FlushMode.Shallow);
        }
    }
}
