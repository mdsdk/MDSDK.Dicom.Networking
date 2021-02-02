// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    public static class CommandExtenstionMethods
    {
        public static bool HasDataSet(this ICommand command) => command.CommandDataSetType != 0x0101;

        public static bool StatusIsSuccess(this IResponse response) => response.Status == 0x0000;
        
        public static bool StatusIsCancel(this IResponse response) => response.Status == 0xFE00;

        public static bool StatusIsPending(this IResponse response) => (response.Status == 0xFF00) || (response.Status == 0xFF01);
    }
}
