// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.N_EVENT_REPORT_RQ, true)]
    public sealed class NEventReportRequest : Request
    {
        public string AffectedSOPClassUID { get; set; }

        public string AffectedSOPInstanceUID { get; set; }

        public ushort EventTypeID { get; set; }
    }

    public class NEventReportRequestMessage<TEventInformation> : DicomMessage<NEventReportRequest, TEventInformation>
        where TEventInformation : new()
    {
    }
}
