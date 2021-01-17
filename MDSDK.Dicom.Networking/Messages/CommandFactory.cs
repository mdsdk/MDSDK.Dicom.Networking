// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using System.Reflection;

namespace MDSDK.Dicom.Networking.Messages
{
    static class CommandFactory
    {
        public static TCommand CreateCommand<TCommand>() where TCommand : Command, new()
        {
            const ushort NoDataSet = 0x0101;
            const ushort DataSet = 0xFEFE;

            var commandAttribute = typeof(TCommand).GetCustomAttribute<CommandAttribute>();
            var command = new TCommand();
            command.CommandField = (ushort)commandAttribute.CommandType;
            command.CommandDataSetType = commandAttribute.HasDataSet ? DataSet : NoDataSet;
            return command;
        }
    }
}
