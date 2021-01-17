// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;

namespace MDSDK.Dicom.Networking.SCUs
{
    public interface ISCP
    {
        void HandleRequest(BinaryStreamReader input);
    }
}
