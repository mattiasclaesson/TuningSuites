using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace T7.Parser
{
    class T7FileParser
    {

        List<Symbol> m_symbolList = new List<Symbol>();

        public List<Symbol> getSymbolList() { return m_symbolList; }

        public bool parse(string a_t7File)
        {
            byte[] signature = new byte[8] {0xFF, 0x60, 0x4C, 0xDF, 0x3C, 0x0C, 0x4E, 0x75 };
            uint offset;
            int i = 0;
            UInt16 nrOfSymbols;
            if (!File.Exists(a_t7File))
                return false;
            FileStream fs = new FileStream(a_t7File, FileMode.Open, FileAccess.Read);
            fs.Position = 0;
            
            while(i < signature.Length)
            {
                if (fs.ReadByte() == signature[i])
                {
                    i++;
                    continue;
                }
                if (fs.Position > 0x60000)
                    return false;
                i = 0;
            }
            offset = (uint)fs.Position;
            nrOfSymbols = readUint16(fs);
            offset = (uint)nrOfSymbols * 14 + offset;
            long position = fs.Position;
            for (i = 0; i < nrOfSymbols; i++)
            {
                fs.Position = position;
                UInt32 addr = readUint32(fs);
                UInt16 dataLength = readUint16(fs);
                Symbol symbol = new Symbol(dataLength);
                if (addr < 0x70000)
                    symbol.setROMAddress(addr);
                else
                    symbol.setRAMAddress(addr);
                symbol.setDataLength(dataLength);
                symbol.setSymbolType(readUint32(fs));
                symbol.setSymbolNameAddress(readUint32(fs));
                position = fs.Position;

                fs.Position = (long)symbol.getSymbolNameAddress();
                symbol.setSymbolName(readString(fs));

                if (symbol.getROMAddress() != 0)
                {
                    fs.Position = (long)symbol.getROMAddress();
                    byte[] data = new byte[symbol.getDataLength()];
                    fs.Read(data, 0, data.Length);
                    symbol.setROMValue(data);
                }
                m_symbolList.Add(symbol);
            }
            

            return true;
        }

        private UInt16 readUint16(FileStream a_fs)
        {
            UInt16 retval = 0;
            retval = (UInt16)(a_fs.ReadByte() << 8);
            retval = (UInt16)(retval | (UInt16)a_fs.ReadByte());
            return retval;
        }

        private UInt32 readUint32(FileStream a_fs)
        {
            UInt32 retval = 0;
            retval = (UInt32)(a_fs.ReadByte() << 24);
            retval = retval | (UInt32)a_fs.ReadByte() << 16;
            retval = retval | (UInt32)a_fs.ReadByte() << 8;
            retval = retval | (UInt32)a_fs.ReadByte();
            return retval;
        }

        private string readString(FileStream a_fs)
        {
            string str = "";
            char b;
            b = (char) a_fs.ReadByte();
            while (b != 0x0)
            {
                str += b.ToString();
                b = (char)a_fs.ReadByte();
            }
            return str;
        }
    }
}
