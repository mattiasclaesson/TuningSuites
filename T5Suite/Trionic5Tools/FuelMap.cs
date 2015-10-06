using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trionic5Tools
{
    public class FuelMap
    {
        private byte[] _fuelMap = new byte[16 * 16];

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
            return _fuelMap[(y * 16) + x];
        }

        public void SetByteXY(int x, int y, byte value)
        {
            _fuelMap[(y * 16) + x] = value;
        }

    }

    public class IdleFuelMap
    {
        private byte[] _idlefuelMap = new byte[12 * 8];

        public byte[] IdleFuelMapData
        {
            get { return _idlefuelMap; }
            set
            {
                for (int i = 0; i < _idlefuelMap.Length; i++)
                {
                    _idlefuelMap[i] = value[i];
                }
            }
        }

        public void Initialize()
        {
            for (int i = 0; i < _idlefuelMap.Length; i++)
            {
                _idlefuelMap[i] = 0;
            }
        }

        public byte GetByteAtPosition(int pos)
        {
            return _idlefuelMap[pos];
        }

        public byte GetByteAtXY(int x, int y)
        {
            return _idlefuelMap[(y * 12) + x];
        }

        public void SetByteXY(int x, int y, byte value)
        {
            _idlefuelMap[(y * 12) + x] = value;
        }

    }
}
