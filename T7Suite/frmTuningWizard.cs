using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using TrionicCANLib.Checksum;

namespace T7
{
    public partial class frmTuningWizard : Form
    {
        readonly frmMain parent;
        private string softwareVersion;

        public frmTuningWizard(frmMain inParent, string in_m_currentfile)
        {
            InitializeComponent();
            parent = inParent;

            // Set-up some navigation rules
            this.wizConfirmPage.AllowNext = false;
            this.wizCompletedPage.AllowBack = false;

            // Read software version from binary
            if (in_m_currentfile != string.Empty)
            {
                if (File.Exists(in_m_currentfile))
                {
                    T7FileHeader t7InfoHeader = new T7FileHeader();
                    t7InfoHeader.init(in_m_currentfile, false);
                    softwareVersion = t7InfoHeader.getSoftwareVersion().Trim();
                    this.lblSoftwareVersion.Text = softwareVersion.Substring(0, 8);
                }
            }
            // List all compatible tuning packages
            foreach (frmMain.TuningAction t in frmMain.installedTunings)
                if (t.compatibelSoftware(softwareVersion))
                    this.listTuningActions.Items.Add(t);
            if (this.listTuningActions.ItemCount <= 0)
                this.wizSelectActionPage.AllowNext = false;

        }

        private void wizardTuning_NextClick(object sender, DevExpress.XtraWizard.WizardCommandButtonClickEventArgs e)
        {
            // Update coming pages based on listbox selection
            if (e.Page.Name == "wizSelectActionPage")
            {
                this.lblTuningActionConfirm.Text = this.listTuningActions.SelectedItem.ToString();
                frmMain.TuningAction selAction = (frmMain.TuningAction)this.listTuningActions.SelectedItem;
                if (selAction.WizCode != string.Empty)
                {
                    this.textPassword.Text = "";
                    //this.textPassword.Focus()
                    this.theCode.Text = selAction.WizCode;
                    this.wizardCodePage.Visible = true;
                    this.wizardCodePage.AllowNext = false;
                    this.lblAuthorText.Text = "Hint: Try with authors first. For this Tuning Package it is " + selAction.WizAuth;
                    this.lblCode.Text = "The Tuning Package \'" + this.listTuningActions.SelectedItem.ToString() + "\' requires that you enter the correct code.";
                }
            }

            // Perform the selected tuning action, and disable possibility to press cancel. 
            // At this stage, it is to late. Modifications has been done.
            else if(e.Page.Name == "wizConfirmPage")
            {
                // Disable turning back
                this.wizCompletedPage.AllowCancel = false;
                frmMain.TuningAction selAction = (frmMain.TuningAction)this.listTuningActions.SelectedItem;

                // Perform the tuning action
                List<string> outList = new List<string>();
                if (selAction.performTuningAction(parent, softwareVersion, out outList) == 0)
                {
                    // Inform the user of the tuning action
                    this.wizCompletedPage.FinishText = "You have now completed the Tuning Action '" +
                        this.listTuningActions.SelectedItem.ToString() +
                        "'. Please check the modified maps below so that they are what you expect them to be." +
                        " Easiest way to do that is to compare to the original binary.";

                    // Fill list with impacted maps
                    foreach (string impM in outList)
                        this.listModifiedMaps.Items.Add(impM);

                    frmMain.TuningAction nameDate = frmMain.installedTunings.ElementAt(0);
                    if (nameDate.WizIdOrFilename == "ap_dateName")
                    {
                        nameDate.performTuningAction(parent, softwareVersion, out outList);
                        foreach (string impM in outList)
                            this.listModifiedMaps.Items.Add(impM);
                    }

                    // Show the message of the tuning pack if present
                    if (selAction.WizMsg != string.Empty)
                    {
                        MessageBox.Show(selAction.WizMsg, "Tuning Wizard Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
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

        private void textPassword_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (this.textPassword.Text == this.theCode.Text)
                this.wizardCodePage.AllowNext = true;
            else
                this.wizardCodePage.AllowNext = false;

        }

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Specify that the link was visited. 
            this.linkLabel1.LinkVisited = true;

            // Navigate to a URL.
            System.Diagnostics.Process.Start("http://www.trionictuning.com");

        }

        private void listTuningActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            frmMain.TuningAction selAction = (frmMain.TuningAction)this.listTuningActions.SelectedItem;
            this.lblAuthor.Text = selAction.WizAuth;
        }
    }
}