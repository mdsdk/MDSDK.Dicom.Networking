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

        public IReadOnlyList<string> TransferSyntaxNames { get; set; } 

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

            var transferSyntaxNames = new List<string>();

            foreach (var subItem in item.SubItems)
            {
                if (subItem is AbstractSyntaxSubItem abstractSyntaxSubItem)
                {
                    presentationContextRequest.AbstractSyntaxName = abstractSyntaxSubItem.AbstractSyntaxName;
                }
                else if (subItem is TransferSyntaxSubItem transferSyntaxSubItem)
                {
                    transferSyntaxNames.Add(transferSyntaxSubItem.TransferSyntaxName);
                }
            }

            presentationContextRequest.TransferSyntaxNames = transferSyntaxNames;

            return presentationContextRequest;
        }
    }
}
