using System;
using System.Collections.Generic;
using System.Text;

namespace WakeupCANbus
{
    /// <summary>
    /// ICANListener is an interface class for receiveing CANMessages from ICANDevices.
    /// Derived classes must implement the handleMessage method that will receive all 
    /// CANMessages published by the ICANDevice.
    /// There is a possibility to add CAN id filters to remove uniteresting messages.
    /// </summary>
    abstract class ICANListener
    {
        /// <summary>
        /// This method is called by ICANDevices where derived objects of this class
        /// are registered. The method is called for each received CANMessage.
        /// What this method does is application dependent.
        /// </summary>
        /// <param name="a_canMessage">The CANMessage to be handled by this method.</param>
        public abstract void handleMessage(CANMessage a_canMessage);

        /// <summary>
        /// This method adds a filter for messages that shouldn't be handled.
        /// </summary>
        /// <param name="a_canID">The ID of the messages that should be filtered.</param>
        public void addIDFilter(uint a_canID) { m_idFilter.Add(a_canID); }

        /// <summary>
        /// This method removed a filter for messages that shouldn't be handled.
        /// </summary>
        /// <param name="a_canID">The ID of the messages that no longer should be filtered.</param>
        public void removeIDFilter(uint a_canID) { m_idFilter.Remove(a_canID); }

        /// <summary>
        /// List of CAN IDs that should be filtered.
        /// </summary>
        private List<uint> m_idFilter = new List<uint>();
    }
}
