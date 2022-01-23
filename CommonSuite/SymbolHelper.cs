using System;
using System.Collections.Generic;
using System.Text;
using NLog;

namespace CommonSuite
{
    public class SymbolHelper
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        private System.Drawing.Color _color = System.Drawing.Color.Black;
        public System.Drawing.Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        Int64 flash_start_address = 0x00000;
        public Int64 Flash_start_address
        {
            get { return flash_start_address; }
            set { flash_start_address = value; }
        }

        Int64 start_address = 0x00000;
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

        int symbol_extendedtype = 0;
        public int Symbol_extendedtype
        {
            get { return symbol_extendedtype; }
            set { symbol_extendedtype = value; }
        }

        int internal_address = 0x00000;
        public int Internal_address
        {
            get { return internal_address; }
            set { internal_address = value; }
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

        // T8
        private int _bitMask = 0x00000;
        public int BitMask
        {
            get { return _bitMask; }
            set { _bitMask = value; }
        }

		// T5
		private bool _isSystemSymbol = false;
        public bool IsSystemSymbol
        {
            get { return _isSystemSymbol; }
            set { _isSystemSymbol = value; }
        }

        private XDFCategories m_category = XDFCategories.Undocumented;
        public XDFCategories XdfCategory
        {
            get { return m_category; }
            set { m_category = value; }
        }

        private XDFSubCategory m_subcategory = XDFSubCategory.Undocumented;
        public XDFSubCategory XdfSubcategory
        {
            get { return m_subcategory; }
            set { m_subcategory = value; }
        }

        private string m_helptext = string.Empty;
        public string Helptext
        {
            get { return m_helptext; }
            set { m_helptext = value; }
        }

        private string m_filename = string.Empty;
        public string Filename
        {
            get { return m_filename; }
            set { m_filename = value; }
        }

        private double _userCorrectionFactor = 1;
        public double UserCorrectionFactor
        {
            get { return _userCorrectionFactor; }
            set { _userCorrectionFactor = value; }
        }

        private double _userCorrectionOffset = 0;
        public double UserCorrectionOffset
        {
            get { return _userCorrectionOffset; }
            set { _userCorrectionOffset = value; }
        }

        private bool _useUserCorrection = false;
        public bool UseUserCorrection
        {
            get { return _useUserCorrection; }
            set { _useUserCorrection = value; }
        }
    }
}
