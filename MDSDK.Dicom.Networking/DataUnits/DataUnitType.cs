// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.DataUnits
{
    public enum DataUnitType
    {
        // PDU types defined in part 8
        AssociateRequestPDU = 0x01,
        AssociateAcceptPDU = 0x02,
        AssociateRejectPDU = 0x03,
        DataTransferPDU = 0x04,
        ReleaseRequestPDU = 0x05,
        ReleaseResponsePDU = 0x06,
        AbortPDU = 0x07,

        // Item and SubItem types defined in part 8
        ApplicationContextItem = 0x10,
        PresentationContextItemRequest = 0x20,
        PresentationContextItemResponse = 0x21,
        AbstractSyntaxSubItem = 0x30,
        TransferSyntaxSubItem = 0x40,
        UserInformationItem = 0x50,
        MaximumLengthSubItem = 0x51,
        
        // UserInformation SubItem types defined in part 7
        ImplementationClassUIDSubItem = 0x52,
        AsynchronousOperationsWindowSubItem = 0x53,
        SCPSCURoleSelectionSubItem = 0x54,
        ImplementationVersionNameSubItem = 0x55,
        SOPClassExtendedNegotiationSubItem = 0x56,
        SOPClassCommonExtendedNegotiationSubItem = 0x57,
        UserIdentityNegotiationSubItem0 = 0x58,
        UserIdentityNegotiationSubItem1 = 0x59,
        
        UserInformationSubItemRangeStart = 0x52,
        UserInformationSubItemRangeEnd = 0xFF,
    }
}
