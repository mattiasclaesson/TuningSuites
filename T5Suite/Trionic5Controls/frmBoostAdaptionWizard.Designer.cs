namespace Trionic5Controls
{
    partial class frmBoostAdaptionWizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBoostAdaptionWizard));
            this.wizardControl1 = new DevExpress.XtraWizard.WizardControl();
            this.welcomeWizardPage1 = new DevExpress.XtraWizard.WelcomeWizardPage();
            this.memoEdit1 = new DevExpress.XtraEditors.MemoEdit();
            this.wizardPage1 = new DevExpress.XtraWizard.WizardPage();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.spinEdit5 = new DevExpress.XtraEditors.SpinEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.spinEdit3 = new DevExpress.XtraEditors.SpinEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.spinEdit4 = new DevExpress.XtraEditors.SpinEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.spinEdit2 = new DevExpress.XtraEditors.SpinEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.spinEdit1 = new DevExpress.XtraEditors.SpinEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.completionWizardPage1 = new DevExpress.XtraWizard.CompletionWizardPage();
            this.memoEdit2 = new DevExpress.XtraEditors.MemoEdit();
            ((System.ComponentModel.ISupportInitialize)(this.wizardControl1)).BeginInit();
            this.wizardControl1.SuspendLayout();
            this.welcomeWizardPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit1.Properties)).BeginInit();
            this.wizardPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit5.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit3.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit4.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit1.Properties)).BeginInit();
            this.completionWizardPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit2.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // wizardControl1
            // 
            this.wizardControl1.Controls.Add(this.welcomeWizardPage1);
            this.wizardControl1.Controls.Add(this.wizardPage1);
            this.wizardControl1.Controls.Add(this.completionWizardPage1);
            this.wizardControl1.Image = ((System.Drawing.Image)(resources.GetObject("wizardControl1.Image")));
            this.wizardControl1.ImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.wizardControl1.Name = "wizardControl1";
            this.wizardControl1.Pages.AddRange(new DevExpress.XtraWizard.BaseWizardPage[] {
            this.welcomeWizardPage1,
            this.wizardPage1,
            this.completionWizardPage1});
            // 
            // welcomeWizardPage1
            // 
            this.welcomeWizardPage1.Controls.Add(this.memoEdit1);
            this.welcomeWizardPage1.IntroductionText = "This wizard will guide you through the neccesary steps to adjust your boost adapt" +
                "ion range";
            this.welcomeWizardPage1.Name = "welcomeWizardPage1";
            this.welcomeWizardPage1.Size = new System.Drawing.Size(569, 427);
            this.welcomeWizardPage1.Text = "Welcome to the boost adaption range wizard";
            // 
            // memoEdit1
            // 
            this.memoEdit1.EditValue = "Please note that this wizard will adjust the actual code in your binary!\r\n\r\nThe b" +
                "oost adaption range should match the spool characteristics of your turbo.";
            this.memoEdit1.Location = new System.Drawing.Point(70, 146);
            this.memoEdit1.Name = "memoEdit1";
            this.memoEdit1.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.memoEdit1.Properties.Appearance.ForeColor = System.Drawing.Color.Firebrick;
            this.memoEdit1.Properties.Appearance.Options.UseBackColor = true;
            this.memoEdit1.Properties.Appearance.Options.UseForeColor = true;
            this.memoEdit1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.memoEdit1.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.memoEdit1.Size = new System.Drawing.Size(434, 172);
            this.memoEdit1.TabIndex = 1;
            // 
            // wizardPage1
            // 
            this.wizardPage1.Controls.Add(this.labelControl6);
            this.wizardPage1.Controls.Add(this.spinEdit5);
            this.wizardPage1.Controls.Add(this.labelControl1);
            this.wizardPage1.Controls.Add(this.labelControl5);
            this.wizardPage1.Controls.Add(this.spinEdit1);
            this.wizardPage1.Controls.Add(this.spinEdit3);
            this.wizardPage1.Controls.Add(this.labelControl2);
            this.wizardPage1.Controls.Add(this.labelControl3);
            this.wizardPage1.Controls.Add(this.spinEdit2);
            this.wizardPage1.Controls.Add(this.spinEdit4);
            this.wizardPage1.Controls.Add(this.labelControl4);
            this.wizardPage1.DescriptionText = "Please set the options below to match your turbos characteristics.";
            this.wizardPage1.Name = "wizardPage1";
            this.wizardPage1.Size = new System.Drawing.Size(754, 415);
            this.wizardPage1.Text = "Boost adaption range options";
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(419, 202);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(16, 13);
            this.labelControl6.TabIndex = 10;
            this.labelControl6.Text = "bar";
            // 
            // spinEdit5
            // 
            this.spinEdit5.EditValue = new decimal(new int[] {
            4,
            0,
            0,
            131072});
            this.spinEdit5.Enabled = false;
            this.spinEdit5.Location = new System.Drawing.Point(324, 199);
            this.spinEdit5.Name = "spinEdit5";
            this.spinEdit5.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit5.Properties.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.spinEdit5.Properties.MaxValue = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.spinEdit5.Properties.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.spinEdit5.Size = new System.Drawing.Size(70, 20);
            this.spinEdit5.TabIndex = 9;
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(187, 202);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(81, 13);
            this.labelControl5.TabIndex = 8;
            this.labelControl5.Text = "Max. boost error";
            // 
            // spinEdit3
            // 
            this.spinEdit3.EditValue = new decimal(new int[] {
            4500,
            0,
            0,
            0});
            this.spinEdit3.Location = new System.Drawing.Point(485, 159);
            this.spinEdit3.Name = "spinEdit3";
            this.spinEdit3.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit3.Properties.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.spinEdit3.Properties.IsFloatValue = false;
            this.spinEdit3.Properties.Mask.EditMask = "N00";
            this.spinEdit3.Properties.MaxValue = new decimal(new int[] {
            9000,
            0,
            0,
            0});
            this.spinEdit3.Properties.MinValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.spinEdit3.Size = new System.Drawing.Size(70, 20);
            this.spinEdit3.TabIndex = 7;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(419, 162);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(46, 13);
            this.labelControl3.TabIndex = 6;
            this.labelControl3.Text = "rpm upto ";
            // 
            // spinEdit4
            // 
            this.spinEdit4.EditValue = new decimal(new int[] {
            2750,
            0,
            0,
            0});
            this.spinEdit4.Location = new System.Drawing.Point(324, 159);
            this.spinEdit4.Name = "spinEdit4";
            this.spinEdit4.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit4.Properties.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.spinEdit4.Properties.IsFloatValue = false;
            this.spinEdit4.Properties.Mask.EditMask = "N00";
            this.spinEdit4.Properties.MaxValue = new decimal(new int[] {
            9000,
            0,
            0,
            0});
            this.spinEdit4.Properties.MinValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.spinEdit4.Size = new System.Drawing.Size(70, 20);
            this.spinEdit4.TabIndex = 5;
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(187, 162);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(116, 13);
            this.labelControl4.TabIndex = 4;
            this.labelControl4.Text = "Automatic gearbox from";
            // 
            // spinEdit2
            // 
            this.spinEdit2.EditValue = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.spinEdit2.Location = new System.Drawing.Point(485, 133);
            this.spinEdit2.Name = "spinEdit2";
            this.spinEdit2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit2.Properties.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.spinEdit2.Properties.IsFloatValue = false;
            this.spinEdit2.Properties.Mask.EditMask = "N00";
            this.spinEdit2.Properties.MaxValue = new decimal(new int[] {
            9000,
            0,
            0,
            0});
            this.spinEdit2.Properties.MinValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.spinEdit2.Size = new System.Drawing.Size(70, 20);
            this.spinEdit2.TabIndex = 3;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(419, 136);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(46, 13);
            this.labelControl2.TabIndex = 2;
            this.labelControl2.Text = "rpm upto ";
            // 
            // spinEdit1
            // 
            this.spinEdit1.EditValue = new decimal(new int[] {
            2750,
            0,
            0,
            0});
            this.spinEdit1.Location = new System.Drawing.Point(324, 133);
            this.spinEdit1.Name = "spinEdit1";
            this.spinEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit1.Properties.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.spinEdit1.Properties.IsFloatValue = false;
            this.spinEdit1.Properties.Mask.EditMask = "N00";
            this.spinEdit1.Properties.MaxValue = new decimal(new int[] {
            9000,
            0,
            0,
            0});
            this.spinEdit1.Properties.MinValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.spinEdit1.Size = new System.Drawing.Size(70, 20);
            this.spinEdit1.TabIndex = 1;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(187, 136);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(102, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Manual gearbox from";
            // 
            // completionWizardPage1
            // 
            this.completionWizardPage1.Controls.Add(this.memoEdit2);
            this.completionWizardPage1.Name = "completionWizardPage1";
            this.completionWizardPage1.Size = new System.Drawing.Size(569, 427);
            // 
            // memoEdit2
            // 
            this.memoEdit2.EditValue = "Please note that boost adaption takes a little while. You can speed up the proces" +
                "s by running the car at wide open throttle a couple of times throughout the sele" +
                "cted boost adaption range.";
            this.memoEdit2.Location = new System.Drawing.Point(70, 146);
            this.memoEdit2.Name = "memoEdit2";
            this.memoEdit2.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.memoEdit2.Properties.Appearance.ForeColor = System.Drawing.Color.Firebrick;
            this.memoEdit2.Properties.Appearance.Options.UseBackColor = true;
            this.memoEdit2.Properties.Appearance.Options.UseForeColor = true;
            this.memoEdit2.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.memoEdit2.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.memoEdit2.Size = new System.Drawing.Size(434, 140);
            this.memoEdit2.TabIndex = 2;
            // 
            // frmBoostAdaptionWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 560);
            this.ControlBox = false;
            this.Controls.Add(this.wizardControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmBoostAdaptionWizard";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Boost adaption range wizard";
            ((System.ComponentModel.ISupportInitialize)(this.wizardControl1)).EndInit();
            this.wizardControl1.ResumeLayout(false);
            this.welcomeWizardPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit1.Properties)).EndInit();
            this.wizardPage1.ResumeLayout(false);
            this.wizardPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit5.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit3.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit4.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit1.Properties)).EndInit();
            this.completionWizardPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit2.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraWizard.WizardControl wizardControl1;
        private DevExpress.XtraWizard.WelcomeWizardPage welcomeWizardPage1;
        private DevExpress.XtraEditors.MemoEdit memoEdit1;
        private DevExpress.XtraWizard.WizardPage wizardPage1;
        private DevExpress.XtraWizard.CompletionWizardPage completionWizardPage1;
        private DevExpress.XtraEditors.MemoEdit memoEdit2;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.SpinEdit spinEdit5;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.SpinEdit spinEdit3;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.SpinEdit spinEdit4;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.SpinEdit spinEdit2;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.SpinEdit spinEdit1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
    }
}