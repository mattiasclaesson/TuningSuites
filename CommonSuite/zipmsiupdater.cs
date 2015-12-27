using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using NLog;
using ICSharpCode.SharpZipLib.Zip;
using System.Net;
using System.ComponentModel;


namespace CommonSuite
{
    public class zipmsiupdater : msiupdater
    {
        private string m_server = "";
        private Logger logger = LogManager.GetCurrentClassLogger();

        private string m_msi;
        private string m_zip;
        private string tempPath = System.IO.Path.GetTempPath();
        private string zipFilePath;
        private string msiFilePath;

        public void ExecuteUpdate(string server, Version ver, string msi, string zip)
        {
            m_msi = msi;
            m_zip = zip;
            zipFilePath = tempPath + "/" + m_zip;
            msiFilePath = tempPath + "/" + m_msi;
            String address = m_server + ver.ToString() + "/" + m_zip;

            File.Delete(zipFilePath);
            File.Delete(msiFilePath);

            WebClient webClient = new WebClient();
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCallback);
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);
            webClient.DownloadFileAsync(new Uri(address), zipFilePath);
        }

        private void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        {
            // Displays the operation identifier, and the transfer progress.
            Console.WriteLine("{0}    downloaded {1} of {2} bytes. {3} % complete...",
                (string)e.UserState,
                e.BytesReceived,
                e.TotalBytesToReceive,
                e.ProgressPercentage);
        }

        private void DownloadFileCallback(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw e.Error;
            }

            if (!e.Cancelled)
            {
                if (!Decompress())
                {
                    PumpString("Could not decompress file: " + zipFilePath, false, false, new Version(), "");
                }

                try
                {
                    System.Diagnostics.Process.Start(msiFilePath);
                }
                catch (Exception E)
                {
                    PumpString("Exception when installing new update: " + E.Message, false, false, new Version(), "");
                }
            }
        }

        public bool Decompress()
        {
            Boolean gotIt = false;
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    if (theEntry.IsFile && theEntry.Name == m_msi)
                    {
                        gotIt = true;
                        FileStream streamWriter = File.Create(msiFilePath);
                        long filesize = theEntry.Size;
                        byte[] data = new byte[filesize];
                        while (true)
                        {
                            filesize = s.Read(data, 0, data.Length);
                            if (filesize > 0)
                            {
                                streamWriter.Write(data, 0, (int)filesize);
                            }
                            else
                            {
                                break;
                            }
                        }
                        streamWriter.Close();
                    }
                }
            }

            return gotIt;
        }
    }
}
