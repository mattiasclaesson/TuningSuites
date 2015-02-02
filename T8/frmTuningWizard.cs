using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace T8SuitePro
{
    public partial class frmTuningWizard : Form
    {
        string m_currentfile = "";
        Form1 parent;
        public frmTuningWizard(Form1 inParent, string in_m_currentfile)
        {
            InitializeComponent();
            parent = inParent;
            m_currentfile = in_m_currentfile;
        }
    }
}
