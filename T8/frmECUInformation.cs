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
    public partial class frmECUInformation : DevExpress.XtraEditors.XtraForm
    {
        /*
08:39:31.244 - VINNumber       : YS3FB45F161020004 
08:39:31.276 - Calibration set : P_78_FIEF_ 
08:39:31.322 - Codefile version: FA5L       
08:39:31.369 - ECU description : P6.8 
08:39:31.416 - ECU hardware    : Trionic 8  
08:39:31.463 - ECU sw number   :     
08:39:31.494 - Programming date: ???? 
08:39:31.556 - Build date      : 2004-11-11 08:58:57      
08:39:31.619 - Serial number   : 863232AC0A50DN7G  
08:39:31.666 - Software version: FA5L_C_FMEP_78_FIEF_80e         
08:39:31.744 - 0F identifier   : FA5L_C_FMEP_XX_XXX_XXX.tmp      
08:39:31.775 - SW identifier 1 : 55560572   
08:39:31.837 - SW identifier 2 : 12799173   
08:39:31.884 - SW identifier 3 : 55353555   
08:39:31.931 - SW identifier 4 : 55352573   
08:39:31.978 - SW identifier 5 : 55352574   
08:39:32.024 - SW identifier 6 : 55352572   
08:39:32.087 - Hardware type   : ECM                      
08:39:32.149 - 75 identifier   : 78 09      
08:39:32.180 - Engine type     : 80e  
08:39:32.227 - Supplier ID     : GMPT 0100  
08:39:32.258 - Speed limiter   : 230 km/h
08:39:32.274 - SAAB partnumber : 55575467
08:39:32.305 - Diagnostic ID   : 0x01 0x10
08:39:32.321 - End model partnr: 55560579
08:39:32.352 - Basemodel partnr: 55353231
         */

        public frmECUInformation()
        {
            InitializeComponent();
        }

        public void SetECUHardwareDescription(string hw)
        {
            textEdit1.Text = hw;
            Application.DoEvents();
        }

        public void SetECUHardware(string hw)
        {
            textEdit2.Text = hw;
            Application.DoEvents();
        }

        public void SetECUHardwareType(string hw)
        {
            textEdit5.Text = hw;
            Application.DoEvents();
        }
        public void SetECUHardwareSupplierID(string hw)
        {
            textEdit6.Text = hw;
            Application.DoEvents();
        }
        public void SetECUBuildDate(string hw)
        {
            textEdit3.Text = hw;
            Application.DoEvents();
        }
        public void SetECUSerialNumber(string hw)
        {
            textEdit4.Text = hw;
            Application.DoEvents();
        }
        public void SetECUSAABPartnumber(string hw)
        {
            textEdit7.Text = hw;
            Application.DoEvents();
        }

        public void SetECUBasemodel(string hw)
        {
            textEdit8.Text = hw;
            Application.DoEvents();
        }
        public void SetECUEndmodel(string hw)
        {
            textEdit9.Text = hw;
            Application.DoEvents();
        }

        public void SetCalibrationSet(string hw)
        {
            textEdit10.Text = hw;
            Application.DoEvents();
        }

        public void SetCodefileVersion(string hw)
        {
            textEdit11.Text = hw;
            Application.DoEvents();
        }

        public void SetSoftwareVersion(string hw)
        {
            textEdit12.Text = hw;
            Application.DoEvents();
        }

        public void SetSoftwareVersionFile(string hw)
        {
            textEdit13.Text = hw;
            Application.DoEvents();
        }

        public void SetSoftwareID1(string hw)
        {
            textEdit14.Text = hw;
            Application.DoEvents();
        }
        public void SetSoftwareID2(string hw)
        {
            textEdit15.Text = hw;
            Application.DoEvents();
        }
        public void SetSoftwareID3(string hw)
        {
            textEdit17.Text = hw;
            Application.DoEvents();
        }
        public void SetSoftwareID4(string hw)
        {
            textEdit16.Text = hw;
            Application.DoEvents();
        }
        public void SetSoftwareID5(string hw)
        {
            textEdit19.Text = hw;
            Application.DoEvents();
        }
        public void SetSoftwareID6(string hw)
        {
            textEdit18.Text = hw;
            Application.DoEvents();
        }

        public void SetVIN(string hw)
        {
            textEdit20.Text = hw;
            Application.DoEvents();
        }

        public void SetEngineType(string hw)
        {
            textEdit21.Text = hw;
            Application.DoEvents();
        }

        public void SetSpeedLimit(string hw)
        {
            textEdit22.Text = hw;
            Application.DoEvents();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textEdit2_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void textEdit5_EditValueChanged(object sender, EventArgs e)
        {

        }
    }
}