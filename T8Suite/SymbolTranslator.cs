using System;
using System.Collections.Generic;

namespace T8SuitePro
{
    class SymbolTranslator
    {
        public static string ToDescription(string symbolname)
        {
            string description = SymbolDictionary.GetSymbolDescription(symbolname);

            return description;
        }
    }
}
    