// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Networking.Messages;
using MDSDK.Dicom.Serialization;
using System;
using System.Collections.Generic;

namespace MDSDK.Dicom.Networking.Examples.QueryRetrieve
{
    public class CFindSCU
    {
        public byte PresentationContextID { get; }

        public CFindSCU(DicomClient client)
        {
            PresentationContextID = client.ProposePresentationContext(DicomUID.PatientRootQueryRetrieveInformationModelFIND,
                DicomTransferSyntax.ImplicitVRLittleEndian);
        }

        private IReadOnlyList<TInfo> PerformCFind<TIdentifier, TInfo>(DicomAssociation association, TIdentifier identifier)
            where TInfo : new()
        {
            var cFindRequest = new CFindRequest
            {
                AffectedSOPClassUID = DicomUID.PatientRootQueryRetrieveInformationModelFIND.UID,
                Priority = RequestPriority.Medium
            };

            association.SendRequest(PresentationContextID, cFindRequest, CommandIsFollowedByDataSet.Yes);
            association.SendDataSet(PresentationContextID, identifier);

            var infoList = new List<TInfo>();

            var cFindResponse = association.ReceiveResponse<CFindResponse>(PresentationContextID, cFindRequest.MessageID);
            while (cFindResponse.StatusIsPending())
            {
                var info = association.ReceiveDataSet<TInfo>(PresentationContextID);
                infoList.Add(info);
                cFindResponse = association.ReceiveResponse<CFindResponse>(PresentationContextID, cFindRequest.MessageID);
            }

            if (!cFindResponse.StatusIsSuccess())
            {
                throw new Exception($"C-FIND SCP returned {cFindResponse.Status}");
            }

            return infoList;
        }

        private void FindSOPInstances(DicomAssociation association, PatientInfo patient, StudyInfo study, SeriesInfo series,
            List<SOPInstanceIdentifier> sopInstanceIdentifiers)
        {
            var sopInstanceSearchKey = new SOPInstanceIdentifier(patient.PatientID, study.StudyInstanceUID, series.SeriesInstanceUID);
            var sopInstanceList = PerformCFind<SOPInstanceIdentifier, SOPInstanceInfo>(association, sopInstanceSearchKey);
            foreach (var sopInstance in sopInstanceList)
            {
                sopInstanceIdentifiers.Add(new SOPInstanceIdentifier(patient.PatientID, study.StudyInstanceUID, series.SeriesInstanceUID)
                {
                    SOPClassUID = sopInstance.SOPClassUID,
                    SOPInstanceUID = sopInstance.SOPInstanceUID
                });

                var sopClass = new DicomUID(sopInstance.SOPClassUID);

                Console.WriteLine($">>> SOP Instance: {sopClass} ({sopInstance.SOPInstanceUID})");
            }
        }

        private void FindSOPInstances(DicomAssociation association, PatientInfo patient, StudyInfo study,
            List<SOPInstanceIdentifier> sopInstanceIdentifiers)
        {
            var seriesSearchKey = new SeriesIdentifier(patient.PatientID, study.StudyInstanceUID);
            var seriesList = PerformCFind<SeriesIdentifier, SeriesInfo>(association, seriesSearchKey);
            foreach (var series in seriesList)
            {
                Console.WriteLine($">> Series: {series.Modality} {series.SeriesTime} '{series.SeriesDescription}' {series.NumberOfSeriesRelatedInstances}");
                FindSOPInstances(association, patient, study, series, sopInstanceIdentifiers);
            }
        }

        private void FindSOPInstances(DicomAssociation association, PatientInfo patient,
            List<SOPInstanceIdentifier> sopInstanceIdentifiers)
        {
            var studySearchKey = new StudyIdentifier(patient.PatientID);
            var studyList = PerformCFind<StudyIdentifier, StudyInfo>(association, studySearchKey);
            foreach (var study in studyList)
            {
                Console.WriteLine($"> Study: {study.StudyDate} '{study.StudyDescription}'");
                FindSOPInstances(association, patient, study, sopInstanceIdentifiers);
            }
        }

        public void FindSOPInstances(DicomAssociation association, string patientName,
            List<SOPInstanceIdentifier> sopInstanceIdentifiers)
        {
            var patientSearchKey = new PatientIdentifier { PatientName = patientName };
            var patientList = PerformCFind<PatientIdentifier, PatientInfo>(association, patientSearchKey);
            foreach (var patient in patientList)
            {
                Console.WriteLine($"Patient: {patient.PatientID} '{patient.PatientName}' {patient.PatientBirthDate} {patient.PatientSex}");
                FindSOPInstances(association, patient, sopInstanceIdentifiers);
            }
        }
    }
}
