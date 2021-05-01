// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    /// <summary>Extension methods for ICommand classes</summary>
    public static class CommandExtensionMethods
    {
        internal static bool IsDataSetRequired(this IMayHaveDataSet command)
        {
            return (command is CStoreRequest)
                || (command is CFindRequest)
                || (command is CGetRequest)
                || (command is CMoveRequest);
        }

        /// <summary>Returns whether a received command is followed by a data set</summary>
        public static bool IsFollowedByDataSet(this ICommand command) => command.CommandDataSetType != 0x0101;

        /// <summary>Returns whether a received response indicates that the associated operation was successfully completed</summary>
        public static bool StatusIsSuccess(this IResponse response) => response.Status == 0x0000;

        /// <summary>Returns whether a received response indicates that the associated operation was cancelled</summary>
        public static bool StatusIsCancel(this IResponse response) => response.Status == 0xFE00;

        /// <summary>Returns whether a received response indicates that the associated operation is ongoing</summary>
        public static bool StatusIsPending(this IResponse response) => (response.Status == 0xFF00) || (response.Status == 0xFF01);
    }
}
