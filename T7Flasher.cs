using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using T7.KWP;
using System.Threading;

namespace T7.Flasher
{
    //-----------------------------------------------------------------------------
    /**
        Trionic 7 CAN flasher base class.
    */
    abstract class IFlasher
    {
        public delegate void StatusChanged(object sender, StatusEventArgs e);
        abstract public event StatusChanged onStatusChanged;

        public class StatusEventArgs : System.EventArgs
        {
            private string _info;

            public string Info
            {
                get { return _info; }
                set { _info = value; }
            }

            public StatusEventArgs(string info)
            {
                this._info = info;
            }
        }


        // flasher commands
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

        // status of current flashing session    
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
        };

        // dynamic state
        protected FlashStatus m_flashStatus;            ///< current status
        protected FlashCommand m_command;               ///< command
        protected Object m_synchObject = new Object();  ///< state lock

        // configuration
        private bool m_EnableCanLog = false;            ///< CAN logging flag

        //-------------------------------------------------------------------------
        /**
            Default constructor.
        */
        public IFlasher()
        {
            // clear state
            this.m_flashStatus = FlashStatus.DoinNuthin;
            this.m_command = FlashCommand.NoCommand;
        }

        //-------------------------------------------------------------------------
        /**
            Sets CAN logging on/off.
        */
        public bool EnableCanLog
        {
            get { return m_EnableCanLog; }
            set { m_EnableCanLog = value; }
        }

        //-------------------------------------------------------------------------
        /**
            Adds a line to CAN log file.
     
            @param      line        line to add
        */
        protected void AddToCanTrace(string line)
        {
            Console.WriteLine(line);
            if (!this.m_EnableCanLog)
            {
                return;
            }

            DateTime dtnow = DateTime.Now;
            using (StreamWriter sw =
                new StreamWriter(System.Windows.Forms.Application.StartupPath +
                "\\CanTraceT7Flasher.txt", true))
            {
                sw.WriteLine(dtnow.ToString("dd/MM/yyyy HH:mm:ss") + " - " + line);
            }
        }

        //-------------------------------------------------------------------------
        /**
            Returns the current status of flasher.
     
            @return                 status 
        */
        public virtual FlashStatus getStatus()
        {
            return this.m_flashStatus;
        }

        public abstract int getNrOfBytesRead();

        //-------------------------------------------------------------------------
        /**
            Interrupts ongoing read or write session.
        */
        public virtual void stopFlasher()
        {
            lock (this.m_synchObject)
            {
                this.m_command = FlashCommand.StopCommand;
            }
        }

        //-------------------------------------------------------------------------
        /**
            Starts a flash reading session.
        
            @param      a_fileName      name of target file
        */
        public virtual void readFlash(string a_fileName)
        {
            lock (this.m_synchObject)
            {
                this.m_command = FlashCommand.ReadCommand;
            }
        }

        //-------------------------------------------------------------------------
        /**
            Starts a SRAM reading session.
        
            @param      a_fileName      name of target file
            @param      a_offset        source offset
            @param      a_length        source length, bytes
        */
        public virtual void readMemory(string a_fileName, UInt32 a_offset,
            UInt32 a_length)
        {
            lock (this.m_synchObject)
            {
                m_command = FlashCommand.ReadMemoryCommand;
            }
        }

        //-------------------------------------------------------------------------
        /**
            Starts a symbol map reading session.
        
            @param      a_fileName      name of target file
        */
        public virtual void readSymbolMap(string a_fileName)
        {
            lock (m_synchObject)
            {
                m_command = FlashCommand.ReadSymbolMapCommand;
            }
        }

        //-------------------------------------------------------------------------
        /**
            Starts friting to flash memory.
        
            @param      a_fileName      name of source file
        */
        public virtual void writeFlash(string a_fileName)
        {
            lock (m_synchObject)
            {
                m_command = FlashCommand.WriteCommand;
            }
        }
    };

    /// <summary>
    /// T7Flasher handles reading and writing of flash in Trionic 7 ECUs.
    /// 
    /// To use this class a KWPHandler must be set for the communication.
    /// </summary>
    class T7Flasher : IFlasher
    {

        public override event IFlasher.StatusChanged onStatusChanged;

        /// <summary>
        /// This method returns the number of bytes that has been read or written so far.
        /// 0 is returned if there is no read or write session ongoing.
        /// </summary>
        /// <returns>Number of bytes that has been read or written.</returns>
        public override int getNrOfBytesRead() { return m_nrOfBytesRead; }

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
        public override void readFlash(string a_fileName)
        {
            base.readFlash(a_fileName);
            lock (m_synchObject)
            {
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
        public override void readMemory(string a_fileName, UInt32 a_offset, UInt32 a_length)
        {
            base.readMemory(a_fileName, a_offset, a_length);
            lock (m_synchObject)
            {
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
        public override void readSymbolMap(string a_fileName)
        {
            base.readSymbolMap(a_fileName);
            lock (m_synchObject)
            {
                m_fileName = a_fileName;
            }
            m_resetEvent.Set();
        }

        /// <summary>
        /// This method starts writing to flash.
        /// </summary>
        /// <param name="a_fileName">The name of the file from where to read the data from.</param>
        public override void writeFlash(string a_fileName)
        {
            base.writeFlash(a_fileName);
            lock (m_synchObject)
            {
                m_fileName = a_fileName;
            }
            m_resetEvent.Set();
        }

        /// <summary>
        /// The run method handles writing and reading. It waits for a command to start read
        /// or write and handles this command until it's completed, stopped or until there is 
        /// a failure.
        /// </summary>
        void run()
        {
            bool gotSequrityAccess = false;
            while (true)
            {
                AddToCanTrace("Running T7Flasher");
                m_nrOfRetries = 0;
                m_nrOfBytesRead = 0;
                m_resetEvent.WaitOne(-1, true);
                gotSequrityAccess = false;
                lock (m_synchObject)
                {
                    if (m_endThread)
                    {
                        AddToCanTrace("Thread ended");
                        return;
                    }
                }


                m_kwpHandler.startSession();
                AddToCanTrace("Session started");

                if (!gotSequrityAccess)
                {
                    AddToCanTrace("No security access");

                    for (int nrOfSequrityTries = 0; nrOfSequrityTries < 5; nrOfSequrityTries++)
                    {
                        if (!m_kwpHandler.requestSequrityAccess(true))
                        {
                            AddToCanTrace("No security access granted");
                            m_flashStatus = FlashStatus.NoSequrityAccess;
                        }
                        else
                        {
                            gotSequrityAccess = true;
                            AddToCanTrace("Security access granted");

                            break;
                        }
                    }
                }
                //Here it would make sense to stop if we didn't ge security acces but
                //let's try anyway. It could be that we don't get a possitive reply from the 
                //ECU if we alredy have security access (from a previous, interrupted, session).
                if (m_command == FlashCommand.ReadCommand)
                {
                    int nrOfBytes = 64;//512;
                    byte[] data;
                    AddToCanTrace("Reading flash content to file: " + m_fileName);


                    if (File.Exists(m_fileName))
                        File.Delete(m_fileName);
                    FileStream fileStream = File.Create(m_fileName, 1024);
                    AddToCanTrace("File created");

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
                        AddToCanTrace("Flash status is reading");

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
                    AddToCanTrace("Closed file");

                    m_kwpHandler.sendDataTransferExitRequest();
                    AddToCanTrace("Done reading");

                }
                else if (m_command == FlashCommand.ReadMemoryCommand)
                {
                    int nrOfBytes = /*512*/64;
                    byte[] data;
                    Console.WriteLine("Reading: " + m_length.ToString() + " bytes");
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
                        Console.WriteLine("Writing data to file: " + m_length.ToString() + " bytes");
                        fileStream.Write(data, 0, nrOfBytes);
                        m_nrOfBytesRead += nrOfBytes;
                    }
                    fileStream.Close();
                    Console.WriteLine("Done reading");
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
                    int nrOfBytes = 128;
                    int i = 0;
                    byte[] data = new byte[nrOfBytes];

                    if (!File.Exists(m_fileName))
                    {
                        m_flashStatus = FlashStatus.NoSuchFile;
                        AddToCanTrace("No such file found: " + m_fileName);
                        continue;
                    }
                    m_flashStatus = FlashStatus.Eraseing;
                    if (m_kwpHandler.sendEraseRequest() != KWPResult.OK)
                    {
                        m_flashStatus = FlashStatus.EraseError;
                        AddToCanTrace("Erase error occured");
                        // break;
                    }
                    AddToCanTrace("Opening file for reading");

                    FileStream fs = new FileStream(m_fileName, FileMode.Open, FileAccess.Read);

                    m_flashStatus = FlashStatus.Writing;
                    AddToCanTrace("Set flash status to writing");

                    //Write 0x0-0x7B000
                    AddToCanTrace("0x0-0x7B000");
                    if (m_kwpHandler.sendWriteRequest(0x0, 0x7B000) != KWPResult.OK)
                    {
                        m_flashStatus = FlashStatus.WriteError;
                        AddToCanTrace("Write error occured");

                        continue;
                    }
                    for (i = 0; i < 0x7B000 / nrOfBytes; i++)
                    {
                        fs.Read(data, 0, nrOfBytes);
                        m_nrOfBytesRead = i * nrOfBytes;
                        if (m_kwpHandler.sendWriteDataRequest(data) != KWPResult.OK)
                        {
                            m_flashStatus = FlashStatus.WriteError;
                            AddToCanTrace("Write error occured");

                            continue;
                        }
                        lock (m_synchObject)
                        {
                            if (m_command == FlashCommand.StopCommand)
                            {
                                AddToCanTrace("Stop command seen");
                                continue;
                            }
                            if (m_endThread)
                            {
                                AddToCanTrace("Thread ended");
                                return;
                            }
                        }
                    }

                    //Write 0x7FE00-0x7FFFF
                    AddToCanTrace("Write 0x7FE00-0x7FFFF");

                    if (m_kwpHandler.sendWriteRequest(0x7FE00, 0x200) != KWPResult.OK)
                    {
                        m_flashStatus = FlashStatus.WriteError;
                        AddToCanTrace("Write error occured");
                        continue;
                    }
                    fs.Seek(0x7FE00, System.IO.SeekOrigin.Begin);
                    for (i = 0x7FE00 / nrOfBytes; i < 0x80000 / nrOfBytes; i++)
                    {
                        fs.Read(data, 0, nrOfBytes);
                        m_nrOfBytesRead = i * nrOfBytes;
                        if (m_kwpHandler.sendWriteDataRequest(data) != KWPResult.OK)
                        {
                            m_flashStatus = FlashStatus.WriteError;
                            AddToCanTrace("Write error occured");
                            continue;
                        }
                        lock (m_synchObject)
                        {
                            if (m_command == FlashCommand.StopCommand)
                            {
                                AddToCanTrace("Stop command seen");
                                continue;
                            }
                            if (m_endThread)
                            {
                                AddToCanTrace("Thread ended");
                                return;
                            }
                        }
                    }
                }
                AddToCanTrace("T7Flasher completed");
                m_flashStatus = FlashStatus.Completed;
            }
        }


        private Thread m_thread;
        private AutoResetEvent m_resetEvent = new AutoResetEvent(false);
        private string m_fileName;
        private static KWPHandler m_kwpHandler;
        private int m_nrOfRetries;
        private int m_nrOfBytesRead;
        private bool m_endThread = false;
        private UInt32 m_offset;
        private UInt32 m_length;
        private static T7Flasher m_instance;
    }
}
