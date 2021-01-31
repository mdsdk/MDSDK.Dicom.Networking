// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using System;

namespace MDSDK.Dicom.Networking.Examples.QueryRetrieve
{
    public class StudyQuery : StudyInfo
    {
        public QueryRetrieveLevel QueryRetrieveLevel { get; set; } = QueryRetrieveLevel.STUDY;

        public string PatientID { get; set; }

        public StudyQuery(string patientID)
        {
            PatientID = patientID;
            StudyDate = string.Empty;
            StudyDescription = string.Empty;
            StudyInstanceUID = string.Empty;
            ModalitiesInStudy = Array.Empty<string>();
        }
    }
}
