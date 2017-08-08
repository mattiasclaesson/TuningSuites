using System;
using System.Collections.Generic;
using System.Text;

namespace Trionic5Controls
{
    class RealtimeValue
    {
        private DateTime _lastUpdate = DateTime.MinValue;

        public DateTime LastUpdate
        {
            get { return _lastUpdate; }
            set { _lastUpdate = value; }
        }

        private float _value = 0;

        public float Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public void UpdateValue(DateTime lastUpdate, float value)
        {
            _value = value;
            _lastUpdate = lastUpdate;
        }
    }
}
