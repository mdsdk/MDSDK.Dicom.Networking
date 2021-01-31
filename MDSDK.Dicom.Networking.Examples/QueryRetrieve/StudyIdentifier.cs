// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Serialization;
using System;

namespace MDSDK.Dicom.Networking.Examples.QueryRetrieve
{
    public class StudyIdentifier : StudyInfo
    {
        public QueryRetrieveLevel QueryRetrieveLevel { get; set; } = QueryRetrieveLevel.STUDY;

        public string PatientID { get; set; }

        public StudyIdentifier(string patientID)
        {
            PatientID = patientID;
            StudyDate = string.Empty;
            StudyDescription = string.Empty;
            StudyInstanceUID = string.Empty;
            SOPClassesInStudy = Array.Empty<DicomUID>();
        }
    }
}
