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
using CommonSuite;

namespace Trionic5Controls
{
    public partial class frmUserLibrary : DevExpress.XtraEditors.XtraForm
    {

        private string m_open_File = "";

        private string m_compare_File = "";

        public string Compare_File
        {
            get { return m_compare_File; }
            set { m_compare_File = value; }
        }

        public string Open_File
        {
            get { return m_open_File; }
            set { m_open_File = value; }
        }

        public frmUserLibrary()
        {
            InitializeComponent();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            m_open_File = gridView1.GetRowCellValue(gridView1.FocusedRowHandle, gcLibraryFilename).ToString();
            //TODO: Set correct filename
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            m_compare_File = gridView1.GetRowCellValue(gridView1.FocusedRowHandle, gcLibraryFilename).ToString();
            //TODO: Set correct filename
            DialogResult = DialogResult.OK;
            this.Close();
        }

        public void EnableCompareButton()
        {
            simpleButton4.Enabled = true;
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                AddFilesToLibrary(folderBrowserDialog1.SelectedPath);
            }
        }

        private void AddFilesToLibrary(string path)
        {
            Application.DoEvents();
            DataTable dt = new DataTable("UserLib");
            dt.Columns.Add("Filename");
            dt.Columns.Add("FilenameNoPath");
            dt.Columns.Add("EngineType");
            dt.Columns.Add("Stage");
            dt.Columns.Add("Injectors");
            dt.Columns.Add("Mapsensor");
            dt.Columns.Add("Torque");
            dt.Columns.Add("E85", Type.GetType("System.Boolean"));
            dt.Columns.Add("T7Valve", Type.GetType("System.Boolean"));
            dt.Columns.Add("Partnumber");
            dt.Columns.Add("SoftwareID");
            dt.Columns.Add("CPU");
            dt.Columns.Add("RAMlocked", Type.GetType("System.Boolean"));
            if (File.Exists(Application.StartupPath + "\\UserLib.xml"))
            {
                dt.ReadXml(Application.StartupPath + "\\UserLib.xml");
            }
            string[] files = Directory.GetFiles(path, "*.bin", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                this.Text = "User library browser - " + Path.GetFileName(file);
                Application.DoEvents();
                if (fi.Length == 0x20000 || fi.Length == 0x40000)
                {
                    try
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (dr["Filename"] != DBNull.Value)
                            {
                                if (dr["Filename"].ToString() == file)
                                {
                                    dt.Rows.Remove(dr);
                                    break;
                                }
                            }
                        }
                        Trionic5File _file = new Trionic5File();
                        _file.SelectFile(file);
                        Trionic5FileInformation m_trionicFileInformation = _file.ParseFile();
                        Trionic5Properties props = _file.GetTrionicProperties();
                        // get information about the file
                        MapSensorType mapsensor = _file.GetMapSensorType(true);
                        ECUFileType fileType = _file.DetermineFileType();
                        //t5p.Enginetype
                        //t5p.Partnumber
                        //t5p.CPUspeed
                        //t5p.SoftwareID
                        //t5p.RAMlocked
                        //TuningStage _stage = m_trionicFile.DetermineTuningStage(out m_maxBoost);
                        int injkonst = _file.GetSymbolAsInt("Inj_konst!");
                        bool m_E85 = false;
                        int max_injection = _file.GetMaxInjection();
                        max_injection *= injkonst;
                        // de maximale waarde uit fuel_map_x_axis! aub
                        byte[] fuelxaxis = _file.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash("Fuel_map_xaxis!"), (uint)m_trionicFileInformation.GetSymbolLength("Fuel_map_xaxis!"));
                        int max_value_x_axis = Convert.ToInt32(fuelxaxis.GetValue(fuelxaxis.Length - 1));
                        if (mapsensor == MapSensorType.MapSensor30)
                        {
                            max_value_x_axis *= 120;
                            max_value_x_axis /= 100;
                        }
                        else if (mapsensor == MapSensorType.MapSensor35)
                        {
                            max_value_x_axis *= 140;
                            max_value_x_axis /= 100;
                        }
                        else if (mapsensor == MapSensorType.MapSensor40)
                        {
                            max_value_x_axis *= 160;
                            max_value_x_axis /= 100;
                        }
                        else if (mapsensor == MapSensorType.MapSensor50)
                        {
                            max_value_x_axis *= 200;
                            max_value_x_axis /= 100;
                        }
                        //Console.WriteLine("max x: " + max_value_x_axis.ToString());
                        float max_support_boost = max_value_x_axis;
                        max_support_boost /= 100;
                        max_support_boost -= 1;
                        float corr_inj = 1.4F / max_support_boost;
                        corr_inj *= 100;
                        //Console.WriteLine("corr_inj = " + corr_inj.ToString());
                        max_injection *= (int)corr_inj;
                        max_injection /= 100;

                        // dtReport.Rows.Add("Max injection: "+ max_injection.ToString());
                        if (max_injection > 7500) m_E85 = true;
                        if (injkonst > 26)
                        {
                            m_E85 = true;
                        }
                        //TODO: nog extra controleren of er andere indicatoren zijn of er E85 gebruikt wordt
                        // we kunnen dit aan de start verrijkingen zien en aan de ontstekingstijdstippen bij 
                        // vollast (ontsteking scherper), let op want dit laatste is bij W/M injectie ook zo.
                        // de een na laatste waarde uit Eftersta_fak! geeft een duidelijke indicatie
                        byte[] eftstafak = _file.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash("Eftersta_fak!"), (uint)m_trionicFileInformation.GetSymbolLength("Eftersta_fak!"));
                        if (eftstafak.Length == 15)
                        {
                            int eftstafakvalue = Convert.ToInt32(eftstafak.GetValue(13));
                            if (eftstafakvalue > 170) m_E85 = true;
                        }
                        if (m_E85)
                        {
                            max_injection *= 10;
                            max_injection /= 14;
                            // dtReport.Rows.Add("Probable fuel: E85");
                        }
                        // get peak from insp_mat and multiply by injector constant
                        InjectorType inj_type = InjectorType.Stock;
                        if (max_injection > 5000) inj_type = InjectorType.Stock;
                        else if (max_injection > 3500) inj_type = InjectorType.GreenGiants;
                        else if (max_injection > 2000) inj_type = InjectorType.Siemens630Dekas;
                        else if (max_injection > 1565) inj_type = InjectorType.Siemens875Dekas;
                        else inj_type = InjectorType.Siemens1000cc;



                        // Add info about T5/T7 valve
                        int frek230 = _file.GetSymbolAsInt("Frek_230!");
                        int frek250 = _file.GetSymbolAsInt("Frek_250!");
                        bool T7Valve = false;
                        if (fileType == ECUFileType.Trionic52File)
                        {
                            if (frek230 == 728 || frek250 == 935)
                            {
                                T7Valve = false;
                            }
                            else
                            {
                                T7Valve = true;
                            }
                        }
                        else if (fileType == ECUFileType.Trionic55File)
                        {
                            if (frek230 == 90 || frek250 == 70)
                            {
                                T7Valve = false;
                            }
                            else
                            {
                                T7Valve = true;
                            }
                        }
                        int torque = 0;
                        float m_maxBoost = 0;
                        TuningStage _stage = _file.DetermineTuningStage(out m_maxBoost);

                        PressureToTorque ptt = new PressureToTorque();
                        torque = Convert.ToInt32(ptt.CalculateTorqueFromPressure(m_maxBoost, props.TurboType));
                        dt.Rows.Add(file, Path.GetFileName(file), props.Enginetype, _stage.ToString(), inj_type.ToString(), mapsensor.ToString(), torque.ToString(), m_E85, T7Valve, props.Partnumber, props.SoftwareID, props.CPUspeed, props.RAMlocked);
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                }
            }
            dt.WriteXml(Application.StartupPath + "\\UserLib.xml");
            gridControl1.DataSource = dt;
            gridView1.BestFitColumns();
            this.Text = "User library browser";
        }

        private void frmUserLibrary_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable("UserLib");
            dt.Columns.Add("Filename");
            dt.Columns.Add("FilenameNoPath");
            dt.Columns.Add("EngineType");
            dt.Columns.Add("Stage");
            dt.Columns.Add("Injectors");
            dt.Columns.Add("Mapsensor");
            dt.Columns.Add("Torque");
            dt.Columns.Add("E85", Type.GetType("System.Boolean"));
            dt.Columns.Add("T7Valve", Type.GetType("System.Boolean"));
            dt.Columns.Add("Partnumber");
            dt.Columns.Add("SoftwareID");
            dt.Columns.Add("CPU");
            dt.Columns.Add("RAMlocked", Type.GetType("System.Boolean"));
            if (File.Exists(Application.StartupPath + "\\UserLib.xml"))
            {
                dt.ReadXml(Application.StartupPath + "\\UserLib.xml");
            }
            gridControl1.DataSource = dt;
            gridView1.BestFitColumns();
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable("UserLib");
            dt.Columns.Add("Filename");
            dt.Columns.Add("FilenameNoPath");
            dt.Columns.Add("EngineType");
            dt.Columns.Add("Stage");
            dt.Columns.Add("Injectors");
            dt.Columns.Add("Mapsensor");
            dt.Columns.Add("Torque");
            dt.Columns.Add("E85", Type.GetType("System.Boolean"));
            dt.Columns.Add("T7Valve", Type.GetType("System.Boolean"));
            dt.Columns.Add("Partnumber");
            dt.Columns.Add("SoftwareID");
            dt.Columns.Add("CPU");
            dt.Columns.Add("RAMlocked", Type.GetType("System.Boolean"));
            dt.WriteXml(Application.StartupPath + "\\UserLib.xml");
            gridControl1.DataSource = dt;
            gridView1.BestFitColumns();
        }
    }
}