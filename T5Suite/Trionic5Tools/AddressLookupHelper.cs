using System;
using System.Collections.Generic;
using System.Text;

namespace Trionic5Tools
{
    public class AddressLookupHelper
    {
        int sram_adddress = 0x00000;

        public int Sram_adddress
        {
            get { return sram_adddress; }
            set { sram_adddress = value; }
        }
        int flash_address = 0x00000;

        public int Flash_address
        {
            get { return flash_address; }
            set { flash_address = value; }
        }

        private bool m_usedAddress = false;

        public bool UsedAddress
        {
            get { return m_usedAddress; }
            set { m_usedAddress = value; }
        }
    }
}
