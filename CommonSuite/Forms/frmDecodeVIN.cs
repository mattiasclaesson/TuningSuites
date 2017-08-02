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
            textEdit1.Text = textEdit1.Text.ToUpper();
            lblBody.Text = "---";
            lblCarModel.Text = "---";
            lblEngineType.Text = "---";
            lblMakeyear.Text = "---";
            lblPlant.Text = "---";
            lblSeries.Text = "---";
            lblTurbo.Text = "---";
            lblChecksum.Text = "Not verified";
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
            char checksum;
            if (decoder.CalculateChecksum(textEdit1.Text, out checksum)) lblChecksum.Text = checksum == textEdit1.Text[8] ? "Valid" : "WRONG! Expected: " + checksum + " but found: " + textEdit1.Text[8];
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DecodeVIN();
        }

        public void SetVinNumber(string vinnumber)
        {
            textEdit1.Text = vinnumber;
            DecodeVIN();
        }
    }
}