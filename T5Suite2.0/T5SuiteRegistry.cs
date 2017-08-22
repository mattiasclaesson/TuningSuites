using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonSuite;

namespace T5Suite2
{
    class T5SuiteRegistry : SuiteRegistry
    {
        private const string T5Suite = "T5Suite2";

        override public string getRegistryPath()
        {
            return T5Suite;
        }
    }
}