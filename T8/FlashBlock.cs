using System;
using System.Collections.Generic;
using System.Linq;
using CommonSuite;
using NLog;

namespace T8SuitePro
{
    public class FlashBlock
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        private int _blockType = 0;

        public int BlockType
        {
            get { return _blockType; }
            set { _blockType = value; }
        }


        private Int32 _blockAddress = 0;

        public Int32 BlockAddress
        {
            get { return _blockAddress; }
            set { _blockAddress = value; }
        }

        private int _blockNumber = 0;

        public int BlockNumber
        {
            get { return _blockNumber; }
            set { _blockNumber = value; }
        }
        private byte[] _blockData;

        public byte[] BlockData
        {
            get { return _blockData; }
            set
            {
                _blockData = value;
                for (int t = 0; t < _blockData.Length; t++)
                {
                    _blockData[t] += 0x53;
                    _blockData[t] ^= 0xA4;
                }
            }
        }

        internal string DataToHex()
        {
            string retval = string.Empty;
            foreach (Byte b in _blockData)
            {
                retval += b.ToString("X2") + " ";
            }
            return retval;
        }

        internal void DecodeBlock(out string VIN, out string ECUDescription, out string InterfaceDevice, out string secretcode)
        {
            VIN = string.Empty;
            ECUDescription = string.Empty;
            InterfaceDevice = string.Empty;
            secretcode = string.Empty;
            // decode the 0x130 byte in the buffer
            // position 0xA7 len 0x10 = description for type of ECU
            // position 0xD8 len 0x11 = VIN number
            // position 0xF9 len 0x0B = interface device
            try
            {
                for (int t = 0xAF; t < 0xAF + 0x10; t++) // was F1
                {
                    ECUDescription += Convert.ToChar(_blockData[t]);
                }
            }
            catch (Exception E)
            {
                logger.Debug("Failed to get ECUDescription: " + E.Message);
            }
            try
            {
                for (int t = 0xE0; t < 0xE0 + 0x11; t++)
                {
                    VIN += Convert.ToChar(_blockData[t]);
                }
            }
            catch (Exception E)
            {
                logger.Debug("Failed to get VIN: " + E.Message);
            }
            try
            {
                for (int t = 0x101; t < 0x101 + 0x0B; t++)
                {
                    if (_blockData[t] != 0xFF)
                    {
                        InterfaceDevice += Convert.ToChar(_blockData[t]);
                    }
                }
            }
            catch (Exception E)
            {
                logger.Debug("Failed to get InterfaceDevice: " + E.Message);
            }
            try
            {
                for (int t = 0x40; t < 0x40 + 0x04; t++)
                {
                    secretcode += Convert.ToChar(_blockData[t]);
                }
            }
            catch (Exception E)
            {
                logger.Debug("Failed to get VIN: " + E.Message);
            }
            //VerifyChecksums();
            DumpVariables();

        }

        private void DumpVariables()
        {
            /*
Variaties in flashblokken:
adres 0x3C length 4 (checksum?)
adres 0x48 length 2
adres 0x84 length 4
adres 0x88 length 4
adres 0x8C length 4
adres 0x90 length 4
adres 0x94 length 4
adres 0xA8 length 4
adres 0xAC length 4
adres 0x12C length 2 (byte checksum?)
             * * */

            Console.Write(GetInt32FromData(0x3C).ToString("X8") + " ");
            Console.Write(GetInt16FromData(0x48).ToString("X4") + " ");
            Console.Write(GetInt32FromData(0x84).ToString("X8") + " ");
            Console.Write(GetInt16FromData(0x89).ToString("X4") + " ");
            Console.Write(GetInt16FromData(0x8D).ToString("X4") + " ");
            Console.Write(GetInt16FromData(0x91).ToString("X4") + " ");
            Console.Write(GetInt16FromData(0x95).ToString("X4") + " ");
            Console.Write(GetInt32FromData(0xA8).ToString("X8") + " ");
            Console.Write(GetInt16FromData(0xAC).ToString("X4") + " ");
            Console.Write(GetInt16FromData(0x12C).ToString("X4") + " ");
            Console.WriteLine();
        }

        private Int32 GetInt16FromData(Int32 offset)
        {
            Int32 var = _blockData[offset++] * 256;
            var += _blockData[offset++];
            return var;
        }

        private Int32 GetInt32FromData(Int32 offset)
        {
            Int32 var = _blockData[offset++] * 256 * 256 * 256;
            var += _blockData[offset++] * 256 * 256;
            var += _blockData[offset++] * 256;
            var += _blockData[offset++];
            return var;
        }

        private void VerifyChecksums()
        {
            byte checksum = 0;
            int idx= 0;
            bool _ok = false;
            foreach (byte b in _blockData)
            {
                checksum += b;
                if (checksum != 0) _ok = true;
                if (checksum == 0 && _ok) logger.Debug("checksum A is zero at index: " + idx.ToString("X2"));
                idx ++;
            }
            idx = 0;
            _ok = false;
            checksum = 0;
            foreach (byte b in _blockData)
            {
                checksum ^= b;
                if (checksum != 0) _ok = true;
                if (checksum == 0 && _ok) logger.Debug("checksum B is zero at index: " + idx.ToString("X2"));
                idx++;
            }

            /*logger.Debug("full checksum: " + checksum.ToString("X2"));
            checksum = 0;
            for(int i = 0; i <= 0x12b; i ++)
            {
                checksum += _blockData[i];
            }
            logger.Debug("partial checksum: " + checksum.ToString("X2") + " file: " + _blockData[0x12d].ToString("X2"));
            checksum = 0;
            for (int i = 0x54; i <= 0x12b; i++)
            {
                checksum += _blockData[i];
            }
            logger.Debug("partial checksum 2: " + checksum.ToString("X2") );
            checksum = 0;
            for (int i = 0; i < 0x54; i++)
            {
                checksum += _blockData[i];
            }
            logger.Debug("partial checksum 3: " + checksum.ToString("X2") );*/
        }

        internal bool isValid()
        {
            // if block starts with 0xF6 0xF6 0xF6 0xF6 0xF6 0xF6 0xF6 it is not valid
            bool valid = true;
            if (_blockData[0] == 0xF6 && _blockData[1] == 0xF6 && _blockData[2] == 0xF6 && _blockData[3] == 0xF6 && _blockData[4] == 0xF6 && _blockData[5] == 0xF6 && _blockData[6] == 0xF6 && _blockData[7] == 0xF6)
            {
                valid = false;
            }
            return valid;
        }

        internal void SetVin(string m_ChassisID)
        {
            for (int t = 0xE0; t < 0xE0 + 0x11; t++)
            {
                _blockData[t] = Convert.ToByte(m_ChassisID[t - 0xE0]); //???
            }
        }

        internal void CodeBlock()
        {
            for (int t = 0; t < _blockData.Length; t++)
            {
                _blockData[t] ^= 0xA4;
                _blockData[t] -= 0x53;
            }
        }
    }
}
