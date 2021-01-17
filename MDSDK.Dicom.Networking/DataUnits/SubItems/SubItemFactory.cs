// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using System;

namespace MDSDK.Dicom.Networking.DataUnits.SubItems
{
    class SubItemFactory : DataUnitFactory<SubItem>
    {
        public static SubItemFactory Instance { get; } = new SubItemFactory();

        public override SubItem Create(DataUnitType subItemType)
        {
            switch (subItemType)
            {
            case DataUnitType.AbstractSyntaxSubItem:
                return new AbstractSyntaxSubItem();
            case DataUnitType.TransferSyntaxSubItem:
                return new TransferSyntaxSubItem();
            case DataUnitType.MaximumLengthSubItem:
                return new MaximumLengthSubItem();
            case DataUnitType.ImplementationClassUIDSubItem:
                return new ImplementationClassUIDSubItem();
            case DataUnitType.AsynchronousOperationsWindowSubItem:
                return new AsynchronousOperationsWindowSubItem();
            case DataUnitType.SCPSCURoleSelectionSubItem:
                return new SCPSCURoleSelectionSubItem();
            case DataUnitType.ImplementationVersionNameSubItem:
                return new ImplementationVersionNameSubItem();
            case DataUnitType.SOPClassExtendedNegotiationSubItem:
                return new SOPClassExtendedNegotiationSubItem();
            case DataUnitType.UserIdentityNegotiationSubItem0:
                return new UserIdentityNegotiationSubItem0();
            case DataUnitType.UserIdentityNegotiationSubItem1:
                return new UserIdentityNegotiationSubItem1();
            }

            if (SubItem.IsUserInformationSubItemType(subItemType))
            {
                return new UnrecognizedUserInformationSubItem(subItemType);
            }
            else
            {
                throw new NotSupportedException($"Expected SubItem but got {subItemType}");
            }
        }
    }
}
