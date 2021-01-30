// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Networking.DataUnits.Items;
using MDSDK.Dicom.Networking.DataUnits.SubItems;
using System.Collections.Generic;

namespace MDSDK.Dicom.Networking.Net
{
    public class PresentationContextRequest
    {
        public byte PresentationContextID { get; set; }

        public string AbstractSyntaxName { get; set; }

        public List<string> TransferSyntaxNames { get; } = new List<string>(); 

        internal PresentationContextRequestItem ToItem()
        {
            var item = new PresentationContextRequestItem
            {
                PresentationContextID = PresentationContextID
            };

            item.SubItems.Add(new AbstractSyntaxSubItem
            {
                AbstractSyntaxName = AbstractSyntaxName
            });

            foreach (var transferSyntaxName in TransferSyntaxNames)
            {
                item.SubItems.Add(new TransferSyntaxSubItem
                {
                    TransferSyntaxName = transferSyntaxName
                });
            }

            return item;
        }

        internal static PresentationContextRequest FromItem(PresentationContextRequestItem item)
        {
            var presentationContextRequest = new PresentationContextRequest
            {
                PresentationContextID = item.PresentationContextID
            };

            foreach (var subItem in item.SubItems)
            {
                if (subItem is AbstractSyntaxSubItem abstractSyntaxSubItem)
                {
                    presentationContextRequest.AbstractSyntaxName = abstractSyntaxSubItem.AbstractSyntaxName;
                }
                else if (subItem is TransferSyntaxSubItem transferSyntaxSubItem)
                {
                    presentationContextRequest.TransferSyntaxNames.Add(transferSyntaxSubItem.TransferSyntaxName);
                }
            }

            return presentationContextRequest;
        }
    }
}
