using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace T8SuitePro.DataControllers
{
    public abstract class BaseDatacontroller
    {
        protected static Logger mLogger;

        public BaseDatacontroller()
        {
              mLogger = LogManager.GetCurrentClassLogger();

        }

        protected abstract void Data();

        protected abstract void Configure();
   
    }
}
