using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommonSuite;


namespace T8SuitePro.DataControllers
{

    class DTCRunTime : BaseDatacontroller
    {
        private UserControls.GridWithButtons mUserControl;

        public DTCRunTime() : base()
        {
            mUserControl = new UserControls.GridWithButtons();

            Data();
            Configure();
        }

        protected override void Data()
        {
        }


        protected override void Configure()
        {
        }




        void frmfaults_onClose(object sender, EventArgs e)
        {
//            t8can.Cleanup();
            //Form1.barConnectedECUName.Caption = string.Empty;
        }

        void frmfaults_onClearCurrentDTC(object sender, frmFaultcodes.ClearDTCEventArgs e)
        {
            // clear the currently selected DTC code from the ECU
            if (e.DTCCode.StartsWith("P"))
            {
                try
                {
                    int DTCCode = Convert.ToInt32(e.DTCCode.Substring(1, e.DTCCode.Length - 1), 16);

                    //TODO ClearDTCCodes() must be added to the api
                    //t8can.ClearDTCCodes(DTCCode);

                    if (sender is frmFaultcodes)
                    {
                        frmFaultcodes frmfaults = (frmFaultcodes)sender;
                        frmfaults.ClearCodes();

                        string[] faults = null;//t8can.ReadDTC();
                        foreach (string fault in faults)
                        {
                            frmfaults.addFault(fault.Substring(5, 5));
                        }
                        frmfaults.Show();
                    }
                }
                catch (Exception E)
                {
                    mLogger.Debug(E.Message);
                }
            }
        }

    }
}
