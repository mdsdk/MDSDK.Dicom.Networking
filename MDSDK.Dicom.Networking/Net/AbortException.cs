// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Networking.DataUnits.PDUs;
using System;

namespace MDSDK.Dicom.Networking
{
    /// <summary>Exception thrown when an Abort PDU while waiting for an incoming message</summary>
    public class AbortException : Exception
    {
        /// <summary>Identifies the source of the Abort PDU</summary>
        public new byte Source { get; }

        /// <summary>Identifies the reason why the Abort PDU was sent</summary>
        public byte Reason { get; }

        internal AbortException(AbortPDU abortPDU)
        {
            Source = abortPDU.Source;
            Reason = abortPDU.Reason;
        }

        internal AbortPDU ToPDU()
        {
            return new AbortPDU
            {
                Source = Source,
                Reason = Reason
            };
        }
    }
}
