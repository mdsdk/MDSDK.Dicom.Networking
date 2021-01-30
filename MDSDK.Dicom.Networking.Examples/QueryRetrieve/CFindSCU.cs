// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Networking.Messages;
using MDSDK.Dicom.Networking.SCUs;
using MDSDK.Dicom.Serialization;
using System;

namespace MDSDK.Dicom.Networking.Examples.QueryRetrieve
{
    public class CFindSCU
    {
        private readonly byte _presentationContextID;

        public CFindSCU(DicomClient client)
        {
            _presentationContextID = client.ProposePresentationContext(DicomUID.PatientRootQueryRetrieveInformationModelFIND,
                DicomTransferSyntax.ImplicitVRLittleEndian);
        }

        public void Execute(DicomAssociation association, string patientName)
        {
            Console.WriteLine($"Executing C-FIND (PatientName = '{patientName}')");

            var cFindRequest = new CFindRequest
            {
                AffectedSOPClassUID = DicomUID.PatientRootQueryRetrieveInformationModelFIND.UID,
                Priority = RequestPriority.Medium
            };

            var patientInfoQuery = new PatientInfoQuery
            {
                PatientName = patientName,
            };

            association.SendRequest(_presentationContextID, cFindRequest);
            association.SendDataset(_presentationContextID, patientInfoQuery);

            var cFindResponse = association.ReceiveResponse<CFindResponse>(_presentationContextID, cFindRequest.MessageID);
            while (cFindResponse.IsPending())
            {
                var pat  = association.ReceiveDataset<PatientInfo>(_presentationContextID);
                Console.WriteLine($"Found {pat.PatientID}, {pat.PatientName}, {pat.PatientBirthDate}, {pat.PatientSex}");
                cFindResponse = association.ReceiveResponse<CFindResponse>(_presentationContextID, cFindRequest.MessageID);
            }

            Console.WriteLine($"C-FIND status = {cFindResponse.Status}");
        }
    }
}
