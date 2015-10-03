using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ProCharts
{
    public class ChannelHelper
    {
        private PointCollection m_pointCollection = new PointCollection();

        public PointCollection PointCollection
        {
            get { return m_pointCollection; }
            set { m_pointCollection = value; }
        }

        private string m_channelName = string.Empty;

        public string ChannelName
        {
            get { return m_channelName; }
            set { m_channelName = value; }
        }

        private Color m_channelColor = Color.White;

        public Color ChannelColor
        {
            get { return m_channelColor; }
            set { m_channelColor = value; }
        }

        private float m_minValue = 0;

        public float MinValue
        {
            get { return m_minValue; }
            set { m_minValue = value; }
        }
        private float m_maxValue = 100;

        public float MaxValue
        {
            get { return m_maxValue; }
            set { m_maxValue = value; }
        }

    }
}
