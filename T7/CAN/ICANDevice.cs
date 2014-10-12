using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using CommonSuite;

namespace T7.CAN
{
    /// <summary>
    /// OpenResult is returned by the open method to report the status of the opening.
    /// </summary>
    public enum OpenResult
    {
        OK,
        OpenError
    }
    /// <summary>
    /// CloseResult is returned by the close method to report the status of the closening.
    /// </summary>
    public enum CloseResult
    {
        OK,
        CloseError
    }

    /// <summary>
    /// ICANDevice is an interface class for CAN devices. It is used to hide the differences 
    /// there are in the CAN drivers from different manufactureres (since there is no 
    /// standardised driver model for CAN devices). 
    /// For each new CAN device there must be a class that inherits from this and all
    /// the abstract methods must be implemented in the sub class.
    /// </summary>
    public abstract class ICANDevice
    {
        public class InformationFrameEventArgs : System.EventArgs
        {
            private CANMessage _message;

            internal CANMessage Message
            {
                get
                {
                    return _message;
                }
                set
                {
                    _message = value;
                }
            }

            public InformationFrameEventArgs(CANMessage message)
            {
                this._message = message;
            }
        }

        public class InformationEventArgs : System.EventArgs
        {
            private string _info;

            public string Info
            {
                get
                {
                    return _info;
                }
                set
                {
                    _info = value;
                }
            }

            public InformationEventArgs(string info)
            {
                this._info = info;
            }
        }

        public delegate void ReceivedAdditionalInformation(object sender, InformationEventArgs e);
        public event ReceivedAdditionalInformation onReceivedAdditionalInformation;


        public delegate void ReceivedAdditionalInformationFrame(object sender, InformationFrameEventArgs e);
        public event ReceivedAdditionalInformationFrame onReceivedAdditionalInformationFrame;

        public void CastInformationEvent(string info)
        {
            if (onReceivedAdditionalInformation != null)
            {
                onReceivedAdditionalInformation(this, new InformationEventArgs(info));
            }
        }

        public void CastInformationEvent(CANMessage message)
        {
            if (onReceivedAdditionalInformationFrame != null)
            {
                onReceivedAdditionalInformationFrame(this, new InformationFrameEventArgs(message));
            }
        }

        abstract public string ForcedComport
        {
            get;
            set;
        }

        private bool _useOnlyPBus = true;

        public bool UseOnlyPBus
        {
            get { return _useOnlyPBus; }
            set { _useOnlyPBus = value; }
        }

        private bool m_EnableCanLog = false;

        public bool EnableCanLog
        {
            get { return m_EnableCanLog; }
            set { m_EnableCanLog = value; }
        }

        protected bool MessageContainsInformationForRealtime(uint msgId)
        {
            bool retval = false;
            switch (msgId)
            {
                case 0x1A0:         //1A0h - Engine information
                case 0x280:         //280h - Pedals, reverse gear
                case 0x290:         //290h - Steering wheel and SID buttons
                case 0x2F0:         //2F0h - Vehicle speed
                case 0x320:         //320h - Doors, central locking and seat belts
                case 0x370:         //370h - Mileage
                case 0x3A0:         //3A0h - Vehicle speed
                case 0x3B0:         //3B0h - Head lights
                case 0x3E0:         //3E0h - Automatic Gearbox
                case 0x410:         //410h - Light dimmer and light sensor
                case 0x430:         //430h - SID beep request (interesting for Knock indicator?)
                case 0x460:         //460h - Engine rpm and speed
                case 0x4A0:         //4A0h - Steering wheel, Vehicle Identification Number
                case 0x520:         //520h - ACC, inside temperature
                case 0x530:         //530h - ACC
                case 0x5C0:         //5C0h - Coolant temperature, air pressure
                case 0x630:         //630h - Fuel usage
                case 0x640:         //640h - Mileage
                case 0x7A0:         //7A0h - Outside temperature
                    retval = true;
                    break;
            }
            return retval;
        }

        protected void AddToCanTrace(string line)
        {
            if (this.EnableCanLog)
            {
                DateTime dtnow = DateTime.Now;
                using (StreamWriter sw = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\CanTraceCANUSBDevice.txt", true))
                {
                    sw.WriteLine(dtnow.ToString("dd/MM/yyyy HH:mm:ss") + " - " + line);
                }
            }
        }
        /// <summary>
        /// This method opens the device for reading and writing.
        /// There is no mechanism for setting the bus speed so this method must
        /// detect this.
        /// </summary>
        /// <returns>OpenResult</returns>
        abstract public OpenResult open();

        abstract public void Flush();
        /// <summary>
        /// This method closes the device for reading and writing.
        /// </summary>
        /// <returns>CloseResult</returns>
        abstract public CloseResult close();

        /// <summary>
        /// This method checks if the CAN device is opened or closed.
        /// </summary>
        /// <returns>true if device is open, otherwise false</returns>
        abstract public bool isOpen();

        /// <summary>
        /// This message sends a CANMessage to the CAN device.
        /// The open method must have been called and returned possitive result
        /// before this method is called.
        /// </summary>
        /// <param name="a_message">The CANMessage</param>
        /// <returns>true on success, otherwise false.</returns>
        abstract public bool sendMessage(CANMessage a_message);

        /// <summary>
        /// This method adds a ICANListener. Any number of ICANListeners can be added (well,
        /// it's limited to processor speed and memory).
        /// </summary>
        /// <param name="a_listener">The ICANListener to be added.</param>
        /// <returns>true on success, otherwise false.</returns>
        public bool addListener(ICANListener a_listener)
        {
            lock (m_listeners)
            {
                m_listeners.Add(a_listener);
            }
            return true;
        }

        abstract public float GetThermoValue();
        abstract public float GetADCValue(uint channel);

        abstract public uint waitForMessage(uint a_canID, uint timeout, out CANMessage canMsg);
        /// <summary>
        /// This method removes a ICANListener.
        /// </summary>
        /// <param name="a_listener">The ICANListener to remove.</param>
        /// <returns>true on success, otherwise false</returns>
        public bool removeListener(ICANListener a_listener)
        {
            lock (m_listeners)
            {
                m_listeners.Remove(a_listener);
            }
            return true;
        }

        protected List<ICANListener> m_listeners = new List<ICANListener>();


        /*
#define AUDIO   0
#define SID     1
#define ACC     2
#define MIU     3
#define TWICE   4
#define TRIONIC 5
#define CDC     6   // needs more research...

// Global constants 
const unsigned char init_table[] = { 0x81, 0x65, 0x98, 0x61, 0x45, 0x11, 0x28 };
const unsigned char unit_table[] = { 0x91, 0x96, 0x98, 0x9A, 0x9B, 0xA1 };
const int reply_table[] = { 0x248, 0x24B, 0x24D, 0x24E, 0x24F, 0x258 };
const int init_reply_table[] = { 0x228, 0x22B, 0x22D, 0x22E, 0x22F, 0x238 };
         * */

        private uint GetReplyforUnit(byte unit)
        {
            uint retval = 0x258;
            switch (unit)
            {
                case 0x81:              // AUDIO
                    retval = 0x248;
                    break;
                case 0x65:              // SID
                    retval = 0x24B;
                    break;
                case 0x98:
                    retval = 0x24D;     //ACC
                    break;
                case 0x61:
                    retval = 0x24E;     //MIU
                    break;
                case 0x45:
                    retval = 0x24F;     //TWICE
                    break;
                case 0x11:
                    retval = 0x258;     //TRIONIC
                    break;
            }
            return retval;
        }

        
        
        private uint GetIDforUnit(byte unit)
        {
            uint retval = 0x238;
            switch (unit)
            {
                case 0x81:              //AUDIO
                    retval = 0x228;
                    break;
                case 0x65:              //SID
                    retval = 0x22B;
                    break;
                case 0x98:
                    retval = 0x22D;     //ACC
                    break;
                case 0x61:
                    retval = 0x22E;     //MIU
                    break;
                case 0x45:
                    retval = 0x22F;     //TWICE
                    break;
                case 0x11:
                    retval = 0x238;     //TRIONIC
                    break;
            }
            return retval;
        }



        /// <summary>
        /// Send a message that starts a session. This is used to test if there is 
        /// a connection.
        /// </summary>
        /// <returns></returns>
        public bool sendSessionRequest(byte unit)
        {

            AddToCanTrace("Sending session request to unit 0x" + unit.ToString("X2"));
            for (int i = 0; i < 5; i++)
            {
                CANMessage msg1 = new CANMessage(0x220, 0, 8);
                msg1.setData(0x000040021100813f);
                //msg1.setData(0x000040021100813f);
                msg1.setCanData(unit, 5); // overwrite the 0x11 with UNIT
                AddToCanTrace("Sending: " + msg1.getData().ToString("X16") + " id: 0x" + msg1.getID().ToString("X4") + " " + i.ToString());
                if (!sendMessage(msg1))
                {
                    AddToCanTrace("Unable to send session request");
                    return false;
                }
                uint reponseID = GetIDforUnit(unit);
                AddToCanTrace("Waiting for ID: 0x" + reponseID.ToString("X4"));
                if (waitForMessage(reponseID, 500, out msg1) == reponseID)
                {
                    AddToCanTrace("ResponseID seen");
                    LogHelper.Log("ResponseID seen");
                    return true;
                }
                AddToCanTrace("no reponse seen from unit 0x" + unit.ToString("X2"));
                LogHelper.Log("no reponse seen from unit 0x" + unit.ToString("X2"));
            }
            return false;
        }

        public int query_data(byte unit, byte data_id, out byte[] answer)
        {
            byte[] data = new byte[8];
            byte length= 0;
            byte i = 0;
            answer = new byte[8]; // test <GS-11012011>
            int rcv_length;
            byte[] query = new byte[8] { 0x40, 0x00, 0x02, 0x21, 0x00, 0x00, 0x00, 0x00 };
            byte[] ack = new byte[8] { 0x40, 0x00, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x00 };
            //ulong query_long = 0x0000000021020040;
            //ulong ack_long = 0x00000000003F0040;
            query[1] = unit;
            ack[1] = unit;
            // If data_id is zero, decrease length field
            if (data_id != 0x00) query[4] = data_id;
            else query[2] = 0x01;

            data[0] = 0x00;
            rcv_length = 0;

            //for (int i = 0; i < 5; i++)
            {

                CANMessage msg = new CANMessage(0x240, 0, 8);
                //msg.setData(query_long);
                msg.setCanData(query[0], 0);
                msg.setCanData(query[1], 1);
                msg.setCanData(query[2], 2);
                msg.setCanData(query[3], 3);
                msg.setCanData(query[4], 4);
                msg.setCanData(query[5], 5);
                msg.setCanData(query[6], 6);
                msg.setCanData(query[7], 7);
                msg.setID(0x240);
                AddToCanTrace("Sending: 0x" + msg.getData().ToString("X16") + " id: 0x" + msg.getID().ToString("X4"));

                if (!sendMessage(msg))
                {
                    return -1;
                }
                uint reply_unit = GetReplyforUnit(unit);
                reply_unit = 0x258; // for testing purposes
                while (data[0] != 0x80 && data[0] != 0xC0)
                {
                    CANMessage replyMessage = new CANMessage();
                    AddToCanTrace("Waiting for ID: 0x" + reply_unit.ToString("X4"));
                    if (waitForMessage(reply_unit, 1000, out replyMessage) == reply_unit)
                    {
                        AddToCanTrace("Rx data: " + replyMessage.getData().ToString("X16"));
                        data[0] = replyMessage.getCanData(0);
                        data[1] = replyMessage.getCanData(1);
                        data[2] = replyMessage.getCanData(2);
                        data[3] = replyMessage.getCanData(3);
                        data[4] = replyMessage.getCanData(4);
                        data[5] = replyMessage.getCanData(5);
                        data[6] = replyMessage.getCanData(6);
                        data[7] = replyMessage.getCanData(7);
                        int idx = 0;
                        if ((data[0] & 0x40) > 0)
                        {
                            if (data[2] > 0x02)
                            {
                                length = data[2];   // subtract two non-payload bytes
                                length -= 2;
                                answer = new byte[length];
                            }
                            else length = 0;

                            if (--length > 0)
                            {
                                answer[idx++] = data[5];
                                rcv_length++;
                            }
                            if (--length > 0)
                            {
                                answer[idx++] = data[6];
                                rcv_length++;
                            }
                            if (--length > 0)
                            {
                                answer[idx++] = data[7];
                                rcv_length++;
                            }
                        }
                        else
                        {
                            for (i = 0; i < 6; i++)
                            {
                                answer[idx++] = data[2 + i];
                                length--;
                                rcv_length++;
                                if (length == 0) i = 6;
                            }
                        }
                        // Send acknowledgement
                        ack[3] = Convert.ToByte(data[0] & 0xBF);
                        CANMessage ackMessage = new CANMessage();
                        //ackMessage.setData(ack_long);
                        ackMessage.setCanData(ack[0], 0);
                        ackMessage.setCanData(ack[1], 1);
                        ackMessage.setCanData(ack[2], 2);
                        ackMessage.setCanData(ack[3], 3);
                        ackMessage.setCanData(ack[4], 4);
                        ackMessage.setCanData(ack[5], 5);
                        ackMessage.setCanData(ack[6], 6);
                        ackMessage.setCanData(ack[7], 7);

                        ackMessage.setID(0x266);
                        sendMessage(ackMessage);
                    }
                    else
                    {
                        // Timeout
                        AddToCanTrace("Timeout waiting for 0x" + reply_unit.ToString("X3"));
                        return -1;
                    }
                }
            }
            return rcv_length;
        }
    }
}
