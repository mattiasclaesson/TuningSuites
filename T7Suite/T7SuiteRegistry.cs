using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonSuite;

namespace T7
{
    class T7SuiteRegistry : SuiteRegistry
    {
        private const string T7SuitePro = "T7SuitePro";

        override public string getRegistryPath()
        {
            return T7SuitePro;
        }
    }
}
