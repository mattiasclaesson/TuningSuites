using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using T7;

namespace T7
{
    class SymbolFiller
    {
        public bool CheckAndFillCollection(SymbolCollection sc)
        {
            bool retval = false;
            // first check whether we have a "blank" file
            bool _hasSymbolNumbers = false;
            foreach (SymbolHelper sh in sc)
            {
                if (sh.Varname.StartsWith("Symbolnumber"))
                {
                    _hasSymbolNumbers = true;
                    break;
                }
            }
            // check known symbol length
            if (_hasSymbolNumbers)
            {
                // 0x1A4 = AmosCal
                // 0x150 = PurgeCal.ValveMap16
                // 0xE0 = MAFCal.WeightConstFuelMa
                // 0xC8 = ExhaustCal.fi_IgnMap
                // 0xC6 = MAPCal.cd_ThrottleMap
            }
            return retval;
        }
    }
}
