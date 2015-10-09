namespace T7
{
    partial class frmSIDInformation
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
            DevExpress.XtraGrid.StyleFormatCondition styleFormatCondition1 = new DevExpress.XtraGrid.StyleFormatCondition();
            this.gcSIDIsReadOnly = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportSIDiSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importSIDiSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcSIDSymbol = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcSIDT7Symbol = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemLookUpEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.gcSIDInfo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcSIDMode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcSIDValue = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemLookUpEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.gcSIDAddress = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcSIDFoundSymbol = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcSIDModeDescr = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemLookUpEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.repositoryItemComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemLookUpEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemLookUpEdit3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemLookUpEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // gcSIDIsReadOnly
            // 
            this.gcSIDIsReadOnly.Caption = "Read only";
            this.gcSIDIsReadOnly.FieldName = "IsReadOnly";
            this.gcSIDIsReadOnly.Name = "gcSIDIsReadOnly";
            // 
            // gridControl1
            // 
            this.gridControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridControl1.ContextMenuStrip = this.contextMenuStrip1;
            this.gridControl1.Location = new System.Drawing.Point(0, 0);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemLookUpEdit1,
            this.repositoryItemLookUpEdit2,
            this.repositoryItemComboBox1,
            this.repositoryItemLookUpEdit3});
            this.gridControl1.Size = new System.Drawing.Size(772, 485);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportSIDiSettingsToolStripMenuItem,
            this.importSIDiSettingsToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 48);
            // 
            // exportSIDiSettingsToolStripMenuItem
            // 
            this.exportSIDiSettingsToolStripMenuItem.Name = "exportSIDiSettingsToolStripMenuItem";
            this.exportSIDiSettingsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exportSIDiSettingsToolStripMenuItem.Text = "Export SIDi settings";
            this.exportSIDiSettingsToolStripMenuItem.Click += new System.EventHandler(this.exportSIDiSettingsToolStripMenuItem_Click);
            // 
            // importSIDiSettingsToolStripMenuItem
            // 
            this.importSIDiSettingsToolStripMenuItem.Name = "importSIDiSettingsToolStripMenuItem";
            this.importSIDiSettingsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.importSIDiSettingsToolStripMenuItem.Text = "Import SIDi settings";
            this.importSIDiSettingsToolStripMenuItem.Click += new System.EventHandler(this.importSIDiSettingsToolStripMenuItem_Click);
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcSIDSymbol,
            this.gcSIDT7Symbol,
            this.gcSIDInfo,
            this.gcSIDMode,
            this.gcSIDValue,
            this.gcSIDAddress,
            this.gcSIDFoundSymbol,
            this.gcSIDIsReadOnly,
            this.gcSIDModeDescr});
            styleFormatCondition1.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            styleFormatCondition1.Appearance.Options.UseBackColor = true;
            styleFormatCondition1.ApplyToRow = true;
            styleFormatCondition1.Column = this.gcSIDIsReadOnly;
            styleFormatCondition1.Condition = DevExpress.XtraGrid.FormatConditionEnum.Equal;
            styleFormatCondition1.Value1 = true;
            this.gridView1.FormatConditions.AddRange(new DevExpress.XtraGrid.StyleFormatCondition[] {
            styleFormatCondition1});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.GroupCount = 1;
            this.gridView1.GroupSummary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Custom, "ModeDescr", null, "({0} {1} {2})")});
            this.gridView1.Name = "gridView1";
            this.gridView1.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gcSIDModeDescr, DevExpress.Data.ColumnSortOrder.Ascending)});
            this.gridView1.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gridView1_ValidatingEditor);
            this.gridView1.CustomDrawGroupRow += new DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventHandler(this.gridView1_CustomDrawGroupRow);
            this.gridView1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanged);
            this.gridView1.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.gridView1_CustomDrawCell);
            this.gridView1.ShownEditor += new System.EventHandler(this.gridView1_ShownEditor);
            this.gridView1.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gridView1_ShowingEditor);
            this.gridView1.HiddenEditor += new System.EventHandler(this.gridView1_HiddenEditor);
            // 
            // gcSIDSymbol
            // 
            this.gcSIDSymbol.Caption = "Short name";
            this.gcSIDSymbol.FieldName = "Symbol";
            this.gcSIDSymbol.Name = "gcSIDSymbol";
            this.gcSIDSymbol.Visible = true;
            this.gcSIDSymbol.VisibleIndex = 0;
            // 
            // gcSIDT7Symbol
            // 
            this.gcSIDT7Symbol.Caption = "Symbol";
            this.gcSIDT7Symbol.ColumnEdit = this.repositoryItemLookUpEdit2;
            this.gcSIDT7Symbol.FieldName = "T7Symbol";
            this.gcSIDT7Symbol.Name = "gcSIDT7Symbol";
            this.gcSIDT7Symbol.Visible = true;
            this.gcSIDT7Symbol.VisibleIndex = 1;
            // 
            // repositoryItemLookUpEdit2
            // 
            this.repositoryItemLookUpEdit2.AutoHeight = false;
            this.repositoryItemLookUpEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemLookUpEdit2.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Varname", "Symbol", 150, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Flash_start_address", "Address", 80)});
            this.repositoryItemLookUpEdit2.DisplayMember = "Varname";
            this.repositoryItemLookUpEdit2.Name = "repositoryItemLookUpEdit2";
            this.repositoryItemLookUpEdit2.ValueMember = "Varname";
            // 
            // gcSIDInfo
            // 
            this.gcSIDInfo.Caption = "Description";
            this.gcSIDInfo.FieldName = "Info";
            this.gcSIDInfo.Name = "gcSIDInfo";
            this.gcSIDInfo.OptionsColumn.AllowEdit = false;
            this.gcSIDInfo.Visible = true;
            this.gcSIDInfo.VisibleIndex = 2;
            // 
            // gcSIDMode
            // 
            this.gcSIDMode.Caption = "Mode";
            this.gcSIDMode.FieldName = "Mode";
            this.gcSIDMode.Name = "gcSIDMode";
            this.gcSIDMode.OptionsColumn.AllowEdit = false;
            // 
            // gcSIDValue
            // 
            this.gcSIDValue.Caption = "ID";
            this.gcSIDValue.ColumnEdit = this.repositoryItemLookUpEdit3;
            this.gcSIDValue.FieldName = "Value";
            this.gcSIDValue.Name = "gcSIDValue";
            this.gcSIDValue.Visible = true;
            this.gcSIDValue.VisibleIndex = 3;
            // 
            // repositoryItemLookUpEdit3
            // 
            this.repositoryItemLookUpEdit3.AutoHeight = false;
            this.repositoryItemLookUpEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemLookUpEdit3.Name = "repositoryItemLookUpEdit3";
            // 
            // gcSIDAddress
            // 
            this.gcSIDAddress.Caption = "Address";
            this.gcSIDAddress.FieldName = "AddressSRAM";
            this.gcSIDAddress.Name = "gcSIDAddress";
            this.gcSIDAddress.Visible = true;
            this.gcSIDAddress.VisibleIndex = 4;
            // 
            // gcSIDFoundSymbol
            // 
            this.gcSIDFoundSymbol.Caption = "Matched symbol";
            this.gcSIDFoundSymbol.FieldName = "FoundT7Symbol";
            this.gcSIDFoundSymbol.Name = "gcSIDFoundSymbol";
            this.gcSIDFoundSymbol.OptionsColumn.AllowEdit = false;
            this.gcSIDFoundSymbol.Visible = true;
            this.gcSIDFoundSymbol.VisibleIndex = 5;
            // 
            // gcSIDModeDescr
            // 
            this.gcSIDModeDescr.Caption = "Mode";
            this.gcSIDModeDescr.FieldName = "ModeDescr";
            this.gcSIDModeDescr.Name = "gcSIDModeDescr";
            this.gcSIDModeDescr.Visible = true;
            this.gcSIDModeDescr.VisibleIndex = 6;
            // 
            // repositoryItemLookUpEdit1
            // 
            this.repositoryItemLookUpEdit1.AutoHeight = false;
            this.repositoryItemLookUpEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemLookUpEdit1.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Symbol", "Symbol", 80, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Info", "Info", 200),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("T7Symbol", "T7Symbol", 100)});
            this.repositoryItemLookUpEdit1.DisplayMember = "Symbol";
            this.repositoryItemLookUpEdit1.Name = "repositoryItemLookUpEdit1";
            this.repositoryItemLookUpEdit1.ValueMember = "Symbol";
            this.repositoryItemLookUpEdit1.EditValueChanged += new System.EventHandler(this.repositoryItemLookUpEdit1_EditValueChanged);
            // 
            // repositoryItemComboBox1
            // 
            this.repositoryItemComboBox1.AutoHeight = false;
            this.repositoryItemComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemComboBox1.Name = "repositoryItemComboBox1";
            // 
            // simpleButton1
            // 
            this.simpleButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButton1.Location = new System.Drawing.Point(685, 491);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(75, 23);
            this.simpleButton1.TabIndex = 1;
            this.simpleButton1.Text = "Ok";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // simpleButton2
            // 
            this.simpleButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButton2.Location = new System.Drawing.Point(604, 491);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(75, 23);
            this.simpleButton2.TabIndex = 2;
            this.simpleButton2.Text = "Cancel";
            this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
            // 
            // frmSIDInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(772, 526);
            this.Controls.Add(this.simpleButton2);
            this.Controls.Add(this.simpleButton1);
            this.Controls.Add(this.gridControl1);
            this.Name = "frmSIDInformation";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SID information selection";
            this.Load += new System.EventHandler(this.frmSIDInformation_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemLookUpEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemLookUpEdit3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemLookUpEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraGrid.Columns.GridColumn gcSIDSymbol;
        private DevExpress.XtraGrid.Columns.GridColumn gcSIDT7Symbol;
        private DevExpress.XtraGrid.Columns.GridColumn gcSIDInfo;
        private DevExpress.XtraGrid.Columns.GridColumn gcSIDMode;
        private DevExpress.XtraGrid.Columns.GridColumn gcSIDValue;
        private DevExpress.XtraGrid.Columns.GridColumn gcSIDAddress;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit repositoryItemLookUpEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn gcSIDFoundSymbol;
        private DevExpress.XtraGrid.Columns.GridColumn gcSIDIsReadOnly;
        private DevExpress.XtraGrid.Columns.GridColumn gcSIDModeDescr;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit repositoryItemLookUpEdit2;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemComboBox1;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit repositoryItemLookUpEdit3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exportSIDiSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importSIDiSettingsToolStripMenuItem;
    }
}