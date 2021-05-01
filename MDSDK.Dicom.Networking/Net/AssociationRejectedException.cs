// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Networking.DataUnits.PDUs;
using MDSDK.Dicom.Networking.Net;
using System;

namespace MDSDK.Dicom.Networking.Net
{
    /// <summary>Exception thrown when an Associate Reject PDU is received in response to an association request</summary>
    public class AssociationRejectedException : Exception
    {
        /// <summary>The Result code returned in the Associate Reject PDU</summary>
        public byte Result { get; }

        /// <summary>The Source and Reason codes returned in the Associate Reject PDU</summary>
        public SourceReason SourceReason { get; }

        internal AssociationRejectedException(AssociateRejectPDU associateRejectPDU)
        {
            Result = associateRejectPDU.Result;
            SourceReason = new SourceReason(associateRejectPDU.Source, associateRejectPDU.Reason);
        }

        internal AssociateRejectPDU ToPDU()
        {
            return new AssociateRejectPDU
            {
                Result = (byte)Result,
                Source = SourceReason.Source,
                Reason = SourceReason.Reason
            };
        }
    }
}
