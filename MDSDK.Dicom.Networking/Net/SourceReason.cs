// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using System;

namespace MDSDK.Dicom.Networking.Net
{
    public readonly struct SourceReason : IEquatable<SourceReason>
    {
        public byte Source { get; }

        public byte Reason { get; }

        public SourceReason(byte source, byte reason)
        {
            Source = source;
            Reason = reason;
        }

        public bool Equals(SourceReason other) => (Source == other.Source) && (Reason == other.Reason);

        public override bool Equals(object obj) => (obj is SourceReason sourceReason) && Equals(sourceReason);

        public override int GetHashCode() => (Source << 8) | Reason;

        public static bool operator ==(SourceReason a, SourceReason b) => a.Equals(b);

        public static bool operator !=(SourceReason a, SourceReason b) => !a.Equals(b);

        public bool IsTransient
        {
            get
            {
                return (this == ServiceProvider_Presentation_LocalLimitExceeded)
                    || (this == ServiceProvider_Presentation_TemporaryCongestion);
            }
        }

        private const byte ServiceUser = 1;
        private const byte ServiceProvider_ACSE = 2;
        private const byte ServiceProvider_Presentation = 3;

        public static readonly SourceReason ServiceUser_NoReasonGiven = new SourceReason(ServiceUser, 1);
        public static readonly SourceReason ServiceUser_ApplicationContextNameNotSupported = new SourceReason(ServiceUser, 2);
        public static readonly SourceReason ServiceUser_CallingAETitleNotRecognized = new SourceReason(ServiceUser, 3);
        public static readonly SourceReason ServiceUser_CalledAETitleNotRecognized = new SourceReason(ServiceUser, 7);
        public static readonly SourceReason ServiceProvider_ACSE_NoReasonGiven = new SourceReason(ServiceProvider_ACSE, 1);
        public static readonly SourceReason ServiceProvider_ACSE_ProtocolVersionNotSupported = new SourceReason(ServiceProvider_ACSE, 2);
        public static readonly SourceReason ServiceProvider_Presentation_TemporaryCongestion = new SourceReason(ServiceProvider_Presentation, 1);
        public static readonly SourceReason ServiceProvider_Presentation_LocalLimitExceeded = new SourceReason(ServiceProvider_Presentation, 2);
    }
}