// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Networking.Messages;
using MDSDK.Dicom.Networking.SCUs;
using MDSDK.Dicom.Serialization;
using System;
using System.Collections.Generic;

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

        private IReadOnlyList<TInfo> PerformCFind<TQuery, TInfo>(DicomAssociation association, TQuery query) 
            where TInfo : new()
        {
            var cFindRequest = new CFindRequest
            {
                AffectedSOPClassUID = DicomUID.PatientRootQueryRetrieveInformationModelFIND.UID,
                Priority = RequestPriority.Medium
            };

            association.SendRequest(_presentationContextID, cFindRequest);
            association.SendDataset(_presentationContextID, query);

            var infoList = new List<TInfo>();

            var cFindResponse = association.ReceiveResponse<CFindResponse>(_presentationContextID, cFindRequest.MessageID);
            while (cFindResponse.IsPending())
            {
                var info = association.ReceiveDataset<TInfo>(_presentationContextID);
                infoList.Add(info);
                cFindResponse = association.ReceiveResponse<CFindResponse>(_presentationContextID, cFindRequest.MessageID);
            }

            if (!cFindResponse.IsSuccess())
            {
                throw new Exception($"C-FIND SCP returned {cFindResponse.Status}");
            }

            return infoList;
        }

        private void GetSOPInstances(DicomAssociation association, PatientInfo patient, StudyInfo study, SeriesInfo series)
        {
            var query = new SOPInstanceQuery(patient.PatientID, study.StudyInstanceUID, series.SeriesInstanceUID);
            var sopInstanceList = PerformCFind<SOPInstanceQuery, SOPInstanceInfo>(association, query);
            foreach (var sopInstance in sopInstanceList)
            {
                if (DicomUID.TryLookup(sopInstance.SOPClassUID, out DicomUID knownSOPClassUID))
                {
                    sopInstance.SOPClassUID = knownSOPClassUID.Name;
                }
                Console.WriteLine($">>> SOP Instance: {sopInstance.SOPClassUID} ({sopInstance.SOPInstanceUID})");
            }
        }

        private void GetSeries(DicomAssociation association, PatientInfo patient, StudyInfo study)
        {
            var query = new SeriesQuery(patient.PatientID, study.StudyInstanceUID);
            var seriesList = PerformCFind<SeriesQuery, SeriesInfo>(association, query);
            foreach (var series in seriesList)
            {
                Console.WriteLine($">> Series: {series.Modality} {series.SeriesTime} '{series.SeriesDescription}' {series.NumberOfSeriesRelatedInstances}");
                GetSOPInstances(association, patient, study, series);
            }
        }

        private void GetStudies(DicomAssociation association, PatientInfo patient)
        {
            var query = new StudyQuery(patient.PatientID);
            var studyList = PerformCFind<StudyQuery, StudyInfo>(association, query);
            foreach (var study in studyList)
            {
                Console.WriteLine($"> Study: {study.StudyDate} '{study.StudyDescription}'");
                GetSeries(association, patient, study);
            }
        }

        public void GetPatients(DicomAssociation association, string patientName)
        {
            var query = new PatientQuery
            {
                PatientName = patientName,
            };

            var patientList = PerformCFind<PatientQuery, PatientInfo>(association, query);
            foreach (var patient in patientList)
            {
                Console.WriteLine($"Patient: {patient.PatientID} '{patient.PatientName}' {patient.PatientBirthDate} {patient.PatientSex}");
                GetStudies(association, patient);
            }
        }
    }
}
