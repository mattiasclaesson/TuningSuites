using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace T5Suite2
{
    public partial class frmPeMicroParameters : DevExpress.XtraEditors.XtraForm
    {
        public frmPeMicroParameters()
        {
            InitializeComponent();
        }

        public string ECUWriteAMDFile
        {
            get
            {
                return mruEdit2.Text;
            }
            set
            {
                mruEdit2.Text = value;
            }
        }

        public string ECUWriteIntelFile
        {
            get
            {
                return mruEdit4.Text;
            }
            set
            {
                mruEdit4.Text = value;
            }
        }

        public string ECUWriteAtmelFile
        {
            get
            {
                return mruEdit5.Text;
            }
            set
            {
                mruEdit5.Text = value;
            }
        }

        public string ECUBruteforceEraseFile
        {
            get
            {
                return mruEdit6.Text;
            }
            set
            {
                mruEdit6.Text = value;
            }
        }

        public string ECUReadFile
        {
            get
            {
                return mruEdit1.Text;
            }
            set
            {
                mruEdit1.Text = value;
            }
        }

        public string TargetECUReadFile
        {
            get
            {
                return mruEdit3.Text;
            }
            set
            {
                mruEdit3.Text = value;
            }
        }

        private void mruEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis)
            {
                // bestand zoeken
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    mruEdit1.Text = openFileDialog1.FileName;
                    mruEdit1.Properties.Items.Add(openFileDialog1.FileName);
                }
            }
        }

        private void mruEdit2_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis)
            {
                // bestand zoeken
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    mruEdit2.Text = openFileDialog1.FileName;
                    mruEdit2.Properties.Items.Add(openFileDialog1.FileName);
                }
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void mruEdit3_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis)
            {
                // bestand zoeken
                openFileDialog2.CheckFileExists = false;
                if (openFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    mruEdit3.Text = openFileDialog2.FileName;
                    mruEdit3.Properties.Items.Add(openFileDialog2.FileName);
                }
            }
 
        }

        private void mruEdit4_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis)
            {
                // bestand zoeken
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    mruEdit4.Text = openFileDialog1.FileName;
                    mruEdit4.Properties.Items.Add(openFileDialog1.FileName);
                }
            }
        }

        private void mruEdit5_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis)
            {
                // bestand zoeken
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    mruEdit5.Text = openFileDialog1.FileName;
                    mruEdit5.Properties.Items.Add(openFileDialog1.FileName);
                }
            }
        }

        private void mruEdit6_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis)
            {
                // bestand zoeken
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    mruEdit6.Text = openFileDialog1.FileName;
                    mruEdit6.Properties.Items.Add(openFileDialog1.FileName);
                }
            }
        }
    }
}