using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

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
    abstract class ICANDevice
    {
        /// <summary>
        /// This method opens the device for reading and writing.
        /// There is no mechanism for setting the bus speed so this method must
        /// detect this.
        /// </summary>
        /// <returns>OpenResult</returns>
        abstract public OpenResult open();

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
            lock(m_listeners)
            {
                m_listeners.Add(a_listener);
            }
            return true;
        }

        /// <summary>
        /// This method removes a ICANListener.
        /// </summary>
        /// <param name="a_listener">The ICANListener to remove.</param>
        /// <returns>true on success, otherwise false</returns>
        public bool removeListener(ICANListener a_listener) 
        {
            lock(m_listeners)
            {
                m_listeners.Remove(a_listener);
            }
            return true;
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

        protected List<ICANListener> m_listeners = new List<ICANListener>();
    }
}
