using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace T7.Parser
{
    class SymbolMapParser
    {

        List<Symbol> m_symbolList = new List<Symbol>();

        public List<Symbol> getSymbolList() { return m_symbolList; }
        private UInt32 m_nrOfSymbols;

        public Symbol getSymbol(UInt32 a_symbolNr)
        {
            if (a_symbolNr > m_nrOfSymbols)
                return new Symbol();
            return m_symbolList[(int)a_symbolNr];
        }

        private bool tableContainsStrings = false;

        public bool TableContainsStrings
        {
            get { return tableContainsStrings; }
            set { tableContainsStrings = value; }
        }

        public bool parse(string a_t7File)
        {
            m_nrOfSymbols = 0;
            if (!File.Exists(a_t7File))
                return false;
            FileStream fs = new FileStream(a_t7File, FileMode.Open, FileAccess.Read);
            fs.Position = 0;
           
            StreamReader reader = new StreamReader( a_t7File );
            string content = reader.ReadToEnd();
            reader.Close();
            UInt32 blockNr = 0;
            UInt32 symbolNr = 0;
            tableContainsStrings = false;
            if (!Regex.IsMatch(content, "BlockType"))
                tableContainsStrings = false;
            else
                tableContainsStrings = true;
            while (fs.Position < fs.Length)
            {
                UInt32 address;
                Symbol symbol = new Symbol();
                fs.ReadByte();
                address = readUint32(fs, 3);
                if (address < 0xF00000)
                    symbol.setROMAddress(address);
                else
                    symbol.setRAMAddress(address);
                symbol.setDataLength(readUint32(fs, 2));
                symbol.setSymbolType(readUint32(fs, 1));
                if (tableContainsStrings)
                {
                    symbol.setSymbolName(readString(fs));
                }
                else
                {
                    if (symbol.getRAMAddress() == symbol.getROMAddress())
                        symbol.setSymbolName("Block nr " + blockNr++);
                    else
                        symbol.setSymbolName("Symbol nr " + symbolNr++);
                }
                m_symbolList.Add(symbol);
                //LogHelper.Log("Added: " + symbol.getSymbolName());
                m_nrOfSymbols++;
            }

            return true;
        }

        private UInt32 readUint32(FileStream a_fs, int nrOfBytes)
        {
            UInt32 retval = 0;
            for (int i = nrOfBytes; i > 0; i--)
            {
                retval = retval | ((UInt32)a_fs.ReadByte() << 8*(i-1));
            }
            return retval;
        }

        private string readString(FileStream a_fs)
        {
            string str = "";
            char b;
            b = (char)a_fs.ReadByte();
            while (b != 0x0)
            {
                str += b.ToString();
                b = (char)a_fs.ReadByte();
                if (a_fs.Position >= a_fs.Length)
                    return str;
            }
            return str;
        }
    }
}
