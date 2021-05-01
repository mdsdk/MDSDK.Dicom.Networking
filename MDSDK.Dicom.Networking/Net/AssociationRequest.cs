// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Networking.DataUnits.Items;
using MDSDK.Dicom.Networking.DataUnits.PDUs;
using MDSDK.Dicom.Networking.DataUnits.SubItems;
using System;
using System.Collections.Generic;
using System.Text;

namespace MDSDK.Dicom.Networking.Net
{
    internal sealed class AssociationRequest
    {
        public string CalledAETitle { get; set; }

        public string CallingAETitle { get; set; }

        public List<PresentationContextRequest> PresentationContextRequests { get; } = new List<PresentationContextRequest>();

        public uint? MaxDataTransferPDULength { get; set; }

        internal AssociateRequestPDU ToPDU()
        {
            var pdu = new AssociateRequestPDU();
            
            NetUtils.WriteAsciiStringTo(CalledAETitle, pdu.CalledAETitle);
            
            NetUtils.WriteAsciiStringTo(CallingAETitle, pdu.CallingAETitle);

            pdu.Items.Add(new ApplicationContextItem
            {
                ApplicationContextName = Encoding.ASCII.GetBytes(NetUtils.DicomApplicationContextName)
            });

            foreach (var presentationContextRequest in PresentationContextRequests)
            {
                pdu.Items.Add(presentationContextRequest.ToItem());
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

        internal static AssociationRequest FromPDU(AssociateRequestPDU pdu)
        {
            var associationRequest = new AssociationRequest
            {
                CalledAETitle = NetUtils.ReadAsciiStringFrom(pdu.CalledAETitle),
                CallingAETitle = NetUtils.ReadAsciiStringFrom(pdu.CallingAETitle)
            };

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
                else if (item is PresentationContextRequestItem presentationContextItem)
                {
                    associationRequest.PresentationContextRequests.Add(PresentationContextRequest.FromItem(presentationContextItem));
                }
                else if (item is UserInformationItem userInformationItem)
                {
                    foreach (var subItem in userInformationItem.SubItems)
                    {
                        if (subItem is MaximumLengthSubItem maximumLengthSubItem)
                        {
                            associationRequest.MaxDataTransferPDULength = maximumLengthSubItem.MaximumLength;
                        }
                    }
                }
            }

            return associationRequest;
        }
    }
}
