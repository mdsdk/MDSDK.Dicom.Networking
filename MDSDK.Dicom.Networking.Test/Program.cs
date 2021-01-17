// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.Dicom.Networking.Messages;
using MDSDK.Dicom.Networking.SCUs;
using System;
using System.IO;

namespace MDSDK.Dicom.Networking.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 1)
                {
                    throw new Exception("Missing remote AE address in command line");
                }

                if (!DicomNetworkAddress.TryParse(args[0], out DicomNetworkAddress remoteAEAddress))
                {
                    throw new Exception($"Invalid DICOM network address '{args[0]}'");
                }

                using var stdout = new StreamWriter(Console.OpenStandardOutput());

                using var cEchoSCU = new CEchoSCU
                {
                    AETitle = "Test",
                    TraceWriter = stdout
                };

                Console.CancelKeyPress += (sender, e) =>
                {
                    cEchoSCU.Cancel();
                    e.Cancel = true;
                };

                cEchoSCU.ConnectTo(remoteAEAddress);

                var cEchoRequest = CEchoRequest.Create((ushort)DateTime.Now.Ticks);
                
                var cEchoResponse = cEchoSCU.Call(cEchoRequest);

                cEchoSCU.DisconnectGracefully();

                Console.WriteLine($"C-ECHO status = {cEchoResponse.Status}");
            }
            catch (Exception error)
            {
                Console.WriteLine($"ERROR: {error}");
            }
        }
    }
}
