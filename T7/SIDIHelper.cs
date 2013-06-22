using System;
using System.Collections.Generic;
using System.Text;

namespace T7
{
    class SIDIHelper
    {
        private string _value = string.Empty;

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private string _symbol = string.Empty;

        public string Symbol
        {
            get { return _symbol; }
            set { _symbol = value; }
        }

        private string _info = string.Empty;

        public string Info
        {
            get { return _info; }
            set { _info = value; }
        }

        private bool _isReadOnly = false;

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set { _isReadOnly = value; }
        }

        private string _FoundT7Symbol = string.Empty;

        public string FoundT7Symbol
        {
            get { return _FoundT7Symbol; }
            set { _FoundT7Symbol = value; }
        }


        private string _T7Symbol = string.Empty;

        public string T7Symbol
        {
            get { return _T7Symbol; }
            set { _T7Symbol = value; }
        }

        private int _mode = 0;

        public int Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                switch (_mode)
                {
                    case 0:
                        _ModeDescr = "Adaptation";
                        break;
                    case 1:
                        _ModeDescr = "Exhaust";
                        break;
                    case 2:
                        _ModeDescr = "TCM FUEL/DTI";
                        break;
                    case 3:
                        _ModeDescr = "TCM GSI/CSLU";
                        break;
                    case 4:
                        _ModeDescr = "TCM TRQ/SPEED";
                        break;
                    case 5:
                        _ModeDescr = "ESP";
                        break;
                    case 6:
                        _ModeDescr = "Purge";
                        break;
                    case 99:
                        _ModeDescr = "Generic";
                        break;
                }
            }
        }

        private string _ModeDescr = string.Empty;

        public string ModeDescr
        {
            get { return _ModeDescr; }
            set { _ModeDescr = value; }
        }

        private int _addressSRAM = 0;

        public int AddressSRAM
        {
            get { return _addressSRAM; }
            set { _addressSRAM = value; }
        }

    }
}
