using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace T8SuitePro
{
    public partial class frmDecodeVIN : DevExpress.XtraEditors.XtraForm
    {
        public frmDecodeVIN()
        {
            InitializeComponent();
        }

        private void textEdit1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DecodeVIN();
            }
        }

        private void DecodeVIN()
        {
            lblBody.Text = "---";
            lblCarModel.Text = "---";
            lblEngineType.Text = "---";
            lblMakeyear.Text = "---";
            lblPlant.Text = "---";
            lblSeries.Text = "---";
            lblTurbo.Text = "---";
            VINDecoder decoder = new VINDecoder();
            VINCarInfo carinfo = decoder.DecodeVINNumber(textEdit1.Text);
            lblBody.Text = carinfo.Body;
            lblCarModel.Text = carinfo.CarModel.ToString();
            lblEngineType.Text = carinfo.EngineType.ToString();
            lblMakeyear.Text = carinfo.Makeyear.ToString();
            lblPlant.Text = carinfo.PlantInfo;
            lblSeries.Text = carinfo.Series;
            lblTurbo.Text = carinfo.TurboModel.ToString();
            lblGearbox.Text = carinfo.GearboxDescription;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DecodeVIN();
            
        }

        private void frmDecodeVIN_Load(object sender, EventArgs e)
        {
            
        }

        internal void SetVinNumber(string vinnumber)
        {
            textEdit1.Text = vinnumber;
            DecodeVIN();
        }
    }
}