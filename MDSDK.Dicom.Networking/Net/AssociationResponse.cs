// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Networking.DataUnits.Items;
using MDSDK.Dicom.Networking.DataUnits.PDUs;
using MDSDK.Dicom.Networking.DataUnits.SubItems;
using System;
using System.Collections.Generic;
using System.Text;

namespace MDSDK.Dicom.Networking.Net
{
    public sealed class AssociationResponse
    {
        public List<PresentationContextResponse> PresentationContextResponses { get; } = new List<PresentationContextResponse>();

        public uint? MaxDataTransferPDULength { get; set; }
        
        internal AssociateAcceptPDU ToPDU(AssociateRequestPDU requestPDU)
        {
            var pdu = new AssociateAcceptPDU();

            requestPDU.CalledAETitle.AsSpan().CopyTo(pdu.CopiedFromAssociateRequestPDU_1);
            requestPDU.CallingAETitle.AsSpan().CopyTo(pdu.CopiedFromAssociateRequestPDU_2);
            requestPDU.Reserved.AsSpan().CopyTo(pdu.CopiedFromAssociateRequestPDU_3);

            pdu.Items.Add(new ApplicationContextItem
            {
                ApplicationContextName = Encoding.ASCII.GetBytes(NetUtils.DicomApplicationContextName)
            });

            foreach (var presentationContextResponse in PresentationContextResponses)
            {
                pdu.Items.Add(presentationContextResponse.ToItem());
            }

            var userInformationItem = new UserInformationItem();

            if (MaxDataTransferPDULength.HasValue)
            {
                userInformationItem.SubItems.Add(new MaximumLengthSubItem
                {
                    MaximumLength = MaxDataTransferPDULength.Value
                });
            }
            
            pdu.Items.Add(userInformationItem);

            return pdu;
        }

        internal static AssociationResponse FromPDU(AssociateAcceptPDU pdu)
        {
            var associationResponse = new AssociationResponse();

            foreach (var item in pdu.Items)
            {
                if (item is ApplicationContextItem applicationContextItem)
                {
                    var applicationContextName = Encoding.ASCII.GetString(applicationContextItem.ApplicationContextName);
                    if (applicationContextName != NetUtils.DicomApplicationContextName)
                    {
                        throw new NotSupportedException($"Unsupported application context name '{applicationContextName}'");
                    }
                }
                else if (item is PresentationContextResponseItem presentationContextItem)
                {
                    associationResponse.PresentationContextResponses.Add(PresentationContextResponse.FromItem(presentationContextItem));
                }
                else if (item is UserInformationItem userInformationItem)
                {
                    foreach (var subItem in userInformationItem.SubItems)
                    {
                        if (subItem is MaximumLengthSubItem maximumLengthSubItem)
                        {
                            associationResponse.MaxDataTransferPDULength = maximumLengthSubItem.MaximumLength;
                        }
                    }
                }
            }

            return associationResponse;
        }
    }
}
