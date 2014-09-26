using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace T8SuitePro
{
    public partial class frmUpdateAvailable : DevExpress.XtraEditors.XtraForm
    {

        /// <summary>
        /// Required designer variable.
        /// </summary>
        
        public frmUpdateAvailable()
        {
            // Required for Windows Form Designer support
            InitializeComponent();
            // TODO: Add any constructor code after InitializeComponent call
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public void SetVersionNumber(string version)
        {
            labelControl2.Text = "Available version: " + version;
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

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("IEXPLORE.EXE", "http://develop.trionictuning.com/T8Suite/Notes.xml" );
        }

        private void frmUpdateAvailable_Load(object sender, EventArgs e)
        {

        }
    }
}