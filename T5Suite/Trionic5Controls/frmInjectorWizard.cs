using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Trionic5Tools;

namespace Trionic5Controls
{
    public partial class frmInjectorWizard : DevExpress.XtraEditors.XtraForm
    {
        public frmInjectorWizard()
        {
            InitializeComponent();
        }

        private float _crankfactor = 9;
        private float _oricrankfactor = 9;

        public float Crankfactor
        {
            get { return _crankfactor; }
            set
            {
                _crankfactor = value;
                _oricrankfactor = _crankfactor;
                spinEdit2.Value = (decimal)_crankfactor;
                labelControl18.Text = _crankfactor.ToString("F2");
            }
        }

        public byte[] GetCrankFactor()
        {
            byte[] retval = new byte[2];
            float val = _crankfactor / 0.004F;
            int ival = Convert.ToInt32(val);
            byte b1 = (byte)(ival / 256);
            byte b2 = (byte)(ival - (int)b1 * 256);
            retval[0] = b1;
            retval[1] = b2;
            return retval;
        }

        public byte[] GetBatteryCorrectionMap()
        {
            byte[] retval = new byte[22];
            int bcount = 0;
            for (int i = 0; i < 11; i++)
            {
                float val = GetBatteryCorrection(i) / 0.004F;
                int ival = Convert.ToInt32(val);
                byte b1 = (byte)(ival / 256);
                byte b2 = (byte)(ival - (int)b1 * 256);
                retval[bcount++] = b1;
                retval[bcount++] = b2;
            }
            return retval;

        }

        public float GetBatteryCorrection(int index)
        {
            float retval = 0;
            switch (index)
            {
                case 0:
                    retval = (float)Convert.ToDouble(textEdit22.Text);
                    break;
                case 1:
                    retval = (float)Convert.ToDouble(textEdit21.Text);
                    break;
                case 2:
                    retval = (float)Convert.ToDouble(textEdit20.Text);
                    break;
                case 3:
                    retval = (float)Convert.ToDouble(textEdit19.Text);
                    break;
                case 4:
                    retval = (float)Convert.ToDouble(textEdit18.Text);
                    break;
                case 5:
                    retval = (float)Convert.ToDouble(textEdit17.Text);
                    break;
                case 6:
                    retval = (float)Convert.ToDouble(textEdit16.Text);
                    break;
                case 7:
                    retval = (float)Convert.ToDouble(textEdit15.Text);
                    break;
                case 8:
                    retval = (float)Convert.ToDouble(textEdit14.Text);
                    break;
                case 9:
                    retval = (float)Convert.ToDouble(textEdit13.Text);
                    break;
                case 10:
                    retval = (float)Convert.ToDouble(textEdit12.Text);
                    break;

            }
            return retval;
        }

        //private byte[] batt_korr_volt;
        private int[] batt_korr_map;

        private string ConvertBattKorrValue(int value)
        {
            float fval = (float)value;
            fval *= 0.004F;
            return fval.ToString("F3");
        }

        public int[] Batt_korr_map
        {
            get { return batt_korr_map; }
            set
            {
                batt_korr_map = value;
                // SET VALUES correctly
                if (batt_korr_map.Length == 11)
                {
                    // <GS-14042010> add correction factors
                    textEdit1.Text = ConvertBattKorrValue(batt_korr_map[10]);
                    textEdit2.Text = ConvertBattKorrValue(batt_korr_map[9]);
                    textEdit3.Text = ConvertBattKorrValue(batt_korr_map[8]);
                    textEdit4.Text = ConvertBattKorrValue(batt_korr_map[7]);
                    textEdit5.Text = ConvertBattKorrValue(batt_korr_map[6]);
                    textEdit6.Text = ConvertBattKorrValue(batt_korr_map[5]);
                    textEdit7.Text = ConvertBattKorrValue(batt_korr_map[4]);
                    textEdit8.Text = ConvertBattKorrValue(batt_korr_map[3]);
                    textEdit9.Text = ConvertBattKorrValue(batt_korr_map[2]);
                    textEdit10.Text = ConvertBattKorrValue(batt_korr_map[1]);
                    textEdit11.Text = ConvertBattKorrValue(batt_korr_map[0]);
                }
            }
        }


        private int _injectorConstant = 21;
        private int _oriinjectorConstant = 21;

        public int InjectorConstant
        {
            get { return _injectorConstant; }
            set
            {
                _injectorConstant = value;
                _oriinjectorConstant = _injectorConstant;
                labelControl3.Text = _injectorConstant.ToString();
                spinEdit1.EditValue = _injectorConstant;
            }
        }

        private bool _progChanges = false;

        private InjectorType _injectorType = InjectorType.Stock;
        private InjectorType _oriinjectorType = InjectorType.Stock;

        public InjectorType InjectorType
        {
            get { return _injectorType; }
            set
            {
                _injectorType = value;
                _oriinjectorType = _injectorType;
                _progChanges = true;
                comboBoxEdit1.SelectedIndex = (int)_injectorType;
                SetInjectorBatteryCorrectionMap(_injectorType);
                SetCrankFactor(_injectorType);
                _progChanges = false;
            }
        }

        private void SetInjectorBatteryCorrectionMap(InjectorType injectorType)
        {
            float tempvalue = 0;
            switch (injectorType)
            {
                case InjectorType.Stock:
                case InjectorType.Siemens875Dekas:
                case InjectorType.Siemens1000cc:
                    tempvalue = 3.73F;
                    textEdit12.Text = tempvalue.ToString("F3");  // 5 volt
                    textEdit13.Text = tempvalue.ToString("F3");  // 6 volt
                    textEdit14.Text = tempvalue.ToString("F3");  // 7 volt
                    tempvalue = 2.32F;
                    textEdit15.Text = tempvalue.ToString("F3");  // 8 volt
                    tempvalue = 1.85F;
                    textEdit16.Text = tempvalue.ToString("F3");  // 9 volt
                    tempvalue = 1.50F;
                    textEdit17.Text = tempvalue.ToString("F3");  // 10 volt
                    tempvalue = 1.28F;
                    textEdit18.Text = tempvalue.ToString("F3");  // 11 volt
                    tempvalue = 0.94F;
                    textEdit19.Text = tempvalue.ToString("F3");  // 12 volt
                    tempvalue = 0.78F;
                    textEdit20.Text = tempvalue.ToString("F3");  // 13 volt
                    tempvalue = 0.77F;
                    textEdit21.Text = tempvalue.ToString("F3");  // 14 volt
                    tempvalue = 0.59F;
                    textEdit22.Text = tempvalue.ToString("F3");  // 15 volt
                    break;
                case InjectorType.GreenGiants:
                    tempvalue = 5.45F;
                    textEdit12.Text = tempvalue.ToString("F3");  // 5 volt
                    tempvalue = 4.142F;
                    textEdit13.Text = tempvalue.ToString("F3");  // 6 volt
                    tempvalue = 3.216F;
                    textEdit14.Text = tempvalue.ToString("F3");  // 7 volt
                    tempvalue = 2.545F;
                    textEdit15.Text = tempvalue.ToString("F3");  // 8 volt
                    tempvalue = 2.102F;
                    textEdit16.Text = tempvalue.ToString("F3");  // 9 volt
                    tempvalue = 1.768F;
                    textEdit17.Text = tempvalue.ToString("F3");  // 10 volt
                    tempvalue = 1.521F;
                    textEdit18.Text = tempvalue.ToString("F3");  // 11 volt
                    tempvalue = 1.308F;
                    textEdit19.Text = tempvalue.ToString("F3");  // 12 volt
                    tempvalue = 1.15F;
                    textEdit20.Text = tempvalue.ToString("F3");  // 13 volt
                    tempvalue = 1.003F;
                    textEdit21.Text = tempvalue.ToString("F3");  // 14 volt
                    tempvalue = 0.894F;
                    textEdit22.Text = tempvalue.ToString("F3");  // 15 volt
                    break;
                case InjectorType.Siemens630Dekas:
                    tempvalue = 3.6F;
                    textEdit12.Text = tempvalue.ToString("F3");  // 5 volt
                    tempvalue = 2.74F;
                    textEdit13.Text = tempvalue.ToString("F3");  // 6 volt
                    tempvalue = 2.023F;
                    textEdit14.Text = tempvalue.ToString("F3");  // 7 volt
                    tempvalue = 1.524F;
                    textEdit15.Text = tempvalue.ToString("F3");  // 8 volt
                    tempvalue = 1.208F;
                    textEdit16.Text = tempvalue.ToString("F3");  // 9 volt
                    tempvalue = 0.974F;
                    textEdit17.Text = tempvalue.ToString("F3");  // 10 volt
                    tempvalue = 0.802F;
                    textEdit18.Text = tempvalue.ToString("F3");  // 11 volt
                    tempvalue = 0.673F;
                    textEdit19.Text = tempvalue.ToString("F3");  // 12 volt
                    tempvalue = 0.548F;
                    textEdit20.Text = tempvalue.ToString("F3");  // 13 volt
                    tempvalue = 0.433F;
                    textEdit21.Text = tempvalue.ToString("F3");  // 14 volt
                    tempvalue = 0.33F;
                    textEdit22.Text = tempvalue.ToString("F3");  // 15 volt
                    break;

            }
            // set battery correction voltage maps
            /*
             * Siemens deka 875         Siemens Deka 630      stock
             * Batt_korr_table
                15v = 0.62              15v=0.17ms          0.59
                14v = 0.73              14v=0.28ms          0.77
                13v = 0.85              13v=0.38ms          0.78
                12v = 1.00              12v=0.50ms          0.94
                11v = 1.20              11v=0.64ms          1.28
                10v = 1.46              10v=0.83ms          1.50
            */
        }

        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // user change injector type
            if (!_progChanges)
            {
                // calculate the new proposed injector constant
                _injectorType = (InjectorType)comboBoxEdit1.SelectedIndex;
                if (_oriinjectorType == _injectorType)
                {
                    _injectorConstant = _oriinjectorConstant;
                }
                else
                {
                    Trionic5Tuner _tun = new Trionic5Tuner();
                    int diffInInjConstant = _tun.DetermineDifferenceInInjectorConstant(_oriinjectorType, _injectorType);
                    //<GS-04082010> the diff percentage seemed to cause trouble!
                    /*
                    float percentageToCompensate = _tun.DetermineDifferenceInInjectorConstantPercentage(_oriinjectorType, _injectorType);
                    // substract difference
                    _injectorConstant = (int)Math.Round(((float)_oriinjectorConstant * percentageToCompensate));
                    _injectorConstant++;*/
                    _injectorConstant = _oriinjectorConstant - diffInInjConstant;
                }
                //<GS-17052010> _injectorConstant = _oriinjectorConstant - diffInInjConstant;
                //labelControl3.Text = _injectorConstant.ToString();
                spinEdit1.EditValue = _injectorConstant;
                // set the correction factor for the selected injectortype
                SetInjectorBatteryCorrectionMap(_injectorType);
                SetCrankFactor(_injectorType);
            }
        }
        private void SetCrankFactor(InjectorType _type)
        {
            switch (_type)
            {
                case InjectorType.Stock:
                case InjectorType.GreenGiants:
                    spinEdit2.EditValue = (decimal)9;
                    break;
                case InjectorType.Siemens630Dekas:
                    spinEdit2.EditValue = (decimal)6;
                    break;
                case InjectorType.Siemens875Dekas:
                    spinEdit2.EditValue = (decimal)4;
                    break;
                case InjectorType.Siemens1000cc:
                    spinEdit2.EditValue = (decimal)3.5;
                    break;
            }
        }

        private void wizardControl1_FinishClick(object sender, CancelEventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void wizardControl1_CancelClick(object sender, CancelEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void spinEdit2_ValueChanged(object sender, EventArgs e)
        {
            _crankfactor = (float)Convert.ToDouble(spinEdit2.EditValue);
        }

        private void spinEdit1_ValueChanged(object sender, EventArgs e)
        {
            _injectorConstant = Convert.ToInt32(spinEdit1.EditValue);
        }

        private void frmInjectorWizard_Load(object sender, EventArgs e)
        {

        }

    }
}