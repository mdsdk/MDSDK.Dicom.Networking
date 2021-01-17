// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    public class DicomMessage<TCommand>
        where TCommand : Command, new()
    {
        public TCommand Command { get; } = CommandFactory.CreateCommand<TCommand>();
    }

    public class DicomMessage<TCommand, TData>
        where TCommand : Command, new()
        where TData : new()
    {
        public TCommand Command { get; } = CommandFactory.CreateCommand<TCommand>();
        public TData Data { get; } = new TData();
    }
}
