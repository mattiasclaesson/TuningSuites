using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Trionic5Tools;
using System.IO;

namespace T5Suite2
{
    public partial class frmFreeTuneSettings : DevExpress.XtraEditors.XtraForm
    {
        public frmFreeTuneSettings()
        {
            InitializeComponent();
            radioGroup1.EditValue = 0;
        }

        public void SetTurboType(TurboType turbo)
        {
            comboBoxEdit3.SelectedIndex = (int)turbo;
        }

        public void SetInjectorType(InjectorType injectors)
        {
            comboBoxEdit2.SelectedIndex = (int)injectors;
        }

        public void SetMapSensorType(MapSensorType mapsensor)
        {
            comboBoxEdit1.SelectedIndex = (int)mapsensor;
        }

        public void SetRPMLimiter(int rpmlimit)
        {
            spinEdit3.EditValue = rpmlimit*10;
        }

        public int GetRPMLimiter()
        {
            return Convert.ToInt32(spinEdit3.EditValue)/10;
        }

        public void SetKnockTime(int knockTime)
        {
            spinEdit4.EditValue = knockTime;
        }

        public int GetKnockTime()
        {
            return Convert.ToInt32(spinEdit4.EditValue);
        }

        public BPCType GetBCVType()
        {
            return (BPCType)comboBoxEdit4.SelectedIndex;
        }

        public void SetBPCType(BPCType valve)
        {
            comboBoxEdit4.SelectedIndex = (int)valve;
        }

        public TurboType GetTurboType()
        {
            return (TurboType)comboBoxEdit3.SelectedIndex;
        }

        public InjectorType GetInjectorType()
        {
            return (InjectorType)comboBoxEdit2.SelectedIndex;
        }

        public MapSensorType GetMapSensorType()
        {
            return (MapSensorType)comboBoxEdit1.SelectedIndex;
        }

        public int GetPeakTorque()
        {
            return Convert.ToInt32(spinEdit1.EditValue);
        }

        public double GetPeakBoost()
        {
            return Convert.ToDouble(spinEdit2.EditValue);
        }

        public bool IsTorqueBased
        {
            get
            {
                if (Convert.ToInt32(radioGroup1.EditValue) == 0)
                {
                    return true;
                }
                return false;
            }
        }

        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // set max torque achievable (400Nm)
            UpdateMaxima((TurboType)comboBoxEdit3.SelectedIndex, (InjectorType)comboBoxEdit2.SelectedIndex, (MapSensorType)comboBoxEdit1.SelectedIndex);
        }


        private void UpdateMaxima(TurboType turbo, InjectorType injectors, MapSensorType mapSensor)
        {
            switch (mapSensor)
            {
                case MapSensorType.MapSensor25:
                    // max achievable 400Nm / 1.45 bar
                    if (injectors == InjectorType.Stock)
                    {
                        spinEdit1.Properties.MaxValue = 400;
                        spinEdit2.Properties.MaxValue = (decimal)1.30;
                    }
                    else
                    {
                        spinEdit1.Properties.MaxValue = 450;
                        spinEdit2.Properties.MaxValue = (decimal)1.45;
                    }
                    break;
                case MapSensorType.MapSensor30:
                    switch (injectors)
                    {
                        case InjectorType.Stock:
                            spinEdit1.Properties.MaxValue = 400;
                            spinEdit2.Properties.MaxValue = (decimal)1.30;
                            break;
                        case InjectorType.GreenGiants:
                            switch (turbo)
                            {
                                case TurboType.Stock:
                                case TurboType.TD0415T:
                                    spinEdit1.Properties.MaxValue = 450;
                                    spinEdit2.Properties.MaxValue = (decimal)1.45;
                                    break;
                                case TurboType.GT28BB:
                                case TurboType.GT28RS:
                                case TurboType.TD0419T:
                                    spinEdit1.Properties.MaxValue = 550;
                                    spinEdit2.Properties.MaxValue = (decimal)1.70;
                                    break;
                                case TurboType.GT3071R:
                                case TurboType.HX35w:
                                    spinEdit1.Properties.MaxValue = 550;
                                    spinEdit2.Properties.MaxValue = (decimal)1.50;
                                    break;
                                case TurboType.HX40w:
                                case TurboType.S400SX371:
                                    spinEdit1.Properties.MaxValue = 550;
                                    spinEdit2.Properties.MaxValue = (decimal)1.40;
                                    break;
                            }
                            break;
                        case InjectorType.Siemens630Dekas:
                            // 3.0 bar sensor, 630cc injectors -> max 600Nm
                            switch (turbo)
                            {
                                case TurboType.Stock:
                                case TurboType.TD0415T:
                                    spinEdit1.Properties.MaxValue = 450;
                                    spinEdit2.Properties.MaxValue = (decimal)1.45;
                                    break;
                                case TurboType.GT28BB:
                                case TurboType.GT28RS:
                                case TurboType.TD0419T:
                                    spinEdit1.Properties.MaxValue = 580;
                                    spinEdit2.Properties.MaxValue = (decimal)1.75;
                                    break;
                                case TurboType.GT3071R:
                                case TurboType.HX35w:
                                    spinEdit1.Properties.MaxValue = 600;
                                    spinEdit2.Properties.MaxValue = (decimal)1.80;
                                    break;
                                case TurboType.HX40w:
                                case TurboType.S400SX371:
                                    spinEdit1.Properties.MaxValue = 600;
                                    spinEdit2.Properties.MaxValue = (decimal)1.70;
                                    break;
                            }
                            break;
                        case InjectorType.Siemens875Dekas:
                        case InjectorType.Siemens1000cc:
                            // 3.0 bar sensor, 630cc injectors -> limit to turbo only
                            switch (turbo)
                            {
                                case TurboType.Stock:
                                case TurboType.TD0415T:
                                    spinEdit1.Properties.MaxValue = 450;
                                    spinEdit2.Properties.MaxValue = (decimal)1.45;
                                    break;
                                case TurboType.GT28BB:
                                case TurboType.GT28RS:
                                case TurboType.TD0419T:
                                    spinEdit1.Properties.MaxValue = 580;
                                    spinEdit2.Properties.MaxValue = (decimal)1.75;
                                    break;
                                case TurboType.GT3071R:
                                case TurboType.HX35w:
                                    spinEdit1.Properties.MaxValue = 620;
                                    spinEdit2.Properties.MaxValue = (decimal)1.9;
                                    break;
                                case TurboType.HX40w:
                                case TurboType.S400SX371:
                                    spinEdit1.Properties.MaxValue = 670;
                                    spinEdit2.Properties.MaxValue = (decimal)2.0;
                                    break;
                            }
                            break;
                    }
                    break;
                case MapSensorType.MapSensor35:
                case MapSensorType.MapSensor40:
                case MapSensorType.MapSensor50:
                    switch (injectors)
                    {
                        case InjectorType.Stock:
                            spinEdit1.Properties.MaxValue = 400;
                            spinEdit2.Properties.MaxValue = (decimal)1.30;
                            break;
                        case InjectorType.GreenGiants:
                            switch (turbo)
                            {
                                case TurboType.Stock:
                                case TurboType.TD0415T:
                                    spinEdit1.Properties.MaxValue = 450;
                                    spinEdit2.Properties.MaxValue = (decimal)1.45;
                                    break;
                                case TurboType.GT28BB:
                                case TurboType.GT28RS:
                                case TurboType.TD0419T:
                                    spinEdit1.Properties.MaxValue = 550;
                                    spinEdit2.Properties.MaxValue = (decimal)1.70;
                                    break;
                                case TurboType.GT3071R:
                                case TurboType.HX35w:
                                    spinEdit1.Properties.MaxValue = 550;
                                    spinEdit2.Properties.MaxValue = (decimal)1.50;
                                    break;
                                case TurboType.HX40w:
                                case TurboType.S400SX371:
                                    spinEdit1.Properties.MaxValue = 550;
                                    spinEdit2.Properties.MaxValue = (decimal)1.40;
                                    break;
                            }
                            break;
                        case InjectorType.Siemens630Dekas:
                            // 3.0 bar sensor, 630cc injectors -> max 600Nm
                            switch (turbo)
                            {
                                case TurboType.Stock:
                                case TurboType.TD0415T:
                                    spinEdit1.Properties.MaxValue = 450;
                                    spinEdit2.Properties.MaxValue = (decimal)1.45;
                                    break;
                                case TurboType.GT28BB:
                                case TurboType.GT28RS:
                                case TurboType.TD0419T:
                                    spinEdit1.Properties.MaxValue = 580;
                                    spinEdit2.Properties.MaxValue = (decimal)1.75;
                                    break;
                                case TurboType.GT3071R:
                                case TurboType.HX35w:
                                    spinEdit1.Properties.MaxValue = 600;
                                    spinEdit2.Properties.MaxValue = (decimal)1.80;
                                    break;
                                case TurboType.HX40w:
                                case TurboType.S400SX371:
                                    spinEdit1.Properties.MaxValue = 600;
                                    spinEdit2.Properties.MaxValue = (decimal)1.70;
                                    break;
                            }
                            break;
                        case InjectorType.Siemens875Dekas:
                        case InjectorType.Siemens1000cc:
                            // 3.0 bar sensor, 630cc injectors -> limit to turbo only
                            switch (turbo)
                            {
                                case TurboType.Stock:
                                case TurboType.TD0415T:
                                    spinEdit1.Properties.MaxValue = 450;
                                    spinEdit2.Properties.MaxValue = (decimal)1.45;
                                    break;
                                case TurboType.GT28BB:
                                case TurboType.GT28RS:
                                case TurboType.TD0419T:
                                    spinEdit1.Properties.MaxValue = 580;
                                    spinEdit2.Properties.MaxValue = (decimal)1.75;
                                    break;
                                case TurboType.GT3071R:
                                case TurboType.HX35w:
                                    spinEdit1.Properties.MaxValue = 620;
                                    spinEdit2.Properties.MaxValue = (decimal)1.9;
                                    break;
                                case TurboType.HX40w:
                                case TurboType.S400SX371:
                                    spinEdit1.Properties.MaxValue = 670;
                                    spinEdit2.Properties.MaxValue = (decimal)2.2;
                                    break;
                            }
                            break;
                    }
                    break;
            }
            UpdateStage();
        }


        private void UpdateStage()
        {
            // set the correct image
            Int32 Nm = Convert.ToInt32(spinEdit1.EditValue);
            Int32 stage = 0;
            if (Nm < 300) stage = 0;
            else if (Nm < 350) stage = 1;
            else if (Nm < 400) stage = 2;
            else if (Nm < 450) stage = 3;
            else if (Nm < 500) stage = 4;
            else if (Nm < 550) stage = 5;
            else if (Nm < 600) stage = 6;
            else if (Nm < 650) stage = 7;
            else stage = 8;
            string pictureName = Application.StartupPath + "\\imgs\\stage" + stage.ToString() + ".jpg";
            if (File.Exists(pictureName))
            {
                pictureBox1.Load(pictureName);
            }

        }

        private void comboBoxEdit2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // injectors changed
            UpdateMaxima((TurboType)comboBoxEdit3.SelectedIndex, (InjectorType)comboBoxEdit2.SelectedIndex, (MapSensorType)comboBoxEdit1.SelectedIndex);
        }

        private void comboBoxEdit3_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateMaxima((TurboType)comboBoxEdit3.SelectedIndex, (InjectorType)comboBoxEdit2.SelectedIndex, (MapSensorType)comboBoxEdit1.SelectedIndex);
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void radioGroup1_EditValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(radioGroup1.EditValue) == 0)
            {
                spinEdit1.Enabled = true;
                spinEdit2.Enabled = false;
            }
            else
            {
                spinEdit1.Enabled = false;
                spinEdit2.Enabled = true;
            }
        }

        private void spinEdit1_ValueChanged(object sender, EventArgs e)
        {
            UpdateStage();
        }

        private void frmFreeTuneSettings_Shown(object sender, EventArgs e)
        {
            UpdateStage();
        }
    }
}