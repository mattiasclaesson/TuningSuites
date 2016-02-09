using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonSuite;

namespace T8SuitePro
{
    class T8SuiteRegistry : SuiteRegistry
    {
        private const string T8SuitePro = "T8SuitePro";

        override public string getRegistryPath()
        {
            return T8SuitePro;
        }
    }
}
