using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Net;
using CommonSuite;
using NLog;

namespace T7
{
    public partial class frmBrowseTunes : DevExpress.XtraEditors.XtraForm
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

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
                    logger.Debug("Error setting proxy server: " + proxyE.Message);
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
                logger.Debug("An error occurred while retrieving the HTML content. " + ex.Message);
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

            File.Delete(Application.StartupPath + "\\T7Tunes.xml");
            try
            {
                URLString = "http://trionic.mobixs.eu/t7tunes/t7tunes.xml";
                XMLResult = GetPageHTML(URLString, 10);
                using (StreamWriter xmlfile = new StreamWriter(Application.StartupPath + "\\t7tunes.xml", false, System.Text.Encoding.ASCII, 2048))
                {
                    xmlfile.Write(XMLResult);
                    xmlfile.Close();
                }
                // document = downloaded
                DataTable dt = new DataTable("T7Tunes");
                dt.Columns.Add("FILENAME");
                dt.Columns.Add("ENGINETYPE");
                dt.Columns.Add("PARTNUMBER");
                dt.Columns.Add("SOFTWAREID");
                dt.Columns.Add("POWER", Type.GetType("System.Int32"));
                dt.Columns.Add("TORQUE", Type.GetType("System.Int32"));
                dt.ReadXml(Application.StartupPath + "\\t7tunes.xml");
                gridControl1.DataSource = dt;
                
            }
            catch (Exception tuE)
            {
                logger.Debug(tuE.Message);
            }
        }

        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshSelection();
        }

        private void RefreshSelection()
        {

/*            bool filteractive = false;
            string filterstring = string.Empty;
            // refresh based on current filtering
            gridView1.ActiveFilter.Clear();
  
            gridView1.ActiveFilterEnabled = filteractive;*/

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
                    break;
                }
            }
        }

        private void DownloadFile(string file)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Binary files|*.bin";
            sfd.FileName = file;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                frmProgress progress = new frmProgress();
                progress.SetProgress("Downloading file");
                progress.Show();
                Application.DoEvents();
                try
                {
                    WebClient client = new WebClient();
                    client.DownloadFile("http://trionic.mobixs.eu/t7tunes/" + file, sfd.FileName);
                    client.Dispose();
                }
                catch (Exception E)
                {
                    logger.Debug(E.Message);
                }
                progress.Close();
            }
           /* string command = "http://trionic.mobixs.eu/t7tunes/" + file;
            try
            {
                System.Diagnostics.Process.Start(command);
            }
            catch (Exception E)
            {
                //PumpString("Exception when checking new update(s): " + E.Message, false, false, new Version());
                logger.Debug(E.Message);
            }*/
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            WebClient client = new WebClient();
            client.Credentials = new System.Net.NetworkCredential("trionic", "trionic");
            client.UploadFile("ftp://localhost/test.bin", @"C:\t7test\inecu.bin");
            client.Dispose();
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
            {
                string filename = gridView1.GetRowCellValue(e.FocusedRowHandle, gcFilename).ToString();
                filename += ".jpg";
                /*            if (File.Exists(Application.StartupPath + "\\" + filename))
                            {
                                File.Delete(Application.StartupPath + "\\" + filename);
                            }
                            //DownloadFile(filename);
                            WebClient client = new WebClient();
                            client.DownloadFile("http://trionic.mobixs.eu/t7tunes/" + filename, Application.StartupPath + "\\" + filename);
                            client.Dispose();*/
                pictureBox1.Load("http://trionic.mobixs.eu/t7tunes/" + filename);
            }
            else
            {

                pictureBox1.Load("http://trionic.mobixs.eu/T7Suite.jpg");
            }
        }
    }
}