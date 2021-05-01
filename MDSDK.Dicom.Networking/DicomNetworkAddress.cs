// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using System;

namespace MDSDK.Dicom.Networking
{
    /// <summary>Represents the DICOM network address of an application entity</summary>
    public class DicomNetworkAddress : IEquatable<DicomNetworkAddress>
    {
        /// <summary>The host name or IP address of the computer hosting the application entity</summary>
        public string HostNameOrIPAddress { get; }

        /// <summary>The port number of the service hosting the application entity</summary>
        public ushort PortNumber { get; }

        /// <summary>The AE title of the application entity</summary>
        public string AETitle { get; }

        /// <summary>Constructor</summary>
        public DicomNetworkAddress(string hostNameOrIPAddress, ushort portNumber, string aeTitle)
        {
            HostNameOrIPAddress = hostNameOrIPAddress;
            PortNumber = portNumber;
            AETitle = aeTitle;
        }

#pragma warning disable 1591

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

#pragma warning disable 1591

        /// <summary>Returns a string of the form HostNameOrIPAddress:PortNumber/AETitle</summary>
        public override string ToString() => $"{HostNameOrIPAddress}:{PortNumber}/{AETitle}";

        /// <summary>Tries to convert a string to a DicomNetworkAddress</summary>
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

        /// <summary>Converts a string to a DicomNetworkAddress</summary>
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
