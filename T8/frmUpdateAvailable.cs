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
        

        public void ShowRSSFeeds()
        {
            try
            {
                RSS2HTMLScoutLib.RSS2HTMLScout RSS2HTML = new RSS2HTMLScoutLib.RSS2HTMLScout();
                //RSS2HTML.ForceRefresh = true;
                RSS2HTML.ItemsPerFeed = 10; // limit 5 latest items per feed
                RSS2HTML.MainHeader = "<html><head><title>T8Suite changelog</title><!-- CSS source code will be inserted here -->{CSS}<!-- HTML page encoding. please change if needed --><!-- <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\"> --></head><body>";
                //RSS2HTML.ChannelHeader = "<div class=\"ChannelHeader\"><table width=\"100%\" border=\"0\"><tr><td valign=\"middle\" align=\"left\"><a href=\"{LINK}\">{IMAGE}</a></td>      <td width=\"99%\" vAlign=middle align=middle><br><h3>{TITLE}</h3></td></tr></table></div>";
                RSS2HTML.ChannelHeader = "<div class=\"ChannelHeader\"><table width=\"100%\" border=\"0\"><tr><td valign=\"middle\" align=\"left\"><a href=\"{LINK}\"> {IMAGE}</a></td>      <td width=\"99%\" vAlign=middle align=middle><br><h2>{TITLE}</h2></td></tr></table>{DESCRIPTION}</div>";
                RSS2HTML.EnclosureTemplate = "<a href=\"{LINK}\">Image: {TITLE} ({LENGTH})</a>";
                RSS2HTML.ErrorMessageTemplate = "<p>Following feeds can not be displayed:<br>{FAILEDFEEDS}<br></p>";
                RSS2HTML.ItemTemplate = "<div class=\"ItemHeader\"><a href=\"{LINK}\">{TITLE}</a></div><div class=\"ItemDescription\">{DESCRIPTION}</div><div class=\"ItemFooter\">{AUTHOR} {DATE} {TIME} <a href=\"{COMMENTS}\">{COMMENTS} {ENCLOSURE}</a></div>";
                RSS2HTML.NewItemTemplate = "<div style=\"font-style: italic; background-color: #ead2d9\" class=\"NewItemHeader\"><a href=\"{LINK}\">{TITLE}</a></div><div class=\"NewItemDescription\">{DESCRIPTION}</div><div class=\"NewItemFooter\">{AUTHOR} {DATE} {TIME} <a href=\"{COMMENTS}\">{COMMENTS} {ENCLOSURE}</a></div>";
                RSS2HTML.MainFooter = "</body></html>";
                RSS2HTML.AddFeed("http://trionic.mobixs.eu/t8suite/Notes.xml", 180); // ' update every 180 minutes (3 hours)
                RSS2HTML.Execute();
                RSS2HTML.SaveOutputToFile(Application.UserAppDataPath + "\\T8Suite.html");
            }
            catch (Exception E)
            {
                Console.WriteLine("Error getting RSS feeds: " + E.Message);
            }

        }

        private void frmUpdateAvailable_Load(object sender, EventArgs e)
        {

        }
    }
}