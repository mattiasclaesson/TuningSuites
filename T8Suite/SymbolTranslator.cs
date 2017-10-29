using System;
using System.Collections.Generic;

namespace T8SuitePro
{
    class SymbolTranslator
    {
        public static string ToDescription(string symbolname)
        {
            if (symbolname.EndsWith("!"))
            {
                symbolname = symbolname.Substring(0, symbolname.Length - 1);
            }
            string description = SymbolDictionary.GetSymbolDescription(symbolname);

            return description;
        }
    }
}
    