// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Examples.QueryRetrieve
{
    public class StudyInfo
    {
        public string StudyDate { get; set; }
        
        public string StudyInstanceUID { get; set; }

        public string StudyDescription { get; set; }

        public string[] ModalitiesInStudy { get; set; }
    }
}
