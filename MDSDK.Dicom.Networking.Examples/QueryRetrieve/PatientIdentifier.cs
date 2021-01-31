// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Examples.QueryRetrieve
{
    public class PatientIdentifier : PatientInfo
    {
        public QueryRetrieveLevel QueryRetrieveLevel { get; set; } = QueryRetrieveLevel.PATIENT;

        public PatientIdentifier()
        {
            PatientName = string.Empty;
            PatientID = string.Empty;
            PatientBirthDate = string.Empty;
            PatientSex = string.Empty;
        }
    }
}
