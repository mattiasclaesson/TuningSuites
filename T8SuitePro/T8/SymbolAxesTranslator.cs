using System;
using System.Collections.Generic;
using System.Text;

namespace T8SuitePro
{
    class SymbolAxesTranslator
    {
       

        public bool GetAxisSymbols(string symbolname, out string x_axis, out string y_axis, out string x_axis_description, out string y_axis_description, out string z_axis_description)
        {
            bool retval = false;
            x_axis = SymbolDictionary.GetSymbolXAxis(symbolname);
            y_axis = SymbolDictionary.GetSymbolYAxis(symbolname);
            x_axis_description = "";
            y_axis_description = "";

            if (x_axis != "")
                x_axis_description = SymbolDictionary.GetSymbolUnitOfMeasure(x_axis);
            if (x_axis != "")
                y_axis_description = SymbolDictionary.GetSymbolUnitOfMeasure(y_axis);
            z_axis_description = SymbolDictionary.GetSymbolUnitOfMeasure(symbolname);

            if (y_axis != "") retval = true;
            return retval;
        }

        public string GetXaxisSymbol(string symbolname)
        {
            string retval;
            retval = SymbolDictionary.GetSymbolXAxis(symbolname);
            if (retval == "")
                retval = string.Empty;
            
            return retval;
        }

        public string GetYaxisSymbol(string symbolname)
        {
            string retval;
            retval = SymbolDictionary.GetSymbolYAxis(symbolname);
            if (retval == "")
                retval = string.Empty;

            return retval;
        }
    }
}
