using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace T8SuitePro
{
    public partial class frmTuningWizard : Form
    {
        readonly Form1 parent;

        public frmTuningWizard(Form1 inParent, string in_m_currentfile)
        {
            InitializeComponent();
            parent = inParent;
            // Set-up some navigation rules
            this.wizConfirmPage.AllowNext = false;
            this.wizCompletedPage.AllowBack = false;
            // List all tuning packages
            foreach (Form1.TuningAction t in Form1.installedTunings )
                this.listTuningActions.Items.Add(t);
            
            // Read software version from binary
            if (in_m_currentfile != string.Empty)
            {
                if (File.Exists(in_m_currentfile))
                {
                    T8Header t8header = new T8Header();
                    t8header.init(in_m_currentfile);
                    this.lblSoftwareVersion.Text = t8header.SoftwareVersion.Trim().Substring(0,4);
                }
            }
        }

        private void wizardTuning_NextClick(object sender, DevExpress.XtraWizard.WizardCommandButtonClickEventArgs e)
        {
            // Update coming pages based on listbox selection
            if (e.Page.Name == "wizSelectActionPage")
            {
                this.lblTuningActionConfirm.Text = this.listTuningActions.SelectedItem.ToString();
            }
            // Perform the selected tuning action, and disable possibility to press cancel. 
            // At this stage, it is to late. Modifications has been done.
            else if(e.Page.Name == "wizConfirmPage")
            {
                // Disable turning back
                this.wizCompletedPage.AllowCancel = false;
                Form1.TuningAction selAction = (Form1.TuningAction)this.listTuningActions.SelectedItem;
                // Perform the tuning action
                //if (parent.performTuningAction(selAction) == 0)
                if(selAction.performTuningAction() == 0)
                {
                    // Inform the user of the tuning action
                    string[] imactedMaps = selAction.getImpactedMaps(); // FIX: Maybe list all maps that were updated?
                    this.wizCompletedPage.FinishText = "You have now completed the Tuning Action '" +
                        this.listTuningActions.SelectedItem.ToString() +
                        "'. Please check the modified maps so that they are what you expect them to be." +
                        " Easiest way to do that is to compare to the original binary.";
                    foreach (string impM in imactedMaps)
                        this.listModifiedMaps.Items.Add(impM);
                }
                else
                {
                    this.wizCompletedPage.FinishText = "The Tuning Action '" + 
                        this.listTuningActions.SelectedItem.ToString() +
                        "' failed! You should likely not use this binary at this point.";
                }
            }
        }

        private void wizardTuning_PrevClick(object sender, DevExpress.XtraWizard.WizardCommandButtonClickEventArgs e)
        {
            // Uncheck confirmation when used clicked back from confirmation page. He is ambivalent.
            if (e.Page.Name == "wizConfirmPage")
                this.checkIUnderstand.Checked = false;
        }

        private void checkIUnderstand_CheckedChanged(object sender, EventArgs e)
        {
            // Only allow to move forward if user is ready to face consequences
            CheckEdit edit = sender as CheckEdit;
            switch (edit.Checked)
            {
                case true:
                    this.wizConfirmPage.AllowNext = true;
                    break;
                case false:
                    this.wizConfirmPage.AllowNext = false;
                    break;
            }
        }
    }
}