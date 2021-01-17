// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using System;

namespace MDSDK.Dicom.Networking.DataUnits.Items
{
    class ItemFactory : DataUnitFactory<Item>
    {
        public static ItemFactory Instance { get; } = new ItemFactory();

        public override Item Create(DataUnitType dataUnitType)
        {
            switch (dataUnitType)
            {
            case DataUnitType.ApplicationContextItem:
                return new ApplicationContextItem();
            case DataUnitType.PresentationContextItemRequest:
                return new PresentationContextRequestItem();
            case DataUnitType.PresentationContextItemResponse:
                return new PresentationContextResponseItem();
            case DataUnitType.UserInformationItem:
                return new UserInformationItem();
            default:
                throw new NotSupportedException($"Expected Item but got {dataUnitType}");
            }
        }
    }
}
