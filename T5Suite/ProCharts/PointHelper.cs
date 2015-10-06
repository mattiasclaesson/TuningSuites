using System;
using System.Collections.Generic;
using System.Text;

namespace ProCharts
{
    public class PointHelper
    {
        private string m_channelname = string.Empty;

        public string Channelname
        {
            get { return m_channelname; }
            set { m_channelname = value; }
        }

        private float m_pointvalue = 0;

        public float Pointvalue
        {
            get { return m_pointvalue; }
            set { m_pointvalue = value; }
        }

        private DateTime m_dateTimeValue = DateTime.MinValue;

        public DateTime DateTimeValue
        {
            get { return m_dateTimeValue; }
            set { m_dateTimeValue = value; }
        }
    }
}
