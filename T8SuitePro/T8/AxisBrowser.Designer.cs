namespace T8SuitePro
{
    partial class AxisBrowser
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
            this.gcBrowseSymbolName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcAxisXAxis = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcAxisYAxis = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcAxisXAxisDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcAxisYAxisDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcBrowseSymbolDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControl1
            // 
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(0, 0);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(680, 367);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            this.gridControl1.DoubleClick += new System.EventHandler(this.gridControl1_DoubleClick);
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcBrowseSymbolName,
            this.gcAxisXAxis,
            this.gcAxisYAxis,
            this.gcAxisXAxisDescription,
            this.gcAxisYAxisDescription,
            this.gcBrowseSymbolDescription});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsBehavior.Editable = false;
            this.gridView1.OptionsView.ShowAutoFilterRow = true;
            this.gridView1.OptionsView.ShowGroupedColumns = true;
            this.gridView1.OptionsView.ShowIndicator = false;
            // 
            // gcBrowseSymbolName
            // 
            this.gcBrowseSymbolName.Caption = "Symbol";
            this.gcBrowseSymbolName.FieldName = "SYMBOLNAME";
            this.gcBrowseSymbolName.Name = "gcBrowseSymbolName";
            this.gcBrowseSymbolName.Visible = true;
            this.gcBrowseSymbolName.VisibleIndex = 0;
            // 
            // gcAxisXAxis
            // 
            this.gcAxisXAxis.Caption = "X-axis";
            this.gcAxisXAxis.FieldName = "XAXIS";
            this.gcAxisXAxis.Name = "gcAxisXAxis";
            this.gcAxisXAxis.Visible = true;
            this.gcAxisXAxis.VisibleIndex = 2;
            // 
            // gcAxisYAxis
            // 
            this.gcAxisYAxis.Caption = "Y-axis";
            this.gcAxisYAxis.FieldName = "YAXIS";
            this.gcAxisYAxis.Name = "gcAxisYAxis";
            this.gcAxisYAxis.Visible = true;
            this.gcAxisYAxis.VisibleIndex = 4;
            // 
            // gcAxisXAxisDescription
            // 
            this.gcAxisXAxisDescription.Caption = "X-axis description";
            this.gcAxisXAxisDescription.FieldName = "XAXISDESCRIPTION";
            this.gcAxisXAxisDescription.Name = "gcAxisXAxisDescription";
            this.gcAxisXAxisDescription.Visible = true;
            this.gcAxisXAxisDescription.VisibleIndex = 3;
            // 
            // gcAxisYAxisDescription
            // 
            this.gcAxisYAxisDescription.Caption = "Y-axis description";
            this.gcAxisYAxisDescription.FieldName = "YAXISDESCRIPTION";
            this.gcAxisYAxisDescription.Name = "gcAxisYAxisDescription";
            this.gcAxisYAxisDescription.Visible = true;
            this.gcAxisYAxisDescription.VisibleIndex = 5;
            // 
            // gcBrowseSymbolDescription
            // 
            this.gcBrowseSymbolDescription.Caption = "Description";
            this.gcBrowseSymbolDescription.FieldName = "DESCRIPTION";
            this.gcBrowseSymbolDescription.Name = "gcBrowseSymbolDescription";
            this.gcBrowseSymbolDescription.Visible = true;
            this.gcBrowseSymbolDescription.VisibleIndex = 1;
            // 
            // AxisBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridControl1);
            this.Name = "AxisBrowser";
            this.Size = new System.Drawing.Size(680, 367);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn gcBrowseSymbolName;
        private DevExpress.XtraGrid.Columns.GridColumn gcAxisXAxis;
        private DevExpress.XtraGrid.Columns.GridColumn gcAxisYAxis;
        private DevExpress.XtraGrid.Columns.GridColumn gcAxisXAxisDescription;
        private DevExpress.XtraGrid.Columns.GridColumn gcAxisYAxisDescription;
        private DevExpress.XtraGrid.Columns.GridColumn gcBrowseSymbolDescription;
    }
}
