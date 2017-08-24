using System;
using System.Collections.Generic;
using System.Text;

namespace Trionic5Tools
{
    public class SymbolHelper
    {
        private System.Drawing.Color _color = System.Drawing.Color.Black;
        public System.Drawing.Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        int flash_start_address = 0x00000;
        public int Flash_start_address
        {
            get { return flash_start_address; }
            set { flash_start_address = value; }
        }

        int start_address = 0x00000;
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

        private bool _isSystemSymbol = false;
        public bool IsSystemSymbol
        {
            get { return _isSystemSymbol; }
            set { _isSystemSymbol = value; }
        }

        private XDFCategories m_category = XDFCategories.Undocumented;
        public XDFCategories Category
        {
            get { return m_category; }
            set { m_category = value; }
        }

        private XDFSubCategory m_subcategory = XDFSubCategory.Undocumented;
        public XDFSubCategory Subcategory
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
