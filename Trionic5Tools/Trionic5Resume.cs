using System;
using System.Collections.Generic;
using System.Text;

namespace Trionic5Tools
{
    public class Trionic5Resume
    {
        private System.Data.DataTable resumeTuning = new System.Data.DataTable();

        public System.Data.DataTable ResumeTuning
        {
            get { return resumeTuning; }
            set { resumeTuning = value; }
        }

        public void AddToResumeTable(string description)
        {
            if (resumeTuning != null)
            {
                if (resumeTuning.Columns.Count == 1)
                {
                    resumeTuning.Rows.Add(description);
                }
            }
        }

        public Trionic5Resume()
        {
            resumeTuning = new System.Data.DataTable();
            resumeTuning.Columns.Add("Description");
        }
    }
}
