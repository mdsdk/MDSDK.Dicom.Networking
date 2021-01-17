// Copyright (c) Robin Boerdijk - All rights reserved - See LICENSE file for license terms

namespace MDSDK.Dicom.Networking.DataUnits
{
    abstract class DataUnitFactory<T> where T : DataUnit
    {
        public abstract T Create(DataUnitType dataUnitType);
    }
}
