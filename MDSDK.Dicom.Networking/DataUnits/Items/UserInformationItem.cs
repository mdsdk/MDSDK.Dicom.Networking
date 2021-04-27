// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;
using MDSDK.Dicom.Networking.DataUnits.SubItems;
using System.Collections.Generic;

namespace MDSDK.Dicom.Networking.DataUnits.Items
{
    class UserInformationItem : Item
    {
        public UserInformationItem()
            : base(DataUnitType.UserInformationItem)
        {
        }

        public List<SubItem> SubItems { get; } = new List<SubItem>();

        public override void ReadContentFrom(BinaryDataReader dataReader)
        {
            SubItem.ReadSubItems(dataReader, SubItems);
        }

        public override void WriteContentTo(BinaryDataWriter dataWriter)
        {
            WriteDataUnits(dataWriter, SubItems);
        }
    }
}
