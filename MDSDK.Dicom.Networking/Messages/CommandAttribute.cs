// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using System;

namespace MDSDK.Dicom.Networking.Messages
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class CommandAttribute : Attribute
    {
        public CommandType CommandType { get; }

        public CommandAttribute(CommandType commandType)
        {
            CommandType = commandType;
        }
    }
}
