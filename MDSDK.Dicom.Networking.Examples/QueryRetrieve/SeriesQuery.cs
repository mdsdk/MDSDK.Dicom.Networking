// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using System;

namespace MDSDK.Dicom.Networking.Examples.QueryRetrieve
{
    public class SeriesQuery : SeriesInfo
    {
        public QueryRetrieveLevel QueryRetrieveLevel { get; set; } = QueryRetrieveLevel.SERIES;

        public string PatientID { get; set; }

        public string StudyInstanceUID { get; set; }

        public SeriesQuery(string patientID, string studyInstanceUID)
        {
            PatientID = patientID;
            StudyInstanceUID = studyInstanceUID;
            SeriesTime = string.Empty;
            Modality = string.Empty;
            SeriesInstanceUID = string.Empty;
            SeriesDescription = string.Empty;
            NumberOfSeriesRelatedInstances = 0;
        }
    }
}
