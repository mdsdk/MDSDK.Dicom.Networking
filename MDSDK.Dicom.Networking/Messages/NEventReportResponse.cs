// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.Messages
{
    [Command(CommandType.N_EVENT_REPORT_RSP, true)]
    public sealed class NEventReportResponse : Response
    {
        public string AffectedSOPClassUID { get; set; }

        public ushort Status { get; set; }

        public string AffectedSOPInstanceUID { get; set; }

        public ushort EventTypeID { get; set; }
    }

    public class NEventReportResponseMessage<TEventReply> : DicomMessage<NEventReportResponse, TEventReply>
        where TEventReply : new()
    {
    }
}
