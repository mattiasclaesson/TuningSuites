using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace RealtimeGraph
{
    public class GraphLine
    {
        private int _numberOfDecimals = 0;

        public int NumberOfDecimals
        {
            get { return _numberOfDecimals; }
            set { _numberOfDecimals = value; }
        }

        private bool _lineVisible = true;

        public bool LineVisible
        {
            get { return _lineVisible; }
            set { _lineVisible = value; }
        }

        private float _currentlySelectedValue = float.MinValue;

        public float CurrentlySelectedValue
        {
            get { return _currentlySelectedValue; }
            set { _currentlySelectedValue = value; }
        }

        private Color _lineColor = Color.Red;

        public Color LineColor
        {
            get { return _lineColor; }
            set { _lineColor = value; }
        }

        private float _minrange = 0;

        public float Minrange
        {
            get { return _minrange; }
            set { _minrange = value; }
        }
        private float _maxrange = 1;

        public float Maxrange
        {
            get { return _maxrange; }
            set { _maxrange = value; }
        }

        private int _maxpoints = 2;

        public int Maxpoints
        {
            get { return _maxpoints; }
            set { _maxpoints = value; }
        }

        private GraphMeasurementCollection m_measurements = new GraphMeasurementCollection();

        public GraphMeasurementCollection Measurements
        {
            get { return m_measurements; }
            set { m_measurements = value; }
        }

        private string _symbol;

        public string Symbol
        {
            get { return _symbol; }
            set { _symbol = value; }
        }

        private string _channelName = string.Empty;

        public string ChannelName
        {
            get { return _channelName; }
            set { _channelName = value; }
        }


        private int _numberofmeasurements = 0;

        public int Numberofmeasurements
        {
            get
            {
                _numberofmeasurements = m_measurements.Count;
                return _numberofmeasurements;
            }
        }

        public void Clear()
        {
            m_measurements.Clear();
            m_measurements = new GraphMeasurementCollection();
            _numberofmeasurements = 0;
        }

        public void AddPoint(float value, DateTime timestamp, float minrange, float maxrange, Color linecolor)
        {
            _lineColor = linecolor;
            _minrange = minrange;
            _maxrange = maxrange;
            // check bounds
            if (m_measurements.Count >= _maxpoints)
            {
                // re-scale
                _maxpoints++;
                /*for (int t = 0; t < _maxpoints -1 ; t++)
                {
                    //Console.WriteLine("Setting index " + t.ToString() + " with value " + m_measurements[t].Value.ToString("F2") + " to " + m_measurements[t + 1].Value.ToString("F2"));
                    //m_measurements[t] = m_measurements[t + 1];
                    m_measurements[t].Value = m_measurements[t+1].Value;
                    m_measurements[t].Timestamp = m_measurements[t+1].Timestamp;
                }
                m_measurements[_maxpoints - 1].Value = value;
                m_measurements[_maxpoints - 1].Timestamp = timestamp;*/
                
                GraphMeasurement measurement = new GraphMeasurement();
                measurement.Symbol = _symbol;
                measurement.Value = value;
                measurement.Timestamp = timestamp;
                m_measurements.Add(measurement);

            }
            else
            {
                GraphMeasurement measurement = new GraphMeasurement();
                measurement.Symbol = _symbol;
                measurement.Value = value;
                measurement.Timestamp = timestamp;
                m_measurements.Add(measurement);
            }
        }
    }
}
