namespace T8SuitePro
{
    partial class frmTuningWizard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.wizardTuning = new DevExpress.XtraWizard.WizardControl();
            this.wizWelcomePage = new DevExpress.XtraWizard.WelcomeWizardPage();
            this.wizSelectActionPage = new DevExpress.XtraWizard.WizardPage();
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.lblSoftwareVersion = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.listTuningActions = new DevExpress.XtraEditors.ListBoxControl();
            this.wizCompletedPage = new DevExpress.XtraWizard.CompletionWizardPage();
            this.listModifiedMaps = new DevExpress.XtraEditors.ListBoxControl();
            this.wizConfirmPage = new DevExpress.XtraWizard.WizardPage();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.lblTuningActionConfirm = new DevExpress.XtraEditors.LabelControl();
            this.checkIUnderstand = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.wizardCodePage = new DevExpress.XtraWizard.WizardPage();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.textPassword = new System.Windows.Forms.TextBox();
            this.lblCode = new System.Windows.Forms.Label();
            this.theCode = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.wizardTuning)).BeginInit();
            this.wizardTuning.SuspendLayout();
            this.wizSelectActionPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.listTuningActions)).BeginInit();
            this.wizCompletedPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.listModifiedMaps)).BeginInit();
            this.wizConfirmPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkIUnderstand.Properties)).BeginInit();
            this.wizardCodePage.SuspendLayout();
            this.SuspendLayout();
            // 
            // wizardTuning
            // 
            this.wizardTuning.Controls.Add(this.wizWelcomePage);
            this.wizardTuning.Controls.Add(this.wizSelectActionPage);
            this.wizardTuning.Controls.Add(this.wizCompletedPage);
            this.wizardTuning.Controls.Add(this.wizConfirmPage);
            this.wizardTuning.Controls.Add(this.wizardCodePage);
            this.wizardTuning.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardTuning.Location = new System.Drawing.Point(0, 0);
            this.wizardTuning.Name = "wizardTuning";
            this.wizardTuning.Pages.AddRange(new DevExpress.XtraWizard.BaseWizardPage[] {
            this.wizWelcomePage,
            this.wizSelectActionPage,
            this.wizardCodePage,
            this.wizConfirmPage,
            this.wizCompletedPage});
            this.wizardTuning.Size = new System.Drawing.Size(531, 316);
            this.wizardTuning.NextClick += new DevExpress.XtraWizard.WizardCommandButtonClickEventHandler(this.wizardTuning_NextClick);
            this.wizardTuning.PrevClick += new DevExpress.XtraWizard.WizardCommandButtonClickEventHandler(this.wizardTuning_PrevClick);
            // 
            // wizWelcomePage
            // 
            this.wizWelcomePage.IntroductionText = "This wizard will help you apply a pre-defined set of Tuning Actions to your binar" +
                "y file. You will be given a choice of Tuning Actions in the coming pages.";
            this.wizWelcomePage.Name = "wizWelcomePage";
            this.wizWelcomePage.Size = new System.Drawing.Size(314, 183);
            this.wizWelcomePage.Text = "Tuning Wizard";
            // 
            // wizSelectActionPage
            // 
            this.wizSelectActionPage.Controls.Add(this.splitContainerControl1);
            this.wizSelectActionPage.DescriptionText = "Selection your wanted Tuning Action from the list below and click Next";
            this.wizSelectActionPage.Name = "wizSelectActionPage";
            this.wizSelectActionPage.Size = new System.Drawing.Size(499, 171);
            this.wizSelectActionPage.Text = "Select Tuning Action";
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.lblSoftwareVersion);
            this.splitContainerControl1.Panel1.Controls.Add(this.labelControl1);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            this.splitContainerControl1.Panel2.Controls.Add(this.listTuningActions);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(499, 171);
            this.splitContainerControl1.SplitterPosition = 101;
            this.splitContainerControl1.TabIndex = 0;
            this.splitContainerControl1.Text = "splitContainerControl1";
            // 
            // lblSoftwareVersion
            // 
            this.lblSoftwareVersion.Appearance.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblSoftwareVersion.Location = new System.Drawing.Point(4, 23);
            this.lblSoftwareVersion.Name = "lblSoftwareVersion";
            this.lblSoftwareVersion.Size = new System.Drawing.Size(24, 13);
            this.lblSoftwareVersion.TabIndex = 1;
            this.lblSoftwareVersion.Text = "FFFF";
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelControl1.Appearance.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelControl1.Location = new System.Drawing.Point(4, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(96, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Software Version";
            // 
            // listTuningActions
            // 
            this.listTuningActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listTuningActions.Location = new System.Drawing.Point(0, 0);
            this.listTuningActions.Name = "listTuningActions";
            this.listTuningActions.Size = new System.Drawing.Size(393, 171);
            this.listTuningActions.TabIndex = 0;
            // 
            // wizCompletedPage
            // 
            this.wizCompletedPage.Controls.Add(this.listModifiedMaps);
            this.wizCompletedPage.FinishText = "You have completed the selected tuning action.";
            this.wizCompletedPage.Name = "wizCompletedPage";
            this.wizCompletedPage.Size = new System.Drawing.Size(314, 183);
            this.wizCompletedPage.Text = "Completed Tuning Wizard";
            // 
            // listModifiedMaps
            // 
            this.listModifiedMaps.Location = new System.Drawing.Point(0, 67);
            this.listModifiedMaps.Name = "listModifiedMaps";
            this.listModifiedMaps.Size = new System.Drawing.Size(314, 86);
            this.listModifiedMaps.TabIndex = 0;
            // 
            // wizConfirmPage
            // 
            this.wizConfirmPage.Controls.Add(this.labelControl4);
            this.wizConfirmPage.Controls.Add(this.lblTuningActionConfirm);
            this.wizConfirmPage.Controls.Add(this.checkIUnderstand);
            this.wizConfirmPage.Controls.Add(this.labelControl2);
            this.wizConfirmPage.DescriptionText = "Please confirm your Tuning Action and click Next";
            this.wizConfirmPage.Name = "wizConfirmPage";
            this.wizConfirmPage.Size = new System.Drawing.Size(499, 171);
            this.wizConfirmPage.Text = "Confirm Tuning Action";
            // 
            // labelControl4
            // 
            this.labelControl4.Appearance.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelControl4.Location = new System.Drawing.Point(25, 66);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(410, 13);
            this.labelControl4.TabIndex = 3;
            this.labelControl4.Text = "The Tuning Action cannot be revered, ensure you have a copy of your binary stored" +
                ".";
            // 
            // lblTuningActionConfirm
            // 
            this.lblTuningActionConfirm.Appearance.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblTuningActionConfirm.Location = new System.Drawing.Point(25, 35);
            this.lblTuningActionConfirm.Name = "lblTuningActionConfirm";
            this.lblTuningActionConfirm.Size = new System.Drawing.Size(25, 13);
            this.lblTuningActionConfirm.TabIndex = 2;
            this.lblTuningActionConfirm.Text = "None";
            // 
            // checkIUnderstand
            // 
            this.checkIUnderstand.Location = new System.Drawing.Point(23, 85);
            this.checkIUnderstand.Name = "checkIUnderstand";
            this.checkIUnderstand.Properties.Appearance.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.checkIUnderstand.Properties.Appearance.Options.UseForeColor = true;
            this.checkIUnderstand.Properties.Caption = "I fully understand the consequeses.";
            this.checkIUnderstand.Size = new System.Drawing.Size(202, 19);
            this.checkIUnderstand.TabIndex = 1;
            this.checkIUnderstand.CheckedChanged += new System.EventHandler(this.checkIUnderstand_CheckedChanged);
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelControl2.Location = new System.Drawing.Point(25, 15);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(364, 13);
            this.labelControl2.TabIndex = 0;
            this.labelControl2.Text = "Your loaded binary file will now be modified with the following Tuning Action:";
            // 
            // wizardCodePage
            // 
            this.wizardCodePage.Controls.Add(this.theCode);
            this.wizardCodePage.Controls.Add(this.label2);
            this.wizardCodePage.Controls.Add(this.label1);
            this.wizardCodePage.Controls.Add(this.linkLabel1);
            this.wizardCodePage.Controls.Add(this.textPassword);
            this.wizardCodePage.Controls.Add(this.lblCode);
            this.wizardCodePage.DescriptionText = "This Tuning Package requires that you have a code";
            this.wizardCodePage.Name = "wizardCodePage";
            this.wizardCodePage.Size = new System.Drawing.Size(499, 171);
            this.wizardCodePage.Text = "Enter Tuning Code";
            this.wizardCodePage.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label2.Location = new System.Drawing.Point(137, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(181, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "forum to learn more and earn a code.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Location = new System.Drawing.Point(10, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Visit the";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(52, 64);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(87, 13);
            this.linkLabel1.TabIndex = 3;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "trionictuning.com";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // textPassword
            // 
            this.textPassword.Location = new System.Drawing.Point(13, 25);
            this.textPassword.Name = "textPassword";
            this.textPassword.PasswordChar = '*';
            this.textPassword.Size = new System.Drawing.Size(100, 20);
            this.textPassword.TabIndex = 2;
            this.textPassword.TextChanged += new System.EventHandler(this.textPassword_TextChanged);
            // 
            // lblCode
            // 
            this.lblCode.AutoSize = true;
            this.lblCode.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblCode.Location = new System.Drawing.Point(10, 9);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(111, 13);
            this.lblCode.TabIndex = 1;
            this.lblCode.Text = "Please enter the code";
            // 
            // theCode
            // 
            this.theCode.AutoSize = true;
            this.theCode.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.theCode.Location = new System.Drawing.Point(10, 99);
            this.theCode.Name = "theCode";
            this.theCode.Size = new System.Drawing.Size(39, 13);
            this.theCode.TabIndex = 6;
            this.theCode.Text = "hidden";
            this.theCode.Visible = false;
            // 
            // frmTuningWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(531, 316);
            this.Controls.Add(this.wizardTuning);
            this.ForeColor = System.Drawing.Color.Red;
            this.Name = "frmTuningWizard";
            this.Text = "Tuning Wizard";
            ((System.ComponentModel.ISupportInitialize)(this.wizardTuning)).EndInit();
            this.wizardTuning.ResumeLayout(false);
            this.wizSelectActionPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.listTuningActions)).EndInit();
            this.wizCompletedPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.listModifiedMaps)).EndInit();
            this.wizConfirmPage.ResumeLayout(false);
            this.wizConfirmPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkIUnderstand.Properties)).EndInit();
            this.wizardCodePage.ResumeLayout(false);
            this.wizardCodePage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraWizard.WizardControl wizardTuning;
        private DevExpress.XtraWizard.WelcomeWizardPage wizWelcomePage;
        private DevExpress.XtraWizard.WizardPage wizSelectActionPage;
        private DevExpress.XtraWizard.CompletionWizardPage wizCompletedPage;
        private DevExpress.XtraWizard.WizardPage wizConfirmPage;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl lblSoftwareVersion;
        private DevExpress.XtraEditors.ListBoxControl listTuningActions;
        private DevExpress.XtraEditors.LabelControl lblTuningActionConfirm;
        private DevExpress.XtraEditors.CheckEdit checkIUnderstand;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.ListBoxControl listModifiedMaps;
        private DevExpress.XtraWizard.WizardPage wizardCodePage;
        private System.Windows.Forms.Label lblCode;
        private System.Windows.Forms.TextBox textPassword;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label theCode;
    }
}