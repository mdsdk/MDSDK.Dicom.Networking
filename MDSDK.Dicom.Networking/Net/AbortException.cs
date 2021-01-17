// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Networking.DataUnits.PDUs;
using System;

namespace MDSDK.Dicom.Networking
{
    public class AbortException : Exception
    {
        public new byte Source { get; }

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
