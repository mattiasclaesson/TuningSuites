namespace CommonSuite
{
    partial class frmSymbolSelection
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
            this.checkedListBoxControl1 = new DevExpress.XtraEditors.CheckedListBoxControl();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcSymbolsName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcSymbolUserdescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcSymbolAddress = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcSymbolLength = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemColorEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemColorEdit();
            ((System.ComponentModel.ISupportInitialize)(this.checkedListBoxControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemColorEdit1)).BeginInit();
            this.SuspendLayout();
            // 
            // checkedListBoxControl1
            // 
            this.checkedListBoxControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBoxControl1.Location = new System.Drawing.Point(12, 37);
            this.checkedListBoxControl1.Name = "checkedListBoxControl1";
            this.checkedListBoxControl1.Size = new System.Drawing.Size(432, 302);
            this.checkedListBoxControl1.TabIndex = 0;
            // 
            // simpleButton1
            // 
            this.simpleButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButton1.Location = new System.Drawing.Point(609, 413);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(75, 23);
            this.simpleButton1.TabIndex = 1;
            this.simpleButton1.Text = "Ok";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // simpleButton2
            // 
            this.simpleButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButton2.Location = new System.Drawing.Point(528, 413);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(75, 23);
            this.simpleButton2.TabIndex = 2;
            this.simpleButton2.Text = "Cancel";
            this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(12, 18);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(157, 13);
            this.labelControl1.TabIndex = 3;
            this.labelControl1.Text = "Choose items to load and display";
            // 
            // gridControl1
            // 
            this.gridControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridControl1.Location = new System.Drawing.Point(12, 37);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemColorEdit1});
            this.gridControl1.Size = new System.Drawing.Size(672, 370);
            this.gridControl1.TabIndex = 8;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcSymbolsName,
            this.gcSymbolUserdescription,
            this.gcSymbolAddress,
            this.gcSymbolLength});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsBehavior.Editable = false;
            this.gridView1.OptionsSelection.MultiSelect = true;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowIndicator = false;
            // 
            // gcSymbolsName
            // 
            this.gcSymbolsName.Caption = "Symbol";
            this.gcSymbolsName.FieldName = "Varname";
            this.gcSymbolsName.Name = "gcSymbolsName";
            this.gcSymbolsName.OptionsColumn.AllowEdit = false;
            this.gcSymbolsName.Visible = true;
            this.gcSymbolsName.VisibleIndex = 0;
            this.gcSymbolsName.Width = 125;
            // 
            // gcSymbolUserdescription
            // 
            this.gcSymbolUserdescription.Caption = "Userdescription";
            this.gcSymbolUserdescription.FieldName = "Userdescription";
            this.gcSymbolUserdescription.Name = "gcSymbolUserdescription";
            this.gcSymbolUserdescription.Visible = true;
            this.gcSymbolUserdescription.VisibleIndex = 1;
            this.gcSymbolUserdescription.Width = 301;
            // 
            // gcSymbolAddress
            // 
            this.gcSymbolAddress.Caption = "Address";
            this.gcSymbolAddress.FieldName = "Flash_start_address";
            this.gcSymbolAddress.Name = "gcSymbolAddress";
            this.gcSymbolAddress.Visible = true;
            this.gcSymbolAddress.VisibleIndex = 2;
            this.gcSymbolAddress.Width = 121;
            // 
            // gcSymbolLength
            // 
            this.gcSymbolLength.Caption = "Length";
            this.gcSymbolLength.FieldName = "Length";
            this.gcSymbolLength.Name = "gcSymbolLength";
            this.gcSymbolLength.Visible = true;
            this.gcSymbolLength.VisibleIndex = 3;
            this.gcSymbolLength.Width = 121;
            // 
            // repositoryItemColorEdit1
            // 
            this.repositoryItemColorEdit1.AutoHeight = false;
            this.repositoryItemColorEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemColorEdit1.Name = "repositoryItemColorEdit1";
            this.repositoryItemColorEdit1.StoreColorAsInteger = true;
            // 
            // frmSymbolSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(696, 446);
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.simpleButton2);
            this.Controls.Add(this.simpleButton1);
            this.Controls.Add(this.checkedListBoxControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSymbolSelection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Logfile data selection";
            ((System.ComponentModel.ISupportInitialize)(this.checkedListBoxControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemColorEdit1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.CheckedListBoxControl checkedListBoxControl1;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn gcSymbolsName;
        private DevExpress.XtraEditors.Repository.RepositoryItemColorEdit repositoryItemColorEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn gcSymbolAddress;
        private DevExpress.XtraGrid.Columns.GridColumn gcSymbolLength;
        private DevExpress.XtraGrid.Columns.GridColumn gcSymbolUserdescription;
    }
}