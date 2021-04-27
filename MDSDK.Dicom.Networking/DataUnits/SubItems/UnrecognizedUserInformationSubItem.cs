// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;
using System;
using System.Diagnostics;

namespace MDSDK.Dicom.Networking.DataUnits.SubItems
{
    class UnrecognizedUserInformationSubItem : SubItem
    {
        public UnrecognizedUserInformationSubItem(DataUnitType subItemType)
            : base(subItemType)
        {
            Debug.Assert(IsUserInformationSubItemType(subItemType));
        }

        public override void ReadContentFrom(BinaryDataReader dataReader)
        {
            dataReader.Input.SkipRemainingBytes();
        }

        public override void WriteContentTo(BinaryDataWriter dataWriter)
        {
            Environment.FailFast("Attempt to send unrecognized user information sub-item");
        }
    }
}
