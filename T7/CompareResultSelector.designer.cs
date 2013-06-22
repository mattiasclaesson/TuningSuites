namespace T7
{
    partial class CompareResultSelector
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcFilename = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcPartnumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcSoftwareID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcNumberOfSymbols = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcFullFilename = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControl1
            // 
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.EmbeddedNavigator.Name = "";
            this.gridControl1.FormsUseDefaultLookAndFeel = false;
            this.gridControl1.Location = new System.Drawing.Point(0, 0);
            this.gridControl1.LookAndFeel.SkinName = "Black";
            this.gridControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(535, 353);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcFilename,
            this.gcPartnumber,
            this.gcSoftwareID,
            this.gcNumberOfSymbols,
            this.gcFullFilename});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsBehavior.Editable = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowIndicator = false;
            this.gridView1.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gcNumberOfSymbols, DevExpress.Data.ColumnSortOrder.Ascending)});
            this.gridView1.DoubleClick += new System.EventHandler(this.gridView1_DoubleClick);
            // 
            // gcFilename
            // 
            this.gcFilename.Caption = "Filename";
            this.gcFilename.FieldName = "FILENAME";
            this.gcFilename.Name = "gcFilename";
            this.gcFilename.Visible = true;
            this.gcFilename.VisibleIndex = 0;
            this.gcFilename.Width = 208;
            // 
            // gcPartnumber
            // 
            this.gcPartnumber.Caption = "Partnumber";
            this.gcPartnumber.FieldName = "PARTNUMBER";
            this.gcPartnumber.Name = "gcPartnumber";
            this.gcPartnumber.Width = 66;
            // 
            // gcSoftwareID
            // 
            this.gcSoftwareID.Caption = "Software ID";
            this.gcSoftwareID.FieldName = "SOFTWAREID";
            this.gcSoftwareID.Name = "gcSoftwareID";
            this.gcSoftwareID.Width = 134;
            // 
            // gcNumberOfSymbols
            // 
            this.gcNumberOfSymbols.Caption = "Number of symbols";
            this.gcNumberOfSymbols.FieldName = "NUMBEROFSYMBOLS";
            this.gcNumberOfSymbols.Name = "gcNumberOfSymbols";
            this.gcNumberOfSymbols.Visible = true;
            this.gcNumberOfSymbols.VisibleIndex = 1;
            this.gcNumberOfSymbols.Width = 106;
            // 
            // gcFullFilename
            // 
            this.gcFullFilename.Caption = "Full filename";
            this.gcFullFilename.FieldName = "FULLFILENAME";
            this.gcFullFilename.Name = "gcFullFilename";
            // 
            // CompareResultSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridControl1);
            this.LookAndFeel.SkinName = "Black";
            this.Name = "CompareResultSelector";
            this.Size = new System.Drawing.Size(535, 353);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn gcFilename;
        private DevExpress.XtraGrid.Columns.GridColumn gcPartnumber;
        private DevExpress.XtraGrid.Columns.GridColumn gcSoftwareID;
        private DevExpress.XtraGrid.Columns.GridColumn gcNumberOfSymbols;
        public DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Columns.GridColumn gcFullFilename;
    }
}
