using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using Trionic5Tools;

namespace T5Suite2
{
    public partial class frmPartnumberLookup : DevExpress.XtraEditors.XtraForm
    {
        private bool m_open_File = false;

        private bool m_compare_File = false;

        public bool Compare_File
        {
            get { return m_compare_File; }
            set { m_compare_File = value; }
        }

        public bool Open_File
        {
            get { return m_open_File; }
            set { m_open_File = value; }
        }

        public frmPartnumberLookup()
        {
            InitializeComponent();
        }

        private void ConvertPartNumber()
        {
            PartNumberConverter pnc = new PartNumberConverter();
            ECUInformation ecuinfo = pnc.GetECUInfo(buttonEdit1.Text, "");
            lblBaseBoost.Text = "---";
            lblCarModel.Text = "---";
            lblEngineType.Text = "---";
            lblMaxBoostAUT.Text = "---";
            lblMaxBoostManual.Text = "---";
            lblPower.Text = "---";
            lblStageI.Text = "---";
            lblStageII.Text = "---";
            lblStageIII.Text = "---";
            lblTorque.Text = "---";
            checkEdit1.Checked = false;
            checkEdit2.Checked = false;
            checkEdit3.Checked = false;
            checkEdit4.Checked = false;
            checkEdit5.Checked = false;
            checkEdit6.Checked = false;
            lblRegion.Text = "---";
            lblMYs.Text = "---";
            lblEcuType.Visible = false;
            lblEcuType.Text = "---";

            if (ecuinfo.Valid)
            {
                lblBaseBoost.Text = ecuinfo.Baseboost.ToString() + " bar";
                lblCarModel.Text = ecuinfo.Carmodel.ToString();
                lblEngineType.Text = ecuinfo.Enginetype.ToString();
                lblMaxBoostAUT.Text = ecuinfo.Max_stock_boost_automatic.ToString() + " bar";
                lblMaxBoostManual.Text = ecuinfo.Max_stock_boost_manual.ToString() + " bar";
                lblPower.Text = ecuinfo.Bhp.ToString() + " bhp";
                lblStageI.Text = ecuinfo.Stage1boost.ToString() + " bar";
                lblStageII.Text = ecuinfo.Stage2boost.ToString() + " bar";
                lblStageIII.Text = ecuinfo.Stage3boost.ToString() + " bar";
                lblEcuType.Text = ecuinfo.Ecutype;
                if (lblEcuType.Text == "T5.2") lblEcuType.Visible = true;
                if (ecuinfo.MakeYearFrom != ecuinfo.MakeYearUpto)
                {
                    lblMYs.Text = ecuinfo.MakeYearFrom.ToString() + "-" + ecuinfo.MakeYearUpto.ToString();
                }
                else
                {
                    lblMYs.Text = ecuinfo.MakeYearFrom.ToString();
                }
                lblRegion.Text = ecuinfo.Region;
                checkEdit6.Checked = ecuinfo.HighAltitude;
                
                if (ecuinfo.Is2point3liter)
                {
                    checkEdit1.Checked = false;
                    checkEdit2.Checked = true;
                }
                else
                {
                    checkEdit1.Checked = true;
                    checkEdit2.Checked = false;
                }
                if (ecuinfo.Isturbo) checkEdit4.Checked = true;
                if (ecuinfo.Isfpt)
                {
                    checkEdit5.Checked = true;
                    checkEdit4.Checked = true;
                }
                if (ecuinfo.Isaero)
                {
                    checkEdit3.Checked = true;
                    checkEdit4.Checked = true;
                    checkEdit5.Checked = true;
                }

                lblTorque.Text = ecuinfo.Torque.ToString() + " Nm";
                if (System.IO.File.Exists(Path.Combine(Application.StartupPath, "Binaries\\" + buttonEdit1.Text + ".BIN")))
                {
                    simpleButton2.Enabled = true;
                    simpleButton3.Enabled = true;
                }
                else
                {
                    simpleButton2.Enabled = false;
                    simpleButton3.Enabled = false;
                }
            }
            else
            {
                MessageBox.Show("The entered partnumber was not recognized by T5Suite");
            }
        }

        private void buttonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            //ConvertPartNumber();
            frmPartNumberList pnl = new frmPartNumberList();
            pnl.ShowDialog();
            if (pnl.Selectedpartnumber != null)
            {
                if (pnl.Selectedpartnumber != string.Empty)
                {
                    buttonEdit1.Text = pnl.Selectedpartnumber;
                }
            }
            if (buttonEdit1.Text != "")
            {
                ConvertPartNumber();
            }
            else
            {
                simpleButton2.Enabled = false;
                simpleButton3.Enabled = false;
            }

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonEdit1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ConvertPartNumber();
            }
        }

        internal void LookUpPartnumber(string p)
        {
            buttonEdit1.Text = p;
            ConvertPartNumber();
        }

        public string GetFileToOpen()
        {
            string retval = string.Empty;
            if (buttonEdit1.Text != string.Empty)
            {
                if (System.IO.File.Exists(Path.Combine(Application.StartupPath, "Binaries\\" + buttonEdit1.Text + ".BIN")))
                {
                    retval = Path.Combine(Application.StartupPath, "Binaries\\" + buttonEdit1.Text + ".BIN");
                }
            }
            return retval;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            m_open_File = true;
            this.Close();
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            m_compare_File = true;
            this.Close();
        }

        internal void DisableOpenButtons()
        {
            // for lookup only
            simpleButton2.Visible = false;
            simpleButton3.Visible = false;
        }
    }
}