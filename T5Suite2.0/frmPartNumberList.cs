using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using Trionic5Tools;

namespace T5Suite2
{
    public partial class frmPartNumberList : DevExpress.XtraEditors.XtraForm
    {
        
        private string m_selectedpartnumber = "";
        private DataTable partnumbers = new DataTable();
        public string Selectedpartnumber
        {
            get { return m_selectedpartnumber; }
            set { m_selectedpartnumber = value; }
        }
        Trionic5File trionic5file = new Trionic5File();

        public frmPartNumberList()
        {
            InitializeComponent();
            partnumbers.Columns.Add("FILENAME");
            partnumbers.Columns.Add("PARTNUMBER");
            partnumbers.Columns.Add("ENGINETYPE");
            partnumbers.Columns.Add("CARTYPE");
            partnumbers.Columns.Add("TUNER");
            partnumbers.Columns.Add("STAGE");
            partnumbers.Columns.Add("INFO");
            partnumbers.Columns.Add("SPEED");
            //partnumbers.Columns.Add("TYPE");
            //backgroundWorker1.RunWorkerAsync();
        }

        private void LoadPartNumbersFromFiles()
        {
            if (Directory.Exists(Application.StartupPath + "\\Binaries"))
            {
                
                string[] binfiles = Directory.GetFiles(Application.StartupPath + "\\Binaries", "*.BIN");
                foreach (string binfile in binfiles)
                {
                    //FileInfo fi = new FileInfo(binfile);
                    //string fileIdent = "T5.5";
                    //if (fi.Length == 0x20000) fileIdent = "T5.2";
                    string speed = "16";
                    if (Find20MhzSequence(binfile)) speed = "20";
                    string binfilename = Path.GetFileNameWithoutExtension(binfile);
                    string partnumber = "";

                    string enginetype = "";
                    string cartype = "";
                    string tuner = "";
                    string stage = "";
                    string additionalinfo = "";
                    string softwareid = "";

                    if (binfilename.Contains("-"))
                    {
                        char[] sep = new char[1];
                        sep.SetValue('-', 0);
                        string[] values = binfilename.Split(sep);
                        if (values.Length == 1)
                        {
                            // assume partnumber
                            partnumber = (string)binfilename;
                            partnumbers.Rows.Add(binfile, partnumber, enginetype, cartype, tuner, stage, additionalinfo, speed);
                        }
                        if (values.Length == 2)
                        {
                            // assume partnumber-softwareid
                            partnumber = (string)values.GetValue(0);
                            softwareid = (string)values.GetValue(1);
                            partnumbers.Rows.Add(binfile, partnumber, enginetype, cartype, tuner, stage, additionalinfo, speed);                   
                        }
                        else if (values.Length == 3)
                        {
                            cartype = (string)values.GetValue(0);
                            enginetype = (string)values.GetValue(1);
                            partnumber = (string)values.GetValue(2);
                            partnumbers.Rows.Add(binfile, partnumber, enginetype, cartype, tuner, stage, additionalinfo, speed);
                        }
                        else if (values.Length == 4)
                        {
                            cartype = (string)values.GetValue(0);
                            enginetype = (string)values.GetValue(1);
                            partnumber = (string)values.GetValue(2);
                            tuner = (string)values.GetValue(3);
                            partnumbers.Rows.Add(binfile, partnumber, enginetype, cartype, tuner, stage, additionalinfo, speed);
                        }
                        else if (values.Length == 5)
                        {
                            cartype = (string)values.GetValue(0);
                            enginetype = (string)values.GetValue(1);
                            partnumber = (string)values.GetValue(2);
                            tuner = (string)values.GetValue(3);
                            stage = (string)values.GetValue(4);
                            partnumbers.Rows.Add(binfile, partnumber, enginetype, cartype, tuner, stage, additionalinfo, speed);
                        }
                        else if (values.Length > 5)
                        {
                            cartype = (string)values.GetValue(0);
                            enginetype = (string)values.GetValue(1);
                            partnumber = (string)values.GetValue(2);
                            tuner = (string)values.GetValue(3);
                            stage = (string)values.GetValue(4);
                            for (int tel = 5; tel < values.Length; tel++)
                            {
                                additionalinfo += (string)values.GetValue(tel) + " ";
                            }
                            partnumbers.Rows.Add(binfile, partnumber, enginetype, cartype, tuner, stage, additionalinfo, speed);
                        }
                    }
                    else
                    {
                        // assume partnumber
                        partnumber = (string)binfilename;
                        ///////////////// temporary conversion code
                        softwareid = (string)trionic5file.GetSoftwareVersion(binfile);
                        string outputfile = Path.GetDirectoryName(binfile);
                        outputfile = Path.Combine(outputfile, Path.GetFileNameWithoutExtension(binfile) + "-" + softwareid + ".BIN");
                        File.Move(binfile, outputfile);
                        ///////////////// end temporary conversion code
                        partnumbers.Rows.Add(binfile, partnumber, enginetype, cartype, tuner, stage, additionalinfo, speed);
                    }
                   // backgroundWorker1.ReportProgress(0);
                    Application.DoEvents();
                }
            }
            
        }

        private void frmPartNumberList_Load(object sender, EventArgs e)
        {
            PartnumberCollection pnc = new PartnumberCollection();
            DataTable dt = pnc.GeneratePartNumberCollection();
            //dt.Columns.Add("Filename");
            //dt.Columns.Add("Tuner");
            //dt.Columns.Add("Stage");
            //dt.Columns.Add("Info");

            LoadPartNumbersFromFiles();

            /*foreach (DataRow dr in dt.Rows)
            {
                foreach (DataRow pdr in partnumbers.Rows)
                {
                    if(dr["Partnumber"] != DBNull.Value && pdr["PARTNUMBER"] != DBNull.Value)
                    {
                        if (dr["Partnumber"].ToString() == pdr["PARTNUMBER"].ToString())
                        {
                            dr["Filename"] = pdr["FILENAME"];
                            dr["Tuner"] = pdr["TUNER"];
                            dr["Stage"] = pdr["STAGE"];
                            dr["Info"] = pdr["INFO"];
                        }
                    }
                }
            }*/


            gridControl1.DataSource = dt;
            gridView1.Columns["Carmodel"].Group();
            gridView1.Columns["Enginetype"].Group();
            gridView1.BestFitColumns();
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            int[] rows = gridView1.GetSelectedRows();
            if(rows.Length > 0)
            {
                m_selectedpartnumber = (string)gridView1.GetRowCellValue((int)rows.GetValue(0), "Partnumber");
                if (m_selectedpartnumber != null)
                {
                    if (m_selectedpartnumber != string.Empty)
                    {

                        this.Close();
                    }
                }
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            int[] rows = gridView1.GetSelectedRows();
            if (rows.Length > 0)
            {
                m_selectedpartnumber = (string)gridView1.GetRowCellValue((int)rows.GetValue(0), "Partnumber");
            }
            this.Close();
        }


        private string CheckTypeInAvailableLibrary(string partnumber)
        {
            string retval = "";
            foreach (DataRow dr in partnumbers.Rows)
            {
                if (dr["PARTNUMBER"] != DBNull.Value)
                {
                    if (dr["PARTNUMBER"].ToString() == partnumber)
                    {
                        retval = dr["Type"].ToString();
                        break;
                    }
                }
            }
            return retval;
        }
        private int CheckInAvailableLibrary(string partnumber)
        {
            int retval = 0;
            foreach (DataRow dr in partnumbers.Rows)
            {
                if (dr["PARTNUMBER"] != DBNull.Value)
                {
                    if (dr["PARTNUMBER"].ToString() == partnumber)
                    {
                        retval = 1;
                        if (dr["SPEED"].ToString() == "20")
                        {
                            retval = 2;
                        }
                        break;
                    }
                }
            }
            return retval;
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName == "Partnumber")
            {
                if (e.CellValue != null)
                {
                    if (e.CellValue != DBNull.Value)
                    {
                        int type = CheckInAvailableLibrary(e.CellValue.ToString());
                        if (type == 1)
                        {
                            e.Graphics.FillRectangle(Brushes.YellowGreen, e.Bounds);
                        }
                        if (type == 2)
                        {
                            e.Graphics.FillRectangle(Brushes.Orange, e.Bounds);
                        }
                    }
                }
            }
            /*if (e.Column.FieldName == "Type")
            {
                if (e.CellValue != null)
                {
                    if (e.CellValue != DBNull.Value)
                    {
                        string type = CheckTypeInAvailableLibrary(gridView1.GetRowCellValue(e.RowHandle, gridView1.Columns["Partnumber"]).ToString());
                        e.DisplayText = type;
                      
                    }
                }
            }*/
        }

        private bool Find20MhzSequence(string filename)
        {
            bool retval = false;
            FileInfo fi = new FileInfo(filename);
            using (FileStream a_fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                byte[] sequence = new byte[32] {0x02, 0x39, 0x00, 0xBF, 0x00, 0xFF, 0xFA, 0x04,
                                            0x00, 0x39, 0x00, 0x80, 0x00, 0xFF, 0xFA, 0x04,
                                            0x02, 0x39, 0x00, 0xC0, 0x00, 0xFF, 0xFA, 0x04,
                                            0x00, 0x39, 0x00, 0x13, 0x00, 0xFF, 0xFA, 0x04};
                /*byte[] seq_mask = new byte[32] {1, 1, 1, 1, 1, 1, 1, 1,
                                            0, 0, 1, 1, 1, 0, 0, 0,   
                                            1, 1, 1, 1, 0, 0, 1, 1,
                                            1, 1, 1, 1, 0, 0, 1, 1};*/
                byte data;
                int i;
                i = 0;
                while (a_fileStream.Position < fi.Length -1)
                {
                    data = (byte)a_fileStream.ReadByte();
                    if (data == sequence[i])
                    {
                        i++;
                    }
                    else
                    {
                        i = 0;
                    }
                    if (i == sequence.Length) break;
                }
                if (i == sequence.Length)
                {
                    retval = true;
                }
            }
            return retval;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
    }
}