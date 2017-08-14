using System;
using System.Collections.Generic;
using System.Threading;
using canlibCLSNET;
using NLog;

namespace T5CANLib.CAN
{
    /// <summary>
    /// All incomming messages are published to registered ICANListeners.
    /// </summary>
    /// 
    public class KvaserCANDevice : ICANDevice
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        int handleWrite = -1;
        int handleRead = -1;

        Canlib.canStatus writeStatus;

        Thread m_readThread;
        readonly Object m_synchObject = new Object();
        bool m_endThread;

        public int ChannelNumber { get; set; }

        public override void Delete()
        {
            // empty
        }

        
        override public void EnableLogging(string path2log)
        {
        }

        override public void DisableLogging()
        {
        }

        override public int GetNumberOfAdapters()
        {
            return 1;
        }


        public override void setPortNumber(string portnumber)
        {
            //nothing to do
        }
        override public void clearTransmitBuffer()
        {
            // implement clearTransmitBuffer
        }

        override public void clearReceiveBuffer()
        {
            // implement clearReceiveBuffer
        }

        // not supported by kvaser
        public override float GetADCValue(uint channel)
        {
            return 0F;
        }

        // not supported by kvaser
        public override float GetThermoValue()
        {
            return 0F;
        }

        public static string[] GetAdapterNames()
        {
            Canlib.canInitializeLibrary();

            //List available channels
            int nrOfChannels;
            Canlib.canGetNumberOfChannels(out nrOfChannels);
            string[] names = new string[nrOfChannels];
            object o = new object();
            for (int i = 0; i < nrOfChannels; i++)
            {
                Canlib.canGetChannelData(i, Canlib.canCHANNELDATA_CHANNEL_NAME, out o);
                names[i] = o.ToString();
                logger.Debug(string.Format("canlibCLSNET.Canlib.canGetChannelData({0}, canlibCLSNET.Canlib.canCHANNELDATA_CHANNEL_NAME, {1})", i, o));
            }
            return names;
        }

        public void SetSelectedAdapter(string adapter)
        {
            int nrOfChannels;
            Canlib.canGetNumberOfChannels(out nrOfChannels);
            object o = new object();
            for (int i = 0; i < nrOfChannels; i++)
            {
                Canlib.canGetChannelData(i, Canlib.canCHANNELDATA_CHANNEL_NAME, out o);
                if(adapter.Equals(o.ToString()))
                {
                    ChannelNumber = i;
                    logger.Debug(string.Format("canlibCLSNET.Canlib.canGetChannelData({0}, canlibCLSNET.Canlib.canCHANNELDATA_CHANNEL_NAME, {1})", i, o));
                    return;
                }
            }

            // Default to channel 0
            ChannelNumber = 0;
        }

        /// <summary>
        /// readMessages is the "run" method of this class. It reads all incomming messages
        /// and publishes them to registered ICANListeners.
        /// </summary>
        public void readMessages()
        {
            byte[] msg = new byte[8];
            int dlc;
            int flag, id;
            long time;
            Canlib.canStatus status;
            CANMessage canMessage = new CANMessage();
            logger.Debug("readMessages started");
            while (true)
            {
                lock (m_synchObject)
                {
                    if (m_endThread)
                    {
                        logger.Debug("readMessages thread ended");
                        return;
                    }
                }
                status = Canlib.canReadWait(handleRead, out id, msg, out dlc, out flag, out time, 250);
                if ((flag & Canlib.canMSG_ERROR_FRAME) == 0)
                {
                    if (status == Canlib.canStatus.canOK)
                    {
                        canMessage.setID((uint)id);
                        canMessage.setTimeStamp((uint)time);
                        canMessage.setFlags((byte)flag);
                        canMessage.setCanData(msg, (byte)dlc);

                        lock (m_listeners)
                        {
                            //AddToCanTrace("RX: " + canMessage.getID().ToString("X3") + " " + canMessage.getLength().ToString("X1") + " " + canMessage.getData().ToString("X16"));
                            //Console.WriteLine("MSG: " + rxMessage);
                            foreach (ICANListener listener in m_listeners)
                            {
                                listener.handleMessage(canMessage);
                            }
                            //CastInformationEvent(canMessage); // <GS-05042011> re-activated this function
                        }
                    }
                    else if (status == Canlib.canStatus.canERR_NOMSG)
                    {
                        Thread.Sleep(1);
                    }
                }
                else
                {
                    logger.Debug("error frame");
                }
            }
        }

        /// <summary>
        /// The open method tries to connect to both busses to see if one of them is connected and
        /// active. The first strategy is to listen for any CAN message. If this fails there is a
        /// check to see if the application is started after an interrupted flash session. This is
        /// done by sending a message to set address and length (only for P-bus).
        /// </summary>
        /// <returns>OpenResult.OK is returned on success. Otherwise OpenResult.OpenError is
        /// returned.</returns>
        override public OpenResult open()
        {
            Canlib.canInitializeLibrary();

            //Check if bus is connected
            if (isOpen())
            {
                close();
            }
            Thread.Sleep(200);
            m_readThread = new Thread(readMessages) { Name = "KvaserCANDevice.m_readThread" };

            m_endThread = false;

            // Take first adapter...
            string[] adapters = GetAdapterNames();
            if (adapters.Length == 0)
            {
                return OpenResult.OpenError;
            }
            SetSelectedAdapter(adapters[0]);

            //Check if P bus is connected
            logger.Debug("handle1 = canlibCLSNET.Canlib.canOpenChannel()");
            handleWrite = Canlib.canOpenChannel(ChannelNumber, 0);
            logger.Debug("canlibCLSNET.Canlib.canSetBusParams(handleWrite)");
            Canlib.canStatus statusSetParamWrite = Canlib.canSetBusParamsC200(handleWrite, 0x40, 0x37);
            logger.Debug("canlibCLSNET.Canlib.canBusOn(handleWrite)");
            Canlib.canStatus statusOnWrite = Canlib.canBusOn(handleWrite);
            Canlib.canIoCtl(handleWrite, Canlib.canIOCTL_SET_LOCAL_TXECHO, 0);
            //
            logger.Debug("handle2 = canlibCLSNET.Canlib.canOpenChannel()");
            handleRead = Canlib.canOpenChannel(ChannelNumber, 0);
            logger.Debug("canlibCLSNET.Canlib.canSetBusParams(handleRead)");
            Canlib.canStatus statusSetParamRead = Canlib.canSetBusParamsC200(handleRead, 0x40, 0x37);
            logger.Debug("canlibCLSNET.Canlib.canBusOn(handleRead)");
            Canlib.canStatus statusOnRead = Canlib.canBusOn(handleRead);
            Canlib.canIoCtl(handleRead, Canlib.canIOCTL_SET_LOCAL_TXECHO, 0);

            if (handleWrite < 0 || handleRead < 0)
            {
                return OpenResult.OpenError;
            }

            logger.Debug("P bus connected");
            if (m_readThread.ThreadState == ThreadState.Unstarted)
            {
                m_readThread.Start();
            }
            return OpenResult.OK;
        }

        /// <summary>
        /// The close method closes the device.
        /// </summary>
        /// <returns>CloseResult.OK on success, otherwise CloseResult.CloseError.</returns>
        override public CloseResult close()
        {
            m_endThread = true;

            Canlib.canStatus statusBusOff1 = Canlib.canStatus.canOK;
            Canlib.canStatus statusBusOff2 = Canlib.canStatus.canOK;
            Canlib.canStatus statusCanClose1 = Canlib.canStatus.canOK;
            Canlib.canStatus statusCanClose2 = Canlib.canStatus.canOK;

            if (handleWrite >= 0)
            {
                statusBusOff1 = Canlib.canBusOff(handleWrite);
                logger.Debug("canlibCLSNET.Canlib.canBusOff(handleWrite)");
                statusCanClose1 = Canlib.canClose(handleWrite);
                logger.Debug("canlibCLSNET.Canlib.canClose(handleWrite)");
            }

            if (handleRead >= 0)
            {
                statusBusOff2 = Canlib.canBusOff(handleRead);
                logger.Debug("canlibCLSNET.Canlib.canBusOff(handleRead)");
                statusCanClose2 = Canlib.canClose(handleRead);
                logger.Debug("canlibCLSNET.Canlib.canClose(handleRead)");
            }

            handleWrite = -1;
            handleRead = -1;
            if (Canlib.canStatus.canOK == statusBusOff1 && Canlib.canStatus.canOK == statusBusOff2 &&
                Canlib.canStatus.canOK == statusCanClose1 && Canlib.canStatus.canOK == statusCanClose2)
            {
                return CloseResult.OK;
            }
            else
            {
                return CloseResult.CloseError;
            }
        }

        /// <summary>
        /// isOpen checks if the device is open.
        /// </summary>
        /// <returns>true if the device is open, otherwise false.</returns>
        override public bool isOpen()
        {
            if (handleWrite >= 0 && handleRead >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// sendMessage send a CANMessage.
        /// </summary>
        /// <param name="a_message">A CANMessage.</param>
        /// <returns>true on success, othewise false.</returns>
        override public bool sendMessage(CANMessage a_message)
        {
            byte[] msg = a_message.getDataAsByteArray();

            writeStatus = Canlib.canWrite(handleWrite, (int)a_message.getID(), msg, a_message.getLength(), 0);

            if (writeStatus == Canlib.canStatus.canOK)
            {
                return true;
            }
            else
            {
                logger.Debug(String.Format("tx failed with status {0}", writeStatus));
                return false;
            }
        }
    }
}
