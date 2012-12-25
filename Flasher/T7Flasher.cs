using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using T7.KWP;
using System.Threading;

namespace T7Tool2.Flasher
{
    /// <summary>
    /// T7Flasher handles reading and writing of flash in Trionic 7 ECUs.
    /// 
    /// To use this class a KWPHandler must be set for the communication.
    /// </summary>
    public class T7Flasher
    {
        /// <summary>
        /// FlashCommand is a representation of the commands for this class.
        /// </summary>
        public enum FlashCommand
        {
            ReadCommand,
            ReadMemoryCommand,
            ReadSymbolMapCommand,
            ReadSymbolNameCommand,
            WriteCommand,
            StopCommand,
            NoCommand
        };


        /// <summary>
        /// FlashStatus is used for reporting the current status a flashing session.
        /// </summary>
        public enum FlashStatus
        {
            Reading,
            Writing,
            Eraseing,
            NoSequrityAccess,
            DoinNuthin,
            Completed,
            NoSuchFile,
            EraseError,
            WriteError,
            ReadError
        }

        /// <summary>
        /// This method returns the current status of this class.
        /// </summary>
        /// <returns>FlashStatus</returns>
        public FlashStatus getStatus() { return m_flashStatus; }

        /// <summary>
        /// This method returns the number of bytes that has been read or written so far.
        /// 0 is returned if there is no read or write session ongoing.
        /// </summary>
        /// <returns>Number of bytes that has been read or written.</returns>
        public int getNrOfBytesRead() { return m_nrOfBytesRead; }

        /// <summary>
        /// This method interrupts ongoing read or write session.
        /// </summary>
        public void stopFlasher()
        {
            lock (m_synchObject)
            {
                m_command = FlashCommand.StopCommand;
            }
        }
        public static void setKWPHandler(KWPHandler a_kwpHandler)
        {
            if (m_kwpHandler != null)
                throw new Exception("KWPHandler already set");
            m_kwpHandler = a_kwpHandler;
        }

        public static T7Flasher getInstance()
        {
            if (m_kwpHandler == null)
                throw new Exception("KWPHandler not set");
            if (m_instance == null)
                m_instance = new T7Flasher();
            return m_instance;
        }

        /// <summary>
        /// Constructor for T7Flasher.
        /// </summary>
        /// <param name="a_kwpHandler">The KWPHandler to be used for the communication.</param>
        public T7Flasher()
        {
            m_command = FlashCommand.NoCommand;
            m_flashStatus = FlashStatus.DoinNuthin;
            m_thread = new Thread(run);
            m_thread.Start();
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~T7Flasher()
        {
            lock (m_synchObject)
            {
                m_endThread = true;
            }
            m_resetEvent.Set();
        }

        /// <summary>
        /// This method starts a reading session.
        /// </summary>
        /// <param name="a_fileName">Name of the file where the flash contents is saved.</param>
        public void readFlash(string a_fileName)
        {
            lock (m_synchObject)
            {
                m_command = FlashCommand.ReadCommand;
                m_fileName = a_fileName;
            }
            m_resetEvent.Set();
        }

        /// <summary>
        /// This method starts a reading session for reading memory.
        /// </summary>
        /// <param name="a_fileName">Name of the file where the flash contents is saved.</param>
        /// <param name="a_offset">Starting address to read from.</param>
        /// <param name="a_length">Length to read.</param>
        public void readMemory(string a_fileName, UInt32 a_offset, UInt32 a_length)
        {
            lock (m_synchObject)
            {
                m_command = FlashCommand.ReadMemoryCommand;
                m_fileName = a_fileName;
                m_offset = a_offset;
                m_length = a_length;
            }
            m_resetEvent.Set();
        }

        /// <summary>
        /// This method starts symbol map.
        /// </summary>
        /// <param name="a_fileName">Name of the file where the flash contents is saved.</param>
        /// <param name="a_offset">Starting address to read from.</param>
        /// <param name="a_length">Length to read.</param>
        public void readSymbolMap(string a_fileName)
        {
            lock (m_synchObject)
            {
                m_command = FlashCommand.ReadSymbolMapCommand;
                m_fileName = a_fileName;
            }
            m_resetEvent.Set();
        }

        /// <summary>
        /// This method starts writing to flash.
        /// </summary>
        /// <param name="a_fileName">The name of the file from where to read the data from.</param>
        public void writeFlash(string a_fileName)
        {
            lock (m_synchObject)
            {
                m_command = FlashCommand.WriteCommand;
                m_fileName = a_fileName;
            }
            m_resetEvent.Set();
        }
        bool gotSequrityAccess = false;

        /// <summary>
        /// The run method handles writing and reading. It waits for a command to start read
        /// or write and handles this command until it's completed, stopped or until there is 
        /// a failure.
        /// </summary>
        void run()
        {
            while (true)
            {

                m_nrOfRetries = 0;
                m_nrOfBytesRead = 0;
                m_resetEvent.WaitOne(-1, true);
                gotSequrityAccess = false;
                lock (m_synchObject)
                {
                    if (m_endThread)
                        return;
                }
                m_kwpHandler.startSession();
                if (!gotSequrityAccess)
                {
                    for (int nrOfSequrityTries = 0; nrOfSequrityTries < 5; nrOfSequrityTries++)
                    {
                        if (!m_kwpHandler.requestSequrityAccess(true))
                            m_flashStatus = FlashStatus.NoSequrityAccess;
                        else
                        {
                            gotSequrityAccess = true;
                            break;
                        }
                    }
                }
                //Here it would make sense to stop if we didn't ge security acces but
                //let's try anyway. It could be that we don't get a possitive reply from the 
                //ECU if we alredy have security access (from a previous, interrupted, session).
                if (m_command == FlashCommand.ReadCommand)
                {
                    int nrOfBytes = 512; // TODO: Maybe increase value for faster reading?
                    byte[] data;

                    if (File.Exists(m_fileName))
                        File.Delete(m_fileName);
                    FileStream fileStream = File.Create(m_fileName, 1024);

                    for (int i = 0; i < 512 * 1024 / nrOfBytes; i++)
                    {
                        lock (m_synchObject)
                        {
                            if (m_command == FlashCommand.StopCommand)
                                continue;
                            if (m_endThread)
                                return;
                        }
                        m_flashStatus = FlashStatus.Reading;
                        while (!m_kwpHandler.sendReadRequest((uint)(nrOfBytes * i), (uint)nrOfBytes))
                        {
                            m_nrOfRetries++;
                        }

                        while (!m_kwpHandler.sendRequestDataByOffset(out data))
                        {
                            m_nrOfRetries++;
                        }
                        fileStream.Write(data, 0, nrOfBytes);
                        m_nrOfBytesRead += nrOfBytes;
                    }
                    fileStream.Close();
                    m_kwpHandler.sendDataTransferExitRequest();
                }
                else if (m_command == FlashCommand.ReadMemoryCommand)
                {
                    int nrOfBytes = 512;//64;
                    byte[] data;

                    if (File.Exists(m_fileName))
                        File.Delete(m_fileName);
                    FileStream fileStream = File.Create(m_fileName, 1024);
                    int nrOfReads = (int)m_length / nrOfBytes;
                    for (int i = 0; i < nrOfReads; i++)
                    {
                        lock (m_synchObject)
                        {
                            if (m_command == FlashCommand.StopCommand)
                                continue;
                            if (m_endThread)
                                return;
                        }
                        m_flashStatus = FlashStatus.Reading;
                        if (i == nrOfReads - 1)
                            nrOfBytes = (int)m_length - nrOfBytes * i;
                        while (!m_kwpHandler.sendReadRequest((uint)m_offset + (uint)(nrOfBytes * i), (uint)nrOfBytes))
                        {
                            m_nrOfRetries++;
                        }

                        while (!m_kwpHandler.sendRequestDataByOffset(out data))
                        {
                            m_nrOfRetries++;
                        }
                        fileStream.Write(data, 0, nrOfBytes);
                        m_nrOfBytesRead += nrOfBytes;
                    }
                    fileStream.Close();
                    m_kwpHandler.sendDataTransferExitRequest();
                }
                else if (m_command == FlashCommand.ReadSymbolMapCommand)
                {
                    byte[] data;
                    string swVersion = "";
                    m_nrOfBytesRead = 0;
                    if (File.Exists(m_fileName))
                        File.Delete(m_fileName);
                    FileStream fileStream = File.Create(m_fileName, 1024);
                    if (m_kwpHandler.sendUnknownRequest() != KWPResult.OK)
                    {
                        m_flashStatus = FlashStatus.ReadError;
                        continue;
                    }
                    m_flashStatus = FlashStatus.Reading;
                    m_kwpHandler.getSwVersionFromDR51(out swVersion);

                    if (m_kwpHandler.sendReadSymbolMapRequest() != KWPResult.OK)
                    {
                        m_flashStatus = FlashStatus.ReadError;
                        continue;
                    }
                    m_kwpHandler.sendDataTransferRequest(out data);
                    while (data.Length > 0x10)
                    {
                        fileStream.Write(data, 1, data.Length - 3);
                        m_nrOfBytesRead += data.Length - 3;
                        lock (m_synchObject)
                        {
                            if (m_command == FlashCommand.StopCommand)
                                continue;
                            if (m_endThread)
                                return;
                        }
                        m_kwpHandler.sendDataTransferRequest(out data);
                    }
                    fileStream.Flush();
                    fileStream.Close();
                }
                else if (m_command == FlashCommand.WriteCommand)
                {
                    AddToTrace("Flash command");
                    int nrOfBytes = 128;
                    int i = 0;
                    byte[] data = new byte[nrOfBytes];
                    if (!File.Exists(m_fileName))
                    {
                        m_flashStatus = FlashStatus.NoSuchFile;
                        continue;
                    }
                    m_flashStatus = FlashStatus.Eraseing;
                    AddToTrace("Start erasing");

                    if (m_kwpHandler.sendEraseRequest() != KWPResult.OK)
                    {
                        m_flashStatus = FlashStatus.EraseError;
                        // break;
                    }
                    AddToTrace("Erase done");

                    FileStream fs = new FileStream(m_fileName, FileMode.Open, FileAccess.Read);

                    m_flashStatus = FlashStatus.Writing;
                    //Write 0x0-0x7B000
                    AddToTrace("Start writing 0x00000-0x7B000");

                    if (m_kwpHandler.sendWriteRequest(0x0, 0x7B000) != KWPResult.OK)
                    {
                        m_flashStatus = FlashStatus.WriteError;
                        AddToTrace("Write error in m_kwpHandler.sendWriteRequest(0x0, 0x7B000)");
                        continue;
                    }
                    AddToTrace("Start sending data with sendWriteDataRequest");

                    for (i = 0; i < 0x7B000 / nrOfBytes; i++)
                    {
                        fs.Read(data, 0, nrOfBytes);
                        m_nrOfBytesRead = i * nrOfBytes;
                        AddToTrace("sending data " + m_nrOfBytesRead.ToString());

                        if (m_kwpHandler.sendWriteDataRequest(data) != KWPResult.OK)
                        {
                            AddToTrace("write error on " + m_nrOfBytesRead.ToString() + " bytes");

                            m_flashStatus = FlashStatus.WriteError;
                            continue;
                        }
                        lock (m_synchObject)
                        {
                            if (m_command == FlashCommand.StopCommand)
                                continue;
                            if (m_endThread)
                                return;
                        }
                    }
                    AddToTrace("Start writing 0x7FE00-0x7FFFF");

                    //Write 0x7FE00-0x7FFFF
                    if (m_kwpHandler.sendWriteRequest(0x7FE00, 0x200) != KWPResult.OK)
                    {
                        AddToTrace("Write error in m_kwpHandler.sendWriteRequest(0x7FE00, 0x200)");

                        m_flashStatus = FlashStatus.WriteError;
                        continue;
                    }
                    AddToTrace("Start sending data with sendWriteDataRequest");
                    fs.Seek(0x7FE00, System.IO.SeekOrigin.Begin);
                    for (i = 0x7FE00 / nrOfBytes; i < 0x80000 / nrOfBytes; i++)
                    {
                        fs.Read(data, 0, nrOfBytes);
                        m_nrOfBytesRead = i * nrOfBytes;
                        AddToTrace("sending data " + m_nrOfBytesRead.ToString());
                        if (m_kwpHandler.sendWriteDataRequest(data) != KWPResult.OK)
                        {
                            AddToTrace("write error on " + m_nrOfBytesRead.ToString() + " bytes");
                            m_flashStatus = FlashStatus.WriteError;
                            continue;
                        }
                        lock (m_synchObject)
                        {
                            if (m_command == FlashCommand.StopCommand)
                                continue;
                            if (m_endThread)
                                return;
                        }
                    }
                }
                AddToTrace("Flash completed");
                m_flashStatus = FlashStatus.Completed;
            }
        }

        private void AddToTrace(string item)
        {
            using (StreamWriter sw = new StreamWriter(Path.GetDirectoryName(m_fileName) + "\\ + flashtrace.txt", true))
            {
                sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss:fff") + " " + gotSequrityAccess.ToString() + " " + item);
            }
        }


        private Thread m_thread;
        private AutoResetEvent m_resetEvent = new AutoResetEvent(false);
        private FlashCommand m_command;
        private string m_fileName;
        private Object m_synchObject = new Object();
        private static KWPHandler m_kwpHandler;
        private int m_nrOfRetries;
        private FlashStatus m_flashStatus;
        private int m_nrOfBytesRead;
        private bool m_endThread = false;
        private UInt32 m_offset;
        private UInt32 m_length;
        private static T7Flasher m_instance;
    }
}

