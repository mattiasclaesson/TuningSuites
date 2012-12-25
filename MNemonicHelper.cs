using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T7
{
    public class MNemonicHelper
    {
        private string mnemonic;

        public string Mnemonic
        {
            get { return mnemonic; }
            set { mnemonic = value; }
        }
        private long address;

        public long Address
        {
            get { return address; }
            set { address = value; }
        }

    }
}
