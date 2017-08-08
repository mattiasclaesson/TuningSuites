using System;
using System.Collections.Generic;
using System.Text;
using NLog;

namespace CommonSuite
{
    public class SymbolHelper
    {
        private int _bitMask = 0x00000;

        private Logger logger = LogManager.GetCurrentClassLogger();

        public int BitMask
        {
            get { return _bitMask; }
            set { _bitMask = value; }
        }

        System.Drawing.Color _color = System.Drawing.Color.Black;

        public System.Drawing.Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        private byte[] _currentdata;

        public byte[] Currentdata
        {
            get { return _currentdata; }
            set { _currentdata = value; }
        }

        int symbol_number = 0;

        public int Symbol_number
        {
            get { return symbol_number; }
            set { symbol_number = value; }
        }

        int symbol_number_ECU = 0;

        public int Symbol_number_ECU
        {
            get { return symbol_number_ECU; }
            set { symbol_number_ECU = value; }
        }

        bool _selected = false;

        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }

        int symbol_type = 0;

        public int Symbol_type
        {
            get { return symbol_type; }
            set { symbol_type = value; }
        }


        int internal_address = 0x00000;

        public int Internal_address
        {
            get { return internal_address; }
            set { internal_address = value; }
        }


        Int64 start_address = 0x00000;

        Int64 flash_start_address = 0x00000;

        public Int64 Flash_start_address
        {
            get { return flash_start_address; }
            set { flash_start_address = value; }
        }

        public Int64 Start_address
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

        string _userdescription = string.Empty;

        public string Userdescription
        {
            get { return _userdescription; }
            set { _userdescription = value; }
        }


        string _description = string.Empty;

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        string _category = "Undocumented";

        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }
        string _subcategory = "Undocumented";

        public string Subcategory
        {
            get { return _subcategory; }
            set { _subcategory = value; }
        }

        public string SmartVarname
        {
            get
            {
                if (_userdescription != "" && !_userdescription.StartsWith("Symbolnumber "))
                {
                    return _userdescription;
                }
                return varname;
            }
        }

        public void createAndUpdateCategory(string name)
        {
            if (name.Contains("."))
            {
                try
                {
                    _category = name.Substring(0, name.IndexOf("."));
                }
                catch (Exception cE)
                {
                    logger.Error(cE);
                }
            }
        }
    }
}
