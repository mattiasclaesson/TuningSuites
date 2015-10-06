using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trionic5Tools
{
    public class IgnitionMeasurement
    {
        private double _ignitionValue = 0;

        public double IgnitionValue
        {
            get { return _ignitionValue; }
            set { _ignitionValue = value; }
        }

        private bool _isKnock = false;

        public bool IsKnock
        {
            get
            {
                return _isKnock;
            }
            set
            {
                _isKnock = value;
            }
        }

    }
}
