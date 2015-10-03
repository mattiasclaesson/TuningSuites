using System;
using System.Collections.Generic;
using System.Text;
using T8SuitePro;

namespace T8SuitePro
{
    class SymbolTranslator
    {
        public string TranslateSymbolToHelpText(string symbolname, out string helptext, out XDFCategories category, out XDFSubCategory subcategory)
        {
            if (symbolname.EndsWith("!")) symbolname = symbolname.Substring(0, symbolname.Length - 1);
            helptext = "";
            category = XDFCategories.Undocumented;
            subcategory = XDFSubCategory.Undocumented;
            string description = helptext = SymbolDictionary.GetSymbolDescription(symbolname);
            return description;
        }
    }
}
    