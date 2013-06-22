using System;
using System.Collections.Generic;
using System.Text;

namespace T7.Parser
{
    class Symbol
    {
        public Symbol(UInt16 datalength)
        {
            m_romValue = new byte[datalength];
            m_ramValue = new byte[datalength];
        }
        public string   getSymbolName() { return m_symbolName; }
        public void     setSymbolName(string a_symbolName) { m_symbolName = a_symbolName; }
        public UInt32   getSymbolNameAddress() { return m_symbolAddress; }
        public void     setSymbolNameAddress(UInt32 a_address) { m_symbolAddress = a_address; }
        public UInt32   getROMAddress() { return m_romAddress; }
        public void     setROMAddress(UInt32 a_address) { m_romAddress = a_address; }
        public UInt32   getRAMAddress() { return m_ramAddress; }
        public void     setRAMAddress(UInt32 a_address) { m_ramAddress = a_address; }
        public uint     getDataLength() { return m_dataLength; }
        public void     setDataLength(uint a_length) { m_dataLength = a_length; }
        public UInt32   getSymbolType() { return m_symbolType; }
        public void     setSymbolType(UInt32 a_symbolType) { m_symbolType = a_symbolType; }
        public byte[]   getROMValue() { return m_romValue; }
        public void     setROMValue(byte[] a_value) { m_romValue = a_value; }
        public byte[]   getRAMValue() { return m_ramValue; }
        public void     setRAMValue(byte[] a_value) { m_ramValue = a_value; }

        private string  m_symbolName;
        private UInt32  m_symbolAddress;
        private UInt32  m_romAddress;
        private byte[]  m_romValue;
        private UInt32  m_ramAddress;
        private byte[]  m_ramValue;
        private uint    m_dataLength;
        private UInt32  m_symbolType;

        override public string ToString()
        {
            if (m_romAddress != 0)
                return "Symbolname: " + m_symbolName + "\n" +
                   "ROM address: " + m_romAddress + "\n" +
                   "RAM adrress: " + m_ramAddress + "\n" +
                   "Data length: " + m_dataLength + "\n";
            else
                return "Symbolname: " + m_symbolName + "\n" +
                   "ROM address: " + m_romAddress + "\n" +
                   "RAM adrress: " + m_ramAddress + "\n" +
                   "Data length: " + m_dataLength + "\n";
        }

        public Symbol(){}
    }
}
