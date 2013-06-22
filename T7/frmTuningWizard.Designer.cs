namespace T7
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTuningWizard));
            this.wizardControl1 = new DevExpress.XtraWizard.WizardControl();
            this.welcomeWizardPage1 = new DevExpress.XtraWizard.WelcomeWizardPage();
            this.memoEdit1 = new DevExpress.XtraEditors.MemoEdit();
            this.wizardPage1 = new DevExpress.XtraWizard.WizardPage();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.memoEdit3 = new DevExpress.XtraEditors.MemoEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.spinEdit2 = new DevExpress.XtraEditors.SpinEdit();
            this.spinEdit1 = new DevExpress.XtraEditors.SpinEdit();
            this.checkEdit1 = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.completionWizardPage1 = new DevExpress.XtraWizard.CompletionWizardPage();
            this.memoEdit2 = new DevExpress.XtraEditors.MemoEdit();
            this.memoEdit4 = new DevExpress.XtraEditors.MemoEdit();
            this.memoEdit5 = new DevExpress.XtraEditors.MemoEdit();
            ((System.ComponentModel.ISupportInitialize)(this.wizardControl1)).BeginInit();
            this.wizardControl1.SuspendLayout();
            this.welcomeWizardPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit1.Properties)).BeginInit();
            this.wizardPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit3.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit1.Properties)).BeginInit();
            this.completionWizardPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit4.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit5.Properties)).BeginInit();
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
            this.welcomeWizardPage1.Controls.Add(this.memoEdit4);
            this.welcomeWizardPage1.Controls.Add(this.memoEdit1);
            this.welcomeWizardPage1.IntroductionText = "This wizard will help you tuning your binary for a certain amount of torque and p" +
                "ower.";
            this.welcomeWizardPage1.Name = "welcomeWizardPage1";
            this.welcomeWizardPage1.Size = new System.Drawing.Size(573, 431);
            this.welcomeWizardPage1.Text = "Welcome to the Tune me up ® Wizard";
            // 
            // memoEdit1
            // 
            this.memoEdit1.EditValue = resources.GetString("memoEdit1.EditValue");
            this.memoEdit1.Location = new System.Drawing.Point(70, 146);
            this.memoEdit1.Name = "memoEdit1";
            this.memoEdit1.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.memoEdit1.Properties.Appearance.ForeColor = System.Drawing.Color.Firebrick;
            this.memoEdit1.Properties.Appearance.Options.UseBackColor = true;
            this.memoEdit1.Properties.Appearance.Options.UseForeColor = true;
            this.memoEdit1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.memoEdit1.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.memoEdit1.Size = new System.Drawing.Size(434, 81);
            this.memoEdit1.TabIndex = 1;
            // 
            // wizardPage1
            // 
            this.wizardPage1.Controls.Add(this.labelControl4);
            this.wizardPage1.Controls.Add(this.memoEdit3);
            this.wizardPage1.Controls.Add(this.labelControl3);
            this.wizardPage1.Controls.Add(this.labelControl2);
            this.wizardPage1.Controls.Add(this.spinEdit2);
            this.wizardPage1.Controls.Add(this.spinEdit1);
            this.wizardPage1.Controls.Add(this.checkEdit1);
            this.wizardPage1.Controls.Add(this.labelControl1);
            this.wizardPage1.DescriptionText = "Please select the preferred options below.";
            this.wizardPage1.Name = "wizardPage1";
            this.wizardPage1.Size = new System.Drawing.Size(758, 419);
            this.wizardPage1.Text = "Tuning options";
            this.wizardPage1.PageCommit += new System.EventHandler(this.wizardPage1_PageCommit);
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(467, 161);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(13, 13);
            this.labelControl4.TabIndex = 6;
            this.labelControl4.Text = "HP";
            // 
            // memoEdit3
            // 
            this.memoEdit3.EditValue = "Please make sure you enter sensible numbers for torque and power!";
            this.memoEdit3.Location = new System.Drawing.Point(205, 252);
            this.memoEdit3.Name = "memoEdit3";
            this.memoEdit3.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.memoEdit3.Properties.Appearance.ForeColor = System.Drawing.Color.Firebrick;
            this.memoEdit3.Properties.Appearance.Options.UseBackColor = true;
            this.memoEdit3.Properties.Appearance.Options.UseForeColor = true;
            this.memoEdit3.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.memoEdit3.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.memoEdit3.Size = new System.Drawing.Size(359, 26);
            this.memoEdit3.TabIndex = 2;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(467, 119);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(15, 13);
            this.labelControl3.TabIndex = 5;
            this.labelControl3.Text = "Nm";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(240, 119);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(79, 13);
            this.labelControl2.TabIndex = 4;
            this.labelControl2.Text = "Maximum torque";
            // 
            // spinEdit2
            // 
            this.spinEdit2.EditValue = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.spinEdit2.Location = new System.Drawing.Point(381, 112);
            this.spinEdit2.Name = "spinEdit2";
            this.spinEdit2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit2.Properties.IsFloatValue = false;
            this.spinEdit2.Properties.Mask.EditMask = "N00";
            this.spinEdit2.Properties.MaxValue = new decimal(new int[] {
            800,
            0,
            0,
            0});
            this.spinEdit2.Properties.MinValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.spinEdit2.Size = new System.Drawing.Size(70, 20);
            this.spinEdit2.TabIndex = 3;
            // 
            // spinEdit1
            // 
            this.spinEdit1.EditValue = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.spinEdit1.Location = new System.Drawing.Point(381, 154);
            this.spinEdit1.Name = "spinEdit1";
            this.spinEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit1.Properties.IsFloatValue = false;
            this.spinEdit1.Properties.Mask.EditMask = "N00";
            this.spinEdit1.Properties.MaxValue = new decimal(new int[] {
            450,
            0,
            0,
            0});
            this.spinEdit1.Properties.MinValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.spinEdit1.Size = new System.Drawing.Size(70, 20);
            this.spinEdit1.TabIndex = 0;
            // 
            // checkEdit1
            // 
            this.checkEdit1.Location = new System.Drawing.Point(379, 189);
            this.checkEdit1.Name = "checkEdit1";
            this.checkEdit1.Properties.Caption = "Car runs E85";
            this.checkEdit1.Size = new System.Drawing.Size(137, 18);
            this.checkEdit1.TabIndex = 2;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(240, 161);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(112, 13);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "Maximum engine power";
            // 
            // completionWizardPage1
            // 
            this.completionWizardPage1.Controls.Add(this.memoEdit5);
            this.completionWizardPage1.Controls.Add(this.memoEdit2);
            this.completionWizardPage1.Name = "completionWizardPage1";
            this.completionWizardPage1.Size = new System.Drawing.Size(573, 431);
            // 
            // memoEdit2
            // 
            this.memoEdit2.EditValue = resources.GetString("memoEdit2.EditValue");
            this.memoEdit2.Location = new System.Drawing.Point(70, 146);
            this.memoEdit2.Name = "memoEdit2";
            this.memoEdit2.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.memoEdit2.Properties.Appearance.ForeColor = System.Drawing.Color.Firebrick;
            this.memoEdit2.Properties.Appearance.Options.UseBackColor = true;
            this.memoEdit2.Properties.Appearance.Options.UseForeColor = true;
            this.memoEdit2.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.memoEdit2.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.memoEdit2.Size = new System.Drawing.Size(434, 65);
            this.memoEdit2.TabIndex = 2;
            // 
            // memoEdit4
            // 
            this.memoEdit4.EditValue = "";
            this.memoEdit4.Location = new System.Drawing.Point(70, 233);
            this.memoEdit4.Name = "memoEdit4";
            this.memoEdit4.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.memoEdit4.Properties.Appearance.ForeColor = System.Drawing.Color.RoyalBlue;
            this.memoEdit4.Properties.Appearance.Options.UseBackColor = true;
            this.memoEdit4.Properties.Appearance.Options.UseForeColor = true;
            this.memoEdit4.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.memoEdit4.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.memoEdit4.Size = new System.Drawing.Size(434, 103);
            this.memoEdit4.TabIndex = 2;
            // 
            // memoEdit5
            // 
            this.memoEdit5.EditValue = "Settings to review";
            this.memoEdit5.Location = new System.Drawing.Point(70, 217);
            this.memoEdit5.Name = "memoEdit5";
            this.memoEdit5.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.memoEdit5.Properties.Appearance.ForeColor = System.Drawing.Color.RoyalBlue;
            this.memoEdit5.Properties.Appearance.Options.UseBackColor = true;
            this.memoEdit5.Properties.Appearance.Options.UseForeColor = true;
            this.memoEdit5.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.memoEdit5.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.memoEdit5.Size = new System.Drawing.Size(434, 103);
            this.memoEdit5.TabIndex = 3;
            // 
            // frmTuningWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 564);
            this.ControlBox = false;
            this.Controls.Add(this.wizardControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmTuningWizard";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tuning wizard";
            ((System.ComponentModel.ISupportInitialize)(this.wizardControl1)).EndInit();
            this.wizardControl1.ResumeLayout(false);
            this.welcomeWizardPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit1.Properties)).EndInit();
            this.wizardPage1.ResumeLayout(false);
            this.wizardPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit3.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit1.Properties)).EndInit();
            this.completionWizardPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit4.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit5.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraWizard.WizardControl wizardControl1;
        private DevExpress.XtraWizard.WelcomeWizardPage welcomeWizardPage1;
        private DevExpress.XtraEditors.MemoEdit memoEdit1;
        private DevExpress.XtraWizard.WizardPage wizardPage1;
        private DevExpress.XtraWizard.CompletionWizardPage completionWizardPage1;
        private DevExpress.XtraEditors.MemoEdit memoEdit2;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.SpinEdit spinEdit2;
        private DevExpress.XtraEditors.CheckEdit checkEdit1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SpinEdit spinEdit1;
        private DevExpress.XtraEditors.MemoEdit memoEdit3;
        private DevExpress.XtraEditors.MemoEdit memoEdit4;
        private DevExpress.XtraEditors.MemoEdit memoEdit5;
    }
}