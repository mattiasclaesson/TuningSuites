using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trionic5Tools
{
    public class KnockMapInfo
    {
        private string _content = string.Empty;

        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }

        private int _numberOfKnocks = 0;

        public int NumberOfKnocks
        {
            get { return _numberOfKnocks; }
            set { _numberOfKnocks = value; }
        }
        private DateTime _fileDateTime = DateTime.MinValue;

        public DateTime FileDateTime
        {
            get { return _fileDateTime; }
            set { _fileDateTime = value; }
        }
        private string _fileName = string.Empty;

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        private string _fileNameNoPath = string.Empty;

        public string FileNameNoPath
        {
            get { return _fileNameNoPath; }
            set { _fileNameNoPath = value; }
        }
    }
}
