// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Networking.DataUnits.Items;
using MDSDK.Dicom.Networking.DataUnits.SubItems;

namespace MDSDK.Dicom.Networking.Net
{
    public class PresentationContextResponse
    {
        public byte PresentationContextID { get; set; }

        public enum ResultCode
        {
            Acceptance = 0,
            UserRejection = 1,
            NoReason = 2,
            AbstractSyntaxNotSupported = 3,
            TransferSyntaxesNotSupported = 4
        }

        public ResultCode Result { get; set; }

        public string TransferSyntaxName { get; set; }

        internal PresentationContextResponseItem ToItem()
        {
            var item = new PresentationContextResponseItem
            {
                PresentationContextID = PresentationContextID,
                Result = (byte)Result
            };

            if (Result == ResultCode.Acceptance)
            {
                item.TransferSyntaxSubItem = new TransferSyntaxSubItem
                {
                    TransferSyntaxName = TransferSyntaxName
                };
            };

            return item;
        }

        internal static PresentationContextResponse FromItem(PresentationContextResponseItem item)
        {
            var presentationContextResponse = new PresentationContextResponse
            {
                PresentationContextID = item.PresentationContextID,
                Result = (ResultCode)item.Result
            };
            if (presentationContextResponse.Result == ResultCode.Acceptance)
            {
                presentationContextResponse.TransferSyntaxName = item.TransferSyntaxSubItem.TransferSyntaxName;
            }
            return presentationContextResponse;
        }
    }
}
