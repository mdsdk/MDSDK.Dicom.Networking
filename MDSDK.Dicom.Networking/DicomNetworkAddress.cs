// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using System;

namespace MDSDK.Dicom.Networking
{
    public class DicomNetworkAddress : IEquatable<DicomNetworkAddress>
    {
        public string HostNameOrIPAddress { get; }

        public ushort PortNumber { get; }

        public string AETitle { get; }

        public DicomNetworkAddress(string hostNameOrIPAddress, ushort portNumber, string aeTitle)
        {
            HostNameOrIPAddress = hostNameOrIPAddress;
            PortNumber = portNumber;
            AETitle = aeTitle;
        }

        public bool Equals(DicomNetworkAddress other)
        {
            // AE titles are case sensitive but host names are not

            return (other != null)
                && HostNameOrIPAddress.Equals(other.HostNameOrIPAddress, StringComparison.InvariantCultureIgnoreCase)
                && PortNumber.Equals(other.PortNumber)
                && AETitle.Equals(other.AETitle, StringComparison.InvariantCulture);
        }

        public override bool Equals(object obj) => (obj is DicomNetworkAddress other) && Equals(other);

        public override int GetHashCode() => Tuple.Create(HostNameOrIPAddress, PortNumber, AETitle).GetHashCode();

        public override string ToString() => $"{HostNameOrIPAddress}:{PortNumber}/{AETitle}";

        public static bool TryParse(string s, out DicomNetworkAddress dicomNetworkAddress)
        {
            var hostEndPos = s.IndexOf(':');
            if (hostEndPos > 0)
            {
                var host = s.Substring(0, hostEndPos).Trim();
                var portStartPos = hostEndPos + 1;
                var portEndPos = s.IndexOf('/', portStartPos);
                if ((portEndPos > portStartPos) && ushort.TryParse(s[portStartPos..portEndPos], out ushort port))
                {
                    var aeTitleStartPos = portEndPos + 1;
                    var aeTitle = s.Substring(aeTitleStartPos).Trim();
                    dicomNetworkAddress = new DicomNetworkAddress(host, port, aeTitle);
                    return true;
                }
            }
            dicomNetworkAddress = null;
            return false;
        }

        public static DicomNetworkAddress Parse(string s)
        {
            if (TryParse(s, out DicomNetworkAddress dicomNetworkAddress))
            {
                return dicomNetworkAddress;
            }
            else
            {
                throw new ArgumentException($"Invalid DICOM network address syntax", nameof(s));
            }
        }
    }
}
