// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Examples.QueryRetrieve
{
    public class SeriesInfo
    {
        public string Modality { get; set; }
        
        public string SeriesTime { get; set; }

        public string SeriesInstanceUID { get; set; }

        public string SeriesDescription { get; set; }

        public int NumberOfSeriesRelatedInstances { get; set; }
    }
}
