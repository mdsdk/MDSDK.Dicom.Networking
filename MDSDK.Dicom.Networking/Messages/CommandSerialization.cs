﻿// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;
using MDSDK.Dicom.Serialization;
using System;
using System.IO;
using System.Reflection;

namespace MDSDK.Dicom.Networking.Messages
{
    internal static class CommandSerialization 
    {
        private static readonly DicomSerializer<CommandHeader> CommandHeaderSerializer = DicomSerializer.GetSerializer<CommandHeader>();

        internal static CommandType GetCommandType<TCommand>() where TCommand : ICommand
        {
            return typeof(TCommand).GetCustomAttribute<CommandAttribute>().CommandType;
        }

        internal static DicomSerializer GetSerializer(CommandType commandType)
        {
            return commandType switch
            {
                CommandType.C_STORE_RQ => DicomSerializer.GetSerializer<CStoreRequest>(),
                CommandType.C_STORE_RSP => DicomSerializer.GetSerializer<CStoreResponse>(),
                CommandType.C_GET_RQ => DicomSerializer.GetSerializer<CGetRequest>(),
                CommandType.C_GET_RSP => DicomSerializer.GetSerializer<CGetResponse>(),
                CommandType.C_FIND_RQ => DicomSerializer.GetSerializer<CFindRequest>(),
                CommandType.C_FIND_RSP => DicomSerializer.GetSerializer<CFindResponse>(),
                CommandType.C_MOVE_RQ => DicomSerializer.GetSerializer<CMoveRequest>(),
                CommandType.C_MOVE_RSP => DicomSerializer.GetSerializer<CMoveResponse>(),
                CommandType.C_ECHO_RQ => DicomSerializer.GetSerializer<CEchoRequest>(),
                CommandType.C_ECHO_RSP => DicomSerializer.GetSerializer<CEchoResponse>(),
                CommandType.N_EVENT_REPORT_RQ => DicomSerializer.GetSerializer<NEventReportRequest>(),
                CommandType.N_EVENT_REPORT_RSP => DicomSerializer.GetSerializer<NEventReportResponse>(),
                CommandType.N_GET_RQ => DicomSerializer.GetSerializer<NGetRequest>(),
                CommandType.N_GET_RSP => DicomSerializer.GetSerializer<NGetResponse>(),
                CommandType.N_SET_RQ => DicomSerializer.GetSerializer<NSetRequest>(),
                CommandType.N_SET_RSP => DicomSerializer.GetSerializer<NSetResponse>(),
                CommandType.N_ACTION_RQ => DicomSerializer.GetSerializer<NActionRequest>(),
                CommandType.N_ACTION_RSP => DicomSerializer.GetSerializer<NActionResponse>(),
                CommandType.N_CREATE_RQ => DicomSerializer.GetSerializer<NCreateRequest>(),
                CommandType.N_CREATE_RSP => DicomSerializer.GetSerializer<NCreateResponse>(),
                CommandType.N_DELETE_RQ => DicomSerializer.GetSerializer<NDeleteRequest>(),
                CommandType.N_DELETE_RSP => DicomSerializer.GetSerializer<NDeleteResponse>(),
                CommandType.C_CANCEL_RQ => DicomSerializer.GetSerializer<CCancelRequest>(),
                _ => throw new NotSupportedException(),
            };
        }

        public static ICommand ReadFrom(Stream stream)
        {
            var input = new BinaryStreamReader(ByteOrder.LittleEndian, stream);

            var commandGroupLengthTag = input.Read<UInt32>();
            var commandGroupLengthLength = input.Read<UInt32>();

            if ((commandGroupLengthTag != 0) || (commandGroupLengthLength != 4))
            {
                throw new IOException($"Expected command group length tag/4 but got {commandGroupLengthTag:X8}/{commandGroupLengthLength}");
            }

            var commandGroupLength = input.Read<UInt32>();

            var data = new byte[commandGroupLength];
            input.ReadAll(data);

            input = new BinaryStreamReader(ByteOrder.LittleEndian, data);

            var commandHeaderReader = new DicomStreamReader(DicomVRCoding.Implicit, input);
            var commandHeader = CommandHeaderSerializer.Deserialize(commandHeaderReader);
            var commandSerializer = GetSerializer(commandHeader.CommandField);

            input = new BinaryStreamReader(ByteOrder.LittleEndian, data);
            var commandReader = new DicomStreamReader(DicomVRCoding.Implicit, input);
            return (ICommand)commandSerializer.Deserialize(commandReader);
        }

        public static void WriteTo(Stream stream, ICommand command)
        {
            var serializer = GetSerializer(command.CommandField);

            if (!serializer.TryGetSerializedLength(command, DicomVRCoding.Implicit, out uint commandGroupLength))
            {
                throw new Exception($"Could not calculate command group length for {command.CommandField}");
            }

            var output = new BinaryStreamWriter(ByteOrder.LittleEndian, stream);

            output.Write<UInt32>(0);
            output.Write<UInt32>(4);
            output.Write<UInt32>(commandGroupLength);

            var writer = new DicomStreamWriter(DicomVRCoding.Implicit, output);
            serializer.Serialize(writer, command);

            output.Flush(FlushMode.Shallow);
        }
    }
}