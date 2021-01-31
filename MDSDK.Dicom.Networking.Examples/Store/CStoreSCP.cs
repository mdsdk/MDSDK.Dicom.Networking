// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Networking.Messages;
using MDSDK.Dicom.Serialization;

namespace MDSDK.Dicom.Networking.Examples.Store
{
    public class CStoreSCP
    {
        public byte PresentationContextID { get; }

        public DicomUID StorageSOPClassUID { get; }

        public CStoreSCP(DicomClient client, DicomUID storageSopClassUID)
        {
            PresentationContextID = client.ProposePresentationContext(storageSopClassUID, DicomTransferSyntax.ImplicitVRLittleEndian);
            StorageSOPClassUID = storageSopClassUID;
        }

        public void HandleCStoreRequest(DicomAssociation association, CStoreRequest cStoreRequest)
        {
            association.ReceiveDataSet(PresentationContextID, stream => { });

            var cStoreResponse = new CStoreResponse
            {
                AffectedSOPClassUID = cStoreRequest.AffectedSOPClassUID,
                AffectedSOPInstanceUID = cStoreRequest.AffectedSOPInstanceUID,
                Status = 0x0000
            };

            association.SendResponse(PresentationContextID, cStoreResponse, cStoreRequest.MessageID, CommandIsFollowedByDataSet.No);
        }
    }
}
