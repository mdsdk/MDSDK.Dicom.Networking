// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Networking.DataUnits.PDUs;
using MDSDK.Dicom.Networking.Net;
using System;

namespace MDSDK.Dicom.Networking.Net
{
    public class AssociationRejectedException : Exception
    {
        public enum RejectResult : byte
        {
            Permanent = 1,
            Transient = 2
        };

        public RejectResult Result { get; }

        public SourceReason SourceReason { get; }

        public AssociationRejectedException(SourceReason sourceReason)
        {
            Result = sourceReason.IsTransient ? RejectResult.Transient : RejectResult.Permanent;
            SourceReason = sourceReason;
        }

        internal AssociationRejectedException(AssociateRejectPDU associateRejectPDU)
        {
            Result = (RejectResult)associateRejectPDU.Result;
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
