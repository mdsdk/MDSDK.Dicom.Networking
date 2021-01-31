// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Serialization;

namespace MDSDK.Dicom.Networking.Examples.QueryRetrieve
{
    public class StudyInfo
    {
        public string StudyDate { get; set; }
        
        public string StudyInstanceUID { get; set; }

        public string StudyDescription { get; set; }

        public DicomUID[] SOPClassesInStudy { get; set; }
    }
}
