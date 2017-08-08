using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Xml;
using System.IO;

namespace T5Suite2
{
    public partial class frmBrowseTunes : DevExpress.XtraEditors.XtraForm
    {
        public frmBrowseTunes()
        {
            InitializeComponent();
            FetchFiles();
        }

        private void labelControl6_Click(object sender, EventArgs e)
        {

        }

        public string GetPageHTML(string pageUrl, int timeoutSeconds)
        {
            System.Net.WebResponse response = null;

            try
            {
                // Setup our Web request
                System.Net.WebRequest request = System.Net.WebRequest.Create(pageUrl);

                try
                {
                    //request.Proxy = System.Net.WebProxy.GetDefaultProxy();
                    request.Proxy.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                }
                catch (Exception proxyE)
                {
                    Console.WriteLine("Error setting proxy server: " + proxyE.Message);
                }

                /*                if (UseDefaultProxy)
                                {
                                    request.Proxy = System.Net.WebProxy.GetDefaultProxy();
                                    if (UseDefaultCredentials)
                                    {
                                        request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                                    }
                                    if (UseDefaultNetworkCredentials)
                                    {
                                        request.Proxy.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                                    }
                                }*/

                request.Timeout = timeoutSeconds * 1000;

                // Retrieve data from request
                response = request.GetResponse();

                System.IO.Stream streamReceive = response.GetResponseStream();
                System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("utf-8");
                System.IO.StreamReader streamRead = new System.IO.StreamReader(streamReceive, encoding);

                // return the retrieved HTML
                return streamRead.ReadToEnd();
            }
            catch (Exception ex)
            {
                // Error occured grabbing data, return empty string.
                Console.WriteLine("An error occurred while retrieving the HTML content. " + ex.Message);
                /*using (StreamWriter logfile = new StreamWriter("update.log", true, System.Text.Encoding.ASCII, 2048))
                {
                    logfile.WriteLine("An error occurred while retrieving the HTML content. " + ex.Message);
                    logfile.Close();
                }*/

                return "";
            }
            finally
            {
                // Check if exists, then close the response.
                if (response != null)
                {
                    response.Close();
                }
            }
        }

        private void FetchFiles()
        {
            // load XML file
            string URLString = "";
            string XMLResult = "";

            File.Delete(Application.StartupPath + "\\t5tunes.xml");
            try
            {
                URLString = "http://trionic.mobixs.eu/t5tunes/t5tunes.xml";
                XMLResult = GetPageHTML(URLString, 10);
                using (StreamWriter xmlfile = new StreamWriter(Application.StartupPath + "\\t5tunes.xml", false, System.Text.Encoding.ASCII, 2048))
                {
                    xmlfile.Write(XMLResult);
                    xmlfile.Close();
                }
                // document = downloaded
                DataTable dt = new DataTable("T5TUNES");
                dt.Columns.Add("Filename");
                dt.Columns.Add("Carmodel");
                dt.Columns.Add("Engine");
                dt.Columns.Add("OBD");
                dt.Columns.Add("Mapsensor");
                dt.Columns.Add("Injectors");
                dt.Columns.Add("stage");
                dt.ReadXml(Application.StartupPath + "\\t5tunes.xml");
                gridControl1.DataSource = dt;
                
            }
            catch (Exception tuE)
            {
                Console.WriteLine(tuE.Message);
            }
        }

        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshSelection();
        }

        private void RefreshSelection()
        {

            bool filteractive = false;
            string filterstring = string.Empty;
            // refresh based on current filtering
            gridView1.ActiveFilter.Clear();
            if (comboBoxEdit1.SelectedIndex > 0)
            {
                // add to filter
                filteractive = true;
                filterstring = @"([Carmodel] = '" + comboBoxEdit1.Text + "')";
                DevExpress.XtraGrid.Columns.ColumnFilterInfo fltr = new DevExpress.XtraGrid.Columns.ColumnFilterInfo(filterstring, "Car model");
                gridView1.ActiveFilter.Add(gcCarmodel, fltr);
            }
            if (comboBoxEdit2.SelectedIndex > 0)
            {
                // add to filter
                filteractive = true;
                filterstring = @"([Engine] = '" + comboBoxEdit2.Text + "')";
                DevExpress.XtraGrid.Columns.ColumnFilterInfo fltr = new DevExpress.XtraGrid.Columns.ColumnFilterInfo(filterstring, "Engine");
                gridView1.ActiveFilter.Add(gcEngine, fltr);
            }
            if (comboBoxEdit3.SelectedIndex > 0)
            {
                // add to filter
                filteractive = true;
                filterstring = @"([OBD] = '" + comboBoxEdit3.Text + "')";
                DevExpress.XtraGrid.Columns.ColumnFilterInfo fltr = new DevExpress.XtraGrid.Columns.ColumnFilterInfo(filterstring, "OBD type");
                gridView1.ActiveFilter.Add(gcOBD, fltr);
            }
            if (comboBoxEdit4.SelectedIndex > 0)
            {
                // add to filter
                filteractive = true;
                filterstring = @"([Mapsensor] = '" + comboBoxEdit4.Text + "')";
                DevExpress.XtraGrid.Columns.ColumnFilterInfo fltr = new DevExpress.XtraGrid.Columns.ColumnFilterInfo(filterstring, "Mapsensor");
                gridView1.ActiveFilter.Add(gcMapsensor, fltr);
            }
            if (comboBoxEdit5.SelectedIndex > 0)
            {
                // add to filter
                filteractive = true;
                filterstring = @"([Injectors] = '" + comboBoxEdit5.Text + "')";
                DevExpress.XtraGrid.Columns.ColumnFilterInfo fltr = new DevExpress.XtraGrid.Columns.ColumnFilterInfo(filterstring, "Injectors");
                gridView1.ActiveFilter.Add(gcInjectors, fltr);
            }
            if (comboBoxEdit6.SelectedIndex > 0)
            {
                // add to filter
                filteractive = true;
                filterstring = @"([stage] = '" + comboBoxEdit6.Text + "')";
                DevExpress.XtraGrid.Columns.ColumnFilterInfo fltr = new DevExpress.XtraGrid.Columns.ColumnFilterInfo(filterstring, "Stage");
                gridView1.ActiveFilter.Add(gcStage, fltr);
            }
            /*** set filter ***/
            gridView1.ActiveFilterEnabled = filteractive;

        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            string filename = string.Empty;
            //filename = 
            int[] rows = gridView1.GetSelectedRows();
            if (rows.Length > 0)
            {
                foreach (int rowindex in rows)
                {
                    filename = gridView1.GetRowCellValue(rowindex, gcFilename).ToString();
                    DownloadFile(filename);
                }
            }
        }

        private void DownloadFile(string file)
        {
            string command = "http://trionic.mobixs.eu/t5tunes/" + file;
            try
            {
                System.Diagnostics.Process.Start(command);
            }
            catch (Exception E)
            {
                //PumpString("Exception when checking new update(s): " + E.Message, false, false, new Version());
                Console.WriteLine(E.Message);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
            {
                string filename = gridView1.GetRowCellValue(e.FocusedRowHandle, gcFilename).ToString();
                filename += ".PNG";
                pictureBox1.Load("http://trionic.mobixs.eu/t5tunes/" + filename);
                string filenamehtm = gridView1.GetRowCellValue(e.FocusedRowHandle, gcFilename).ToString();
                filenamehtm += ".htm";
                webBrowser1.Navigate("http://trionic.mobixs.eu/t5tunes/" + filenamehtm);
            }
            else
            {

                pictureBox1.Load("http://trionic.mobixs.eu/T5Suite.jpg");
            }
        }
    }
}