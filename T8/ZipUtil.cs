using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace CommonSuite
{
    class ZipUtil
    {
        static public void unzip(string filename)
        {
            // Already unpacked?
            if (!File.Exists(filename))
            {
                return;
            }

            using (ZipInputStream s = new ZipInputStream(File.OpenRead(filename)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string fileName = Path.GetFileName(theEntry.Name);

                    if (fileName != String.Empty)
                    {
                        using (FileStream streamWriter = File.Create(theEntry.Name))
                        {
                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            // After unpack rename the zip file.
            File.Move(filename, filename + "unpacked");
        }
    }
}
