// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Serialization;

namespace MDSDK.Dicom.Networking
{
    public class DicomPresentationContext
    {
        public byte PresentationContextID { get; }

        public DicomUID TransferSyntaxUID { get; }

        public DicomPresentationContext(byte presentationContextID, DicomUID transferSyntaxUID)
        {
            PresentationContextID = presentationContextID;
            TransferSyntaxUID = transferSyntaxUID;
        }
    }
}
