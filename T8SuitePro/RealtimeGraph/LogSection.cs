using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealtimeGraph
{
    public class LogSection
    {
        private string _description;

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private DateTime _startDateTime;

        public DateTime StartDateTime
        {
            get { return _startDateTime; }
            set { _startDateTime = value; }
        }
        private DateTime _endDateTime;

        public DateTime EndDateTime
        {
            get { return _endDateTime; }
            set { _endDateTime = value; }
        }
        private TimeSpan _duration;

        public TimeSpan Duration
        {
            get { return _duration; }
            set
            {
                _duration = value;
                _description = _startDateTime.ToString("HH:mm:ss") + " - " + _endDateTime.ToString("HH:mm:ss") + " [" + _duration.ToString() + "]";
            }
        }
    }
}
