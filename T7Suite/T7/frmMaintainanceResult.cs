using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace T7
{
    public partial class frmMaintainanceResult : DevExpress.XtraEditors.XtraForm
    {
        public frmMaintainanceResult()
        {
            InitializeComponent();
        }

        public string commReport
        {
            set
            {
                char[] sep = new char[1];
                sep.SetValue('\n', 0);
                string[] vals = value.Split(sep);
                foreach (string s in vals)
                {
                    string rs = s;
                    rs = rs.Replace("\n", "");
                    rs = rs.Replace("\r", "");
                    if (rs != string.Empty)
                    {
                        while (rs.Contains("  ")) rs = rs.Replace("  ", " ");
                        listBoxControl1.Items.Add(rs);
                    }
                }

                
            }
        }
    }
}