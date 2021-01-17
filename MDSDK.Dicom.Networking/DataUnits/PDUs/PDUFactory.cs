// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using System;

namespace MDSDK.Dicom.Networking.DataUnits.PDUs
{
    class PDUFactory : DataUnitFactory<PDU>
    {
        public static PDUFactory Instance { get; } = new PDUFactory();

        public override PDU Create(DataUnitType dataUnitType)
        {
            return dataUnitType switch
            {
                DataUnitType.AssociateRequestPDU => new AssociateRequestPDU(),
                DataUnitType.AssociateAcceptPDU => new AssociateAcceptPDU(),
                DataUnitType.AssociateRejectPDU => new AssociateRejectPDU(),
                DataUnitType.DataTransferPDU => new DataTransferPDUHeader(),
                DataUnitType.ReleaseRequestPDU => new ReleaseRequestPDU(),
                DataUnitType.ReleaseResponsePDU => new ReleaseResponsePDU(),
                DataUnitType.AbortPDU => new AbortPDU(),
                _ => throw new NotSupportedException($"Expected PDU but got {dataUnitType}"),
            };
        }
    }
}
