// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using System;

namespace MDSDK.Dicom.Networking.Examples.QueryRetrieve
{
    public class SOPInstanceQuery : SOPInstanceInfo
    {
        public QueryRetrieveLevel QueryRetrieveLevel { get; set; } = QueryRetrieveLevel.IMAGE;

        public string PatientID { get; set; }

        public string StudyInstanceUID { get; set; }

        public string SeriesInstanceUID { get; set; }

        public SOPInstanceQuery(string patientID, string studyInstanceUID, string seriesInstanceUID)
        {
            PatientID = patientID;
            StudyInstanceUID = studyInstanceUID;
            SeriesInstanceUID = seriesInstanceUID;
            SOPInstanceUID = string.Empty;
            SOPClassUID = string.Empty;
        }
    }
}
