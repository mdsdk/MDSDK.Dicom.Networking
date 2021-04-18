// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Serialization;

namespace MDSDK.Dicom.Networking
{
    public class DicomPresentationContext
    {
        public byte PresentationContextID { get; }

        public DicomTransferSyntax TransferSyntax { get; }

        public DicomPresentationContext(byte presentationContextID, DicomTransferSyntax transferSyntax)
        {
            PresentationContextID = presentationContextID;
            TransferSyntax = transferSyntax;
        }
    }
}
