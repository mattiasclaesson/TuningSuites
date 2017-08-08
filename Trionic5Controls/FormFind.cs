using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Be.Windows.Forms;

namespace Trionic5Controls
{
	/// <summary>
	/// Summary description for FormFind.
	/// </summary>
    public class FormFind : DevExpress.XtraEditors.XtraForm
	{
		private Be.Windows.Forms.HexBox hexBox;
		private System.Windows.Forms.TextBox txtString;
		private System.Windows.Forms.RadioButton rbString;
		private System.Windows.Forms.RadioButton rbHex;
        private System.Windows.Forms.Label label1;
        private Button btnOK;
        private Button btnCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormFind()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			rbString.CheckedChanged += new EventHandler(rb_CheckedChanged);
			rbHex.CheckedChanged += new EventHandler(rb_CheckedChanged);

//			rbString.Enter += new EventHandler(rbString_Enter);
//			rbHex.Enter += new EventHandler(rbHex_Enter);

			hexBox.ByteProvider = new DynamicByteProvider(new ByteCollection());
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFind));
            this.hexBox = new Be.Windows.Forms.HexBox();
            this.txtString = new System.Windows.Forms.TextBox();
            this.rbString = new System.Windows.Forms.RadioButton();
            this.rbHex = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // hexBox
            // 
            this.hexBox.AccessibleDescription = null;
            this.hexBox.AccessibleName = null;
            resources.ApplyResources(this.hexBox, "hexBox");
            this.hexBox.BackgroundImage = null;
            this.hexBox.LineInfoForeColor = System.Drawing.Color.Empty;
            this.hexBox.Name = "hexBox";
            this.hexBox.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            // 
            // txtString
            // 
            this.txtString.AccessibleDescription = null;
            this.txtString.AccessibleName = null;
            resources.ApplyResources(this.txtString, "txtString");
            this.txtString.BackgroundImage = null;
            this.txtString.Font = null;
            this.txtString.Name = "txtString";
            // 
            // rbString
            // 
            this.rbString.AccessibleDescription = null;
            this.rbString.AccessibleName = null;
            resources.ApplyResources(this.rbString, "rbString");
            this.rbString.BackgroundImage = null;
            this.rbString.Checked = true;
            this.rbString.Font = null;
            this.rbString.Name = "rbString";
            this.rbString.TabStop = true;
            // 
            // rbHex
            // 
            this.rbHex.AccessibleDescription = null;
            this.rbHex.AccessibleName = null;
            resources.ApplyResources(this.rbHex, "rbHex");
            this.rbHex.BackgroundImage = null;
            this.rbHex.Font = null;
            this.rbHex.Name = "rbHex";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.Font = null;
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Name = "label1";
            // 
            // btnOK
            // 
            this.btnOK.AccessibleDescription = null;
            this.btnOK.AccessibleName = null;
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.BackgroundImage = null;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOK.Font = null;
            this.btnOK.Name = "btnOK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.AccessibleDescription = null;
            this.btnCancel.AccessibleName = null;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.BackgroundImage = null;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = null;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // FormFind
            // 
            this.AcceptButton = this.btnOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            this.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Appearance.Options.UseFont = true;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rbHex);
            this.Controls.Add(this.rbString);
            this.Controls.Add(this.txtString);
            this.Controls.Add(this.hexBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormFind";
            this.Activated += new System.EventHandler(this.FormFind_Activated);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public byte[] GetFindBytes()
		{
			if(rbString.Checked)
			{
				byte[] res = System.Text.ASCIIEncoding.ASCII.GetBytes(txtString.Text);
				return res;
			}
			else
			{
				return ((DynamicByteProvider)hexBox.ByteProvider).Bytes.GetBytes();
			}
		}

		private void rb_CheckedChanged(object sender, System.EventArgs e)
		{
			txtString.Enabled = rbString.Checked;
			hexBox.Enabled = !txtString.Enabled;

			if(txtString.Enabled)
				txtString.Focus();
			else
				hexBox.Focus();
		}

		private void rbString_Enter(object sender, EventArgs e)
		{
			txtString.Focus();
		}

		private void rbHex_Enter(object sender, EventArgs e)
		{
			hexBox.Focus();
		}

		private void FormFind_Activated(object sender, System.EventArgs e)
		{
			if(rbString.Checked)
				txtString.Focus();
			else
				hexBox.Focus();
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			if(rbString.Checked && txtString.Text.Length == 0)
				DialogResult = DialogResult.Cancel;
			else if(rbHex.Checked && hexBox.ByteProvider.Length == 0)
				DialogResult = DialogResult.Cancel;
			else
				DialogResult = DialogResult.OK;
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}
	}
}
