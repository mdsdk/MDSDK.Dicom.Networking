// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

using MDSDK.BinaryIO;
using MDSDK.Dicom.Networking.DataUnits;
using MDSDK.Dicom.Networking.DataUnits.PDUs;
using System;
using System.IO;

namespace MDSDK.Dicom.Networking.Net
{
    internal sealed class PresentationContextInputStream : StreamBase
    {
        private readonly DicomConnection _connection;

        private readonly FragmentType _fragmentType;

        private FragmentHeader _fragmentHeader;

        private long _fragmentEndPosition;

        public byte PresentationContextID { get; private set; }

        public PresentationContextInputStream(DicomConnection connection, FragmentType fragmentType)
        {
            _connection = connection;

            _fragmentType = fragmentType;

            StartReadFragment(isFirstFragment: true);

            PresentationContextID = _fragmentHeader.PresentationContextID;
        }

        private void StartReadFragment(bool isFirstFragment)
        {
            if (_fragmentHeader != null)
            {
                throw new Exception("Logic error");
            }

            var fragmentHeader = FragmentHeader.ReadFrom(_connection.Input);

            if (!isFirstFragment && (_fragmentHeader.PresentationContextID != PresentationContextID))
            {
                throw new IOException($"Expected presentation context ID {PresentationContextID} but got {_fragmentHeader.PresentationContextID}");
            }

            if (fragmentHeader.FragmentType != _fragmentType)
            {
                throw new IOException($"Expected {_fragmentType} but got {fragmentHeader.FragmentType}");
            }

            _fragmentHeader = fragmentHeader;
            _fragmentEndPosition = _connection.Input.Position + (fragmentHeader.Length - 2);
        }

        private void EndReadFragment(out bool wasLastFragment)
        {
            if (_fragmentHeader == null)
            {
                throw new Exception("Logic error");
            }

            if (_connection.Input.Position != _fragmentEndPosition)
            {
                throw new Exception("Logic error");
            }

            wasLastFragment = _fragmentHeader.IsLastFragment;
            
            _fragmentHeader = null;
            _fragmentEndPosition = 0;
        }

        public override bool CanRead => true;

        private bool _atEnd;

        public override int Read(Span<byte> buffer)
        {
            if (buffer.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(buffer));
            }

            if (_atEnd)
            {
                return 0;
            }

            while (_connection.Input.Position == _fragmentEndPosition)
            {
                EndReadFragment(out bool wasLastFragment);
                if (wasLastFragment)
                {
                    _atEnd = true;
                    return 0;
                }
                if (_connection.Input.Position == _connection.EndOfDataTransferPDUPosition)
                {
                    _connection.ReadNextDataTransferPDU();
                }
                StartReadFragment(isFirstFragment: false);
            }
            
            return _connection.Input.ReadSome(buffer);
        }

        public override void Close()
        {
            if (_fragmentHeader != null)
            {
                EndReadFragment(out bool wasLastFragment);
                if (!wasLastFragment)
                {
                    throw new Exception("Logic error");
                }
            }
        }
    }
}
