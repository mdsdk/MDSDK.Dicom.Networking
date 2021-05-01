// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Networking.Examples.Store;
using MDSDK.Dicom.Networking.Messages;
using MDSDK.Dicom.Serialization;
using System;
using System.Collections.Generic;

namespace MDSDK.Dicom.Networking.Examples.QueryRetrieve
{
    public class CGetSCU
    {
        public byte PresentationContextID { get; }

        private readonly Dictionary<byte, CStoreSCP> _cStoreSCPS = new();

        public CGetSCU(DicomClient client, params DicomUID[] storageSOPClassUIDs)
        {
            PresentationContextID = client.ProposePresentationContext(DicomUID.SOPClass.PatientRootQueryRetrieveInformationModelGet,
                DicomUID.TransferSyntax.ImplicitVRLittleEndian);

            foreach (var storageSOPClassUID in storageSOPClassUIDs)
            {
                var cStoreSCP = new CStoreSCP(client, storageSOPClassUID);
                _cStoreSCPS.Add(cStoreSCP.PresentationContextID, cStoreSCP);
            }
        }

        public void Download(DicomAssociation association, SOPInstanceIdentifier query)
        {
            var cGetRequest = new CGetRequest
            {
                AffectedSOPClassUID = DicomUID.SOPClass.PatientRootQueryRetrieveInformationModelGet,
                Priority = RequestPriority.Medium
            };

            association.SendRequest(PresentationContextID, cGetRequest, query);
            association.ReceiveCommands(HandleCommand);
        }

        private void HandleCommand(DicomAssociation association, byte presentationContextID, DicomUID transferSyntaxUID, 
            ICommand command, out bool stop)
        {
            if (command is CStoreRequest cStoreRequest)
            {
                if (_cStoreSCPS.TryGetValue(presentationContextID, out CStoreSCP cStoreSCP))
                {
                    cStoreSCP.HandleCStoreRequest(association, cStoreRequest);
                    stop = false;
                }
                else
                {
                    throw new Exception($"Unexpected {cStoreRequest} received in presentation context {presentationContextID}");
                }
            }
            else if (command is CGetResponse cGetResponse)
            {
                if (presentationContextID != PresentationContextID)
                {
                    throw new Exception($"Unexpected {cGetResponse} received in presentation context {presentationContextID}");
                }
                if (cGetResponse.IsFollowedByDataSet())
                {
                    association.ReceiveDataSet(cGetResponse, PresentationContextID, stream => { });
                }
                stop = !cGetResponse.StatusIsPending();
            }
            else
            {
                throw new Exception($"Unexpected {command} received in presentation context {presentationContextID}");
            }
        }
    }
}
