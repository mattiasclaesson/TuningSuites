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
            this.components = new System.ComponentModel.Container();
            this.wizardTuning = new DevExpress.XtraWizard.WizardControl();
            this.wizWelcomePage = new DevExpress.XtraWizard.WelcomeWizardPage();
            this.wizSelectActionPage = new DevExpress.XtraWizard.WizardPage();
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.lblAuthor = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
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
            this.lblAuthorText = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.theCode = new DevExpress.XtraEditors.LabelControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.textPassword = new DevExpress.XtraEditors.TextEdit();
            this.lblCode = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutConverter1 = new DevExpress.XtraLayout.Converter.LayoutConverter(this.components);
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
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
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
            this.splitContainerControl1.Panel1.Controls.Add(this.lblAuthor);
            this.splitContainerControl1.Panel1.Controls.Add(this.labelControl3);
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
            // lblAuthor
            // 
            this.lblAuthor.Appearance.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblAuthor.Location = new System.Drawing.Point(4, 61);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new System.Drawing.Size(24, 13);
            this.lblAuthor.TabIndex = 3;
            this.lblAuthor.Text = "FFFF";
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelControl3.Appearance.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelControl3.Location = new System.Drawing.Point(4, 42);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(39, 13);
            this.labelControl3.TabIndex = 2;
            this.labelControl3.Text = "Author";
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
            this.listTuningActions.SelectedIndexChanged += new System.EventHandler(this.listTuningActions_SelectedIndexChanged);
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
            this.checkIUnderstand.Properties.Caption = "I fully understand the consequenses.";
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
            this.wizardCodePage.Controls.Add(this.lblAuthorText);
            this.wizardCodePage.Controls.Add(this.label2);
            this.wizardCodePage.Controls.Add(this.linkLabel1);
            this.wizardCodePage.Controls.Add(this.label1);
            this.wizardCodePage.Controls.Add(this.theCode);
            this.wizardCodePage.Controls.Add(this.layoutControl1);
            this.wizardCodePage.DescriptionText = "This Tuning Package requires that you have a code";
            this.wizardCodePage.Name = "wizardCodePage";
            this.wizardCodePage.Size = new System.Drawing.Size(499, 171);
            this.wizardCodePage.Text = "Enter Tuning Code";
            this.wizardCodePage.Visible = false;
            // 
            // lblAuthorText
            // 
            this.lblAuthorText.AutoSize = true;
            this.lblAuthorText.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblAuthorText.Location = new System.Drawing.Point(9, 139);
            this.lblAuthorText.Name = "lblAuthorText";
            this.lblAuthorText.Size = new System.Drawing.Size(29, 13);
            this.lblAuthorText.TabIndex = 5;
            this.lblAuthorText.Text = "Hint:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label2.Location = new System.Drawing.Point(149, 123);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(158, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "and learn how to earn the code.";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(66, 123);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(87, 13);
            this.linkLabel1.TabIndex = 3;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "trionictuning.com";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Location = new System.Drawing.Point(9, 123);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Please visit ";
            // 
            // theCode
            // 
            this.theCode.Location = new System.Drawing.Point(411, 155);
            this.theCode.Name = "theCode";
            this.theCode.Size = new System.Drawing.Size(63, 13);
            this.theCode.TabIndex = 1;
            this.theCode.Text = "labelControl5";
            this.theCode.Visible = false;
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.textPassword);
            this.layoutControl1.Controls.Add(this.lblCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1012, 151, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(499, 120);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // textPassword
            // 
            this.textPassword.Location = new System.Drawing.Point(71, 29);
            this.textPassword.Name = "textPassword";
            this.textPassword.Size = new System.Drawing.Size(416, 20);
            this.textPassword.StyleController = this.layoutControl1;
            this.textPassword.TabIndex = 7;
            this.textPassword.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.textPassword_EditValueChanging);
            // 
            // lblCode
            // 
            this.lblCode.Appearance.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblCode.Location = new System.Drawing.Point(12, 12);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(475, 13);
            this.lblCode.StyleController = this.layoutControl1;
            this.lblCode.TabIndex = 4;
            this.lblCode.Text = "labelControl3";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem3});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(499, 120);
            this.layoutControlGroup1.Text = "layoutControlGroup1";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.lblCode;
            this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(479, 17);
            this.layoutControlItem1.Text = "layoutControlItem1";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextToControlDistance = 0;
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.textPassword;
            this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 17);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(479, 83);
            this.layoutControlItem3.Text = "Enter code:";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(56, 13);
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
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.textPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
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
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.LabelControl lblCode;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.Converter.LayoutConverter layoutConverter1;
        private DevExpress.XtraEditors.LabelControl theCode;
        private DevExpress.XtraEditors.TextEdit textPassword;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private DevExpress.XtraEditors.LabelControl lblAuthor;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private System.Windows.Forms.Label lblAuthorText;
    }
}