using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T8SuitePro
{
    public class LogFilter
    {
        public enum MathType : int
        {
            GreaterThan,
            SmallerThan,
            Equals
        }

        private int _index = 0;

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        private bool _active = false;

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        private MathType _type = MathType.GreaterThan;

        public MathType Type
        {
            get { return _type; }
            set { _type = value; }
        }
        private float _value = 0;

        public float Value
        {
            get { return _value; }
            set { _value = value; }
        }
        private string _symbol = "";

        public string Symbol
        {
            get { return _symbol; }
            set { _symbol = value; }
        }

    }
}
