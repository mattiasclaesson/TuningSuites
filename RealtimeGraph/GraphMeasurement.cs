using System;
using System.Collections.Generic;
using System.Text;

namespace RealtimeGraph
{
    public class GraphMeasurement
    {
        private string _symbol = string.Empty;

        public string Symbol
        {
            get { return _symbol; }
            set { _symbol = value; }
        }

        private float _value = 0F;

        public float Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private DateTime _timestamp = DateTime.Now;

        public DateTime Timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }
            
    }
}
