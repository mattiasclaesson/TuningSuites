using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace RealtimeGraph
{
    public class RealtimeSymbolHelper
    {
        int start_address = 0x00000;

        int flash_start_address = 0x00000;

        public int Flash_start_address
        {
            get { return flash_start_address; }
            set { flash_start_address = value; }
        }

        public Int32 Start_address
        {
            get { return start_address; }
            set { start_address = value; }
        }
        int length = 0x00;

        public int Length
        {
            get { return length; }
            set { length = value; }
        }
        string varname = string.Empty;

        public string Varname
        {
            get { return varname; }
            set { varname = value; }
        }

        private Color _color = Color.Black;

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }
    }
}
