// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Networking.Examples.Echo;
using MDSDK.Dicom.Networking.Examples.QueryRetrieve;
using MDSDK.Dicom.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace MDSDK.Dicom.Networking.Test
{
    class Program
    {
        static DicomNetworkAddress RemoteAEAddress;

        static string PatientName;

        static void ParseCommandLine(string[] args)
        {
            if (args.Length != 2)
            {
                throw new Exception("Usage: <remote AE address> <patient name>");
            }

            if (!DicomNetworkAddress.TryParse(args[0], out RemoteAEAddress))
            {
                throw new Exception($"Invalid DICOM network address '{args[0]}'");
            }

            if (args[1].Length < 4)
            {
                throw new Exception($"Patient name too short");
            }

            PatientName = args[1];
        }

        static void Main(string[] args)
        {
            try
            {
                ParseCommandLine(args);

                using var cancellationTokenSource = new CancellationTokenSource();

                Console.CancelKeyPress += (sender, e) =>
                {
                    cancellationTokenSource.Cancel();
                    e.Cancel = true;
                };

                using var stdout = new StreamWriter(Console.OpenStandardOutput());

                var client = new DicomClient("Test")
                {
                    CancellationToken = cancellationTokenSource.Token,
                    TraceWriter = stdout
                };

                var cEchoSCU = new CEchoSCU(client);

                var cFindSCU = new CFindSCU(client);

                var cGetSCU = new CGetSCU(client,
                    DicomUID.SOPClass.MRImageStorage,
                    DicomUID.SOPClass.CTImageStorage,
                    DicomUID.SOPClass.UltrasoundImageStorage,
                    DicomUID.SOPClass.NuclearMedicineImageStorage,
                    DicomUID.SOPClass.PositronEmissionTomographyImageStorage
                );

                using var association = client.ConnectTo(RemoteAEAddress);

                cEchoSCU.Ping(association);

                var sopInstances = new List<SOPInstanceIdentifier>();

                cFindSCU.FindSOPInstances(association, PatientName, sopInstances);
                // sopInstances.Clear();

                foreach (var sopInstance in sopInstances)
                {
                    cGetSCU.Download(association, sopInstance);
                }

                association.Release();
            }
            catch (Exception error)
            {
                Console.WriteLine($"ERROR: {error}");
            }
        }
    }
}
