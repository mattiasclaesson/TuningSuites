using System;
using System.Collections.Generic;
using System.Text;

namespace WakeupCANbus
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

        protected List<ICANListener> m_listeners = new List<ICANListener>();
    }
}
