// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Networking.Examples.Echo;
using MDSDK.Dicom.Networking.Examples.QueryRetrieve;
using MDSDK.Dicom.Networking.SCUs;
using System;
using System.IO;
using System.Threading;

namespace MDSDK.Dicom.Networking.Test
{
    class Program
    {
        static DicomNetworkAddress GetRemoteAEAddress(string[] args)
        {
            if (args.Length != 1)
            {
                throw new Exception("Missing remote AE address in command line");
            }

            if (!DicomNetworkAddress.TryParse(args[0], out DicomNetworkAddress remoteAEAddress))
            {
                throw new Exception($"Invalid DICOM network address '{args[0]}'");
            }

            return remoteAEAddress;
        }

        static void Main(string[] args)
        {
            try
            {
                var remoteAEAddress = GetRemoteAEAddress(args);

                using var cancellationTokenSource = new CancellationTokenSource();

                Console.CancelKeyPress += (sender, e) =>
                {
                    cancellationTokenSource.Cancel();
                    e.Cancel = true;
                };

                using var stdout = new StreamWriter(Console.OpenStandardOutput());

                var client = new DicomClient
                {
                    AETitle = "Test",
                    CancellationToken = cancellationTokenSource.Token,
                    // TraceWriter = stdout
                };

                var cEchoSCU = new CEchoSCU(client);
                var cFindSCU = new CFindSCU(client);

                using var association = client.ConnectTo(remoteAEAddress);

                cEchoSCU.Execute(association);

                cFindSCU.Execute(association, "*TEST*");
                cFindSCU.Execute(association, "*DOE*");

                association.Release();
            }
            catch (Exception error)
            {
                Console.WriteLine($"ERROR: {error}");
            }
        }
    }
}
