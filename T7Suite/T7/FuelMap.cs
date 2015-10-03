using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T7
{
    public class FuelMap
    {
        private byte[] _fuelMap = new byte[18 * 16];

        public byte[] FuelMapData
        {
            get { return _fuelMap; }
            set
            {
                for (int i = 0; i < _fuelMap.Length; i++)
                {
                    _fuelMap[i] = value[i];
                }
            }
        }

        public void Initialize()
        {
            for (int i = 0; i < _fuelMap.Length; i++)
            {
                _fuelMap[i] = 0;
            }
        }

        public byte GetByteAtPosition(int pos)
        {
            return _fuelMap[pos];
        }

        public byte GetByteAtXY(int x, int y)
        {
            return _fuelMap[(y * 18) + x];
        }

        public void SetByteXY(int x, int y, byte value)
        {
            _fuelMap[(y * 18) + x] = value;
        }

    }

    
}
