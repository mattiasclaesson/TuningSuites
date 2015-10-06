namespace T5Suite2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AxisBrowser));
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
            this.gridControl1.AccessibleDescription = null;
            this.gridControl1.AccessibleName = null;
            resources.ApplyResources(this.gridControl1, "gridControl1");
            this.gridControl1.BackgroundImage = null;
            this.gridControl1.EmbeddedNavigator.AccessibleDescription = null;
            this.gridControl1.EmbeddedNavigator.AccessibleName = null;
            this.gridControl1.EmbeddedNavigator.AllowHtmlTextInToolTip = ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("gridControl1.EmbeddedNavigator.AllowHtmlTextInToolTip")));
            this.gridControl1.EmbeddedNavigator.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("gridControl1.EmbeddedNavigator.Anchor")));
            this.gridControl1.EmbeddedNavigator.BackgroundImage = null;
            this.gridControl1.EmbeddedNavigator.BackgroundImageLayout = ((System.Windows.Forms.ImageLayout)(resources.GetObject("gridControl1.EmbeddedNavigator.BackgroundImageLayout")));
            this.gridControl1.EmbeddedNavigator.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("gridControl1.EmbeddedNavigator.ImeMode")));
            this.gridControl1.EmbeddedNavigator.TextLocation = ((DevExpress.XtraEditors.NavigatorButtonsTextLocation)(resources.GetObject("gridControl1.EmbeddedNavigator.TextLocation")));
            this.gridControl1.EmbeddedNavigator.ToolTip = resources.GetString("gridControl1.EmbeddedNavigator.ToolTip");
            this.gridControl1.EmbeddedNavigator.ToolTipIconType = ((DevExpress.Utils.ToolTipIconType)(resources.GetObject("gridControl1.EmbeddedNavigator.ToolTipIconType")));
            this.gridControl1.EmbeddedNavigator.ToolTipTitle = resources.GetString("gridControl1.EmbeddedNavigator.ToolTipTitle");
            this.gridControl1.Font = null;
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            this.gridControl1.DoubleClick += new System.EventHandler(this.gridControl1_DoubleClick);
            // 
            // gridView1
            // 
            resources.ApplyResources(this.gridView1, "gridView1");
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
            resources.ApplyResources(this.gcBrowseSymbolName, "gcBrowseSymbolName");
            this.gcBrowseSymbolName.FieldName = "SYMBOLNAME";
            this.gcBrowseSymbolName.Name = "gcBrowseSymbolName";
            // 
            // gcAxisXAxis
            // 
            resources.ApplyResources(this.gcAxisXAxis, "gcAxisXAxis");
            this.gcAxisXAxis.FieldName = "XAXIS";
            this.gcAxisXAxis.Name = "gcAxisXAxis";
            // 
            // gcAxisYAxis
            // 
            resources.ApplyResources(this.gcAxisYAxis, "gcAxisYAxis");
            this.gcAxisYAxis.FieldName = "YAXIS";
            this.gcAxisYAxis.Name = "gcAxisYAxis";
            // 
            // gcAxisXAxisDescription
            // 
            resources.ApplyResources(this.gcAxisXAxisDescription, "gcAxisXAxisDescription");
            this.gcAxisXAxisDescription.FieldName = "XAXISDESCRIPTION";
            this.gcAxisXAxisDescription.Name = "gcAxisXAxisDescription";
            // 
            // gcAxisYAxisDescription
            // 
            resources.ApplyResources(this.gcAxisYAxisDescription, "gcAxisYAxisDescription");
            this.gcAxisYAxisDescription.FieldName = "YAXISDESCRIPTION";
            this.gcAxisYAxisDescription.Name = "gcAxisYAxisDescription";
            // 
            // gcBrowseSymbolDescription
            // 
            resources.ApplyResources(this.gcBrowseSymbolDescription, "gcBrowseSymbolDescription");
            this.gcBrowseSymbolDescription.FieldName = "DESCRIPTION";
            this.gcBrowseSymbolDescription.Name = "gcBrowseSymbolDescription";
            // 
            // AxisBrowser
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.gridControl1);
            this.Name = "AxisBrowser";
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
