using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CommonSuite
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
            textEdit1.Text = textEdit1.Text.ToUpper(); // Make sure it is all capitol letters
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
            lblTurbo.Text = carinfo.TurboModel.ToString().Replace("_","-");
            lblGearbox.Text = carinfo.GearboxDescription;
            lblChecksum.Text = carinfo.IsChecksumValid ? "Valid" : "WRONG!";
            btnFixChecksum.Enabled = !carinfo.IsChecksumValid;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DecodeVIN();
        }

        public void SetVinNumber(string vinnumber)
        {
            textEdit1.Text = vinnumber; // Make sure it is all capitol letters
            DecodeVIN();
        }

        private void btnFixChecksum_Click(object sender, EventArgs e)
        {
            VINDecoder decoder = new VINDecoder();
            char checksum;
            if (decoder.CalculateChecksum(textEdit1.Text, out checksum))
            {
                System.Text.StringBuilder strBuilder = new System.Text.StringBuilder(textEdit1.Text);
                strBuilder[8] = checksum;
                textEdit1.Text = strBuilder.ToString();
                DecodeVIN();
            } 
        }

    }
}