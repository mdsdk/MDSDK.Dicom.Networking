// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Serialization.TransferSyntaxes;

namespace MDSDK.Dicom.Networking
{
    public class DicomPresentationContext
    {
        public byte PresentationContextID { get; }

        public TransferSyntax TransferSyntax { get; }

        public DicomPresentationContext(byte presentationContextID, TransferSyntax transferSyntax)
        {
            PresentationContextID = presentationContextID;
            TransferSyntax = transferSyntax;
        }
    }
}
