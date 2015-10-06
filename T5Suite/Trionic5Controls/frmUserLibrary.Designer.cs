namespace Trionic5Controls
{
    partial class frmUserLibrary
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
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcLibraryFilename = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcLibraryFilenameNoPath = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcEngineType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcStage = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcInjectors = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcMapSensor = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcTorque = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcE85 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcT7Valve = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcPartnumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcSoftwareID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton3 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton4 = new DevExpress.XtraEditors.SimpleButton();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.simpleButton5 = new DevExpress.XtraEditors.SimpleButton();
            this.gcCPUSpeed = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcRamLocked = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupControl1
            // 
            this.groupControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupControl1.Controls.Add(this.gridControl1);
            this.groupControl1.Location = new System.Drawing.Point(12, 12);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(693, 355);
            this.groupControl1.TabIndex = 0;
            this.groupControl1.Text = "Files in library";
            // 
            // gridControl1
            // 
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(2, 20);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(689, 333);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcLibraryFilename,
            this.gcLibraryFilenameNoPath,
            this.gcEngineType,
            this.gcStage,
            this.gcInjectors,
            this.gcMapSensor,
            this.gcTorque,
            this.gcE85,
            this.gcT7Valve,
            this.gcPartnumber,
            this.gcSoftwareID,
            this.gcCPUSpeed,
            this.gcRamLocked});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsBehavior.Editable = false;
            this.gridView1.OptionsView.ShowGroupedColumns = true;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowIndicator = false;
            this.gridView1.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gcTorque, DevExpress.Data.ColumnSortOrder.Descending)});
            // 
            // gcLibraryFilename
            // 
            this.gcLibraryFilename.Caption = "Filename";
            this.gcLibraryFilename.FieldName = "Filename";
            this.gcLibraryFilename.Name = "gcLibraryFilename";
            // 
            // gcLibraryFilenameNoPath
            // 
            this.gcLibraryFilenameNoPath.Caption = "Filename";
            this.gcLibraryFilenameNoPath.FieldName = "FilenameNoPath";
            this.gcLibraryFilenameNoPath.Name = "gcLibraryFilenameNoPath";
            this.gcLibraryFilenameNoPath.Visible = true;
            this.gcLibraryFilenameNoPath.VisibleIndex = 0;
            // 
            // gcEngineType
            // 
            this.gcEngineType.Caption = "Engine type";
            this.gcEngineType.FieldName = "EngineType";
            this.gcEngineType.Name = "gcEngineType";
            this.gcEngineType.Visible = true;
            this.gcEngineType.VisibleIndex = 1;
            // 
            // gcStage
            // 
            this.gcStage.Caption = "Stage";
            this.gcStage.FieldName = "Stage";
            this.gcStage.Name = "gcStage";
            this.gcStage.Visible = true;
            this.gcStage.VisibleIndex = 2;
            // 
            // gcInjectors
            // 
            this.gcInjectors.Caption = "Injectors";
            this.gcInjectors.FieldName = "Injectors";
            this.gcInjectors.Name = "gcInjectors";
            this.gcInjectors.Visible = true;
            this.gcInjectors.VisibleIndex = 3;
            // 
            // gcMapSensor
            // 
            this.gcMapSensor.Caption = "Mapsensor";
            this.gcMapSensor.FieldName = "Mapsensor";
            this.gcMapSensor.Name = "gcMapSensor";
            this.gcMapSensor.Visible = true;
            this.gcMapSensor.VisibleIndex = 4;
            // 
            // gcTorque
            // 
            this.gcTorque.Caption = "Torque";
            this.gcTorque.FieldName = "Torque";
            this.gcTorque.Name = "gcTorque";
            this.gcTorque.Visible = true;
            this.gcTorque.VisibleIndex = 5;
            // 
            // gcE85
            // 
            this.gcE85.Caption = "E85";
            this.gcE85.FieldName = "E85";
            this.gcE85.Name = "gcE85";
            this.gcE85.Visible = true;
            this.gcE85.VisibleIndex = 6;
            // 
            // gcT7Valve
            // 
            this.gcT7Valve.Caption = "T7 BCV";
            this.gcT7Valve.FieldName = "T7Valve";
            this.gcT7Valve.Name = "gcT7Valve";
            this.gcT7Valve.Visible = true;
            this.gcT7Valve.VisibleIndex = 7;
            // 
            // gcPartnumber
            // 
            this.gcPartnumber.Caption = "Partnumber";
            this.gcPartnumber.FieldName = "Partnumber";
            this.gcPartnumber.Name = "gcPartnumber";
            this.gcPartnumber.Visible = true;
            this.gcPartnumber.VisibleIndex = 8;
            // 
            // gcSoftwareID
            // 
            this.gcSoftwareID.Caption = "SoftwareID";
            this.gcSoftwareID.FieldName = "SoftwareID";
            this.gcSoftwareID.Name = "gcSoftwareID";
            this.gcSoftwareID.Visible = true;
            this.gcSoftwareID.VisibleIndex = 9;
            // 
            // simpleButton1
            // 
            this.simpleButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.simpleButton1.Location = new System.Drawing.Point(630, 373);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(75, 23);
            this.simpleButton1.TabIndex = 1;
            this.simpleButton1.Text = "Close";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // simpleButton2
            // 
            this.simpleButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButton2.Location = new System.Drawing.Point(524, 373);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(100, 23);
            this.simpleButton2.TabIndex = 2;
            this.simpleButton2.Text = "Open selected";
            this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
            // 
            // simpleButton3
            // 
            this.simpleButton3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.simpleButton3.Location = new System.Drawing.Point(12, 373);
            this.simpleButton3.Name = "simpleButton3";
            this.simpleButton3.Size = new System.Drawing.Size(100, 23);
            this.simpleButton3.TabIndex = 3;
            this.simpleButton3.Text = "Add files";
            this.simpleButton3.Click += new System.EventHandler(this.simpleButton3_Click);
            // 
            // simpleButton4
            // 
            this.simpleButton4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButton4.Enabled = false;
            this.simpleButton4.Location = new System.Drawing.Point(387, 373);
            this.simpleButton4.Name = "simpleButton4";
            this.simpleButton4.Size = new System.Drawing.Size(131, 23);
            this.simpleButton4.TabIndex = 4;
            this.simpleButton4.Text = "Compare to selected";
            this.simpleButton4.Click += new System.EventHandler(this.simpleButton4_Click);
            // 
            // simpleButton5
            // 
            this.simpleButton5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.simpleButton5.Location = new System.Drawing.Point(118, 373);
            this.simpleButton5.Name = "simpleButton5";
            this.simpleButton5.Size = new System.Drawing.Size(100, 23);
            this.simpleButton5.TabIndex = 5;
            this.simpleButton5.Text = "Clear library";
            this.simpleButton5.Click += new System.EventHandler(this.simpleButton5_Click);
            // 
            // gcCPUSpeed
            // 
            this.gcCPUSpeed.Caption = "CPU";
            this.gcCPUSpeed.FieldName = "CPU";
            this.gcCPUSpeed.Name = "gcCPUSpeed";
            this.gcCPUSpeed.Visible = true;
            this.gcCPUSpeed.VisibleIndex = 10;
            // 
            // gcRamLocked
            // 
            this.gcRamLocked.Caption = "RAM locked";
            this.gcRamLocked.FieldName = "RAMlocked";
            this.gcRamLocked.Name = "gcRamLocked";
            this.gcRamLocked.Visible = true;
            this.gcRamLocked.VisibleIndex = 11;
            // 
            // frmUserLibrary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.simpleButton1;
            this.ClientSize = new System.Drawing.Size(717, 408);
            this.Controls.Add(this.simpleButton5);
            this.Controls.Add(this.simpleButton4);
            this.Controls.Add(this.simpleButton3);
            this.Controls.Add(this.simpleButton2);
            this.Controls.Add(this.simpleButton1);
            this.Controls.Add(this.groupControl1);
            this.Name = "frmUserLibrary";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "User library browser";
            this.Load += new System.EventHandler(this.frmUserLibrary_Load);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraEditors.SimpleButton simpleButton3;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.SimpleButton simpleButton4;
        private DevExpress.XtraGrid.Columns.GridColumn gcLibraryFilename;
        private DevExpress.XtraGrid.Columns.GridColumn gcLibraryFilenameNoPath;
        private DevExpress.XtraGrid.Columns.GridColumn gcEngineType;
        private DevExpress.XtraGrid.Columns.GridColumn gcStage;
        private DevExpress.XtraGrid.Columns.GridColumn gcInjectors;
        private DevExpress.XtraGrid.Columns.GridColumn gcMapSensor;
        private DevExpress.XtraGrid.Columns.GridColumn gcTorque;
        private DevExpress.XtraGrid.Columns.GridColumn gcE85;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private DevExpress.XtraGrid.Columns.GridColumn gcT7Valve;
        private DevExpress.XtraGrid.Columns.GridColumn gcPartnumber;
        private DevExpress.XtraGrid.Columns.GridColumn gcSoftwareID;
        private DevExpress.XtraEditors.SimpleButton simpleButton5;
        private DevExpress.XtraGrid.Columns.GridColumn gcCPUSpeed;
        private DevExpress.XtraGrid.Columns.GridColumn gcRamLocked;
    }
}