using System;
using System.Collections.Generic;
using System.Text;

namespace WakeupCANbus
{
    /// <summary>
    /// KWPRequst represents a KWP request message (see KWPCANDevice for description of KWP).
    /// </summary>
    public class KWPRequest
    {

        byte[] m_request;
        uint m_nrOfPid;

        /// <summary>
        /// Get the number of PIDs in this KWPRequest.
        /// </summary>
        /// <returns>The number of PIDs.</returns>
        public uint getNrOfPID() { return m_nrOfPid; }

        /// <summary>
        /// Constructor for request with one PID.
        /// </summary>
        /// <param name="a_mode">The mode.</param>
        /// <param name="a_pid">The PID.</param>
        public KWPRequest(byte a_mode, byte a_pid)
        {
            int i = 0;
            byte length = 2;
            m_request = new byte[length + 1];
            m_request[i++] = length;
            m_request[i++] = a_mode;
            m_request[i++] = a_pid;
            m_nrOfPid = 1;
        }

        /// <summary>
        /// Constructor for requests with no PID.
        /// </summary>
        /// <param name="a_mode">The mode.</param>
        /// <param name="a_data">The data.</param>
        public KWPRequest(byte a_mode, byte[] a_data)
        {
            int i = 0;
            byte length = (byte)(1 + a_data.Length);
            m_request = new byte[length + 1];
            //Set length of request
            m_request[i++] = length;
            m_request[i++] = a_mode;
            for (int j = 0; i < m_request.Length; i++, j++)
                m_request[i] = a_data[j];
            m_nrOfPid = 0;
        }

        /// <summary>
        /// Constructor for requests with no PID and no data.
        /// </summary>
        /// <param name="a_mode">The mode.</param>
        public KWPRequest(byte a_mode)
        {
            int i = 0;
            byte length = 1;
            m_request = new byte[length + 1];
            //Set length of request
            m_request[i++] = length;
            m_request[i++] = a_mode;
            m_nrOfPid = 0;
        }

        /// <summary>
        /// Constructor for requests with one PID and data.
        /// </summary>
        /// <param name="a_mode">The mode.</param>
        /// <param name="a_pid">The PID.</param>
        /// <param name="a_data">The data.</param>
        public KWPRequest(byte a_mode, byte a_pid, byte[] a_data)
        {
            int i = 0;
            byte length = (byte)(2 + a_data.Length);
            if(a_mode == 0x3D && a_pid == 0x80)
            {
                Console.WriteLine("KWPRequest length: " + length.ToString("X8"));
            }
            m_request = new byte[length + 1];
            //Set length of request
            m_request[i++] = length;
            m_request[i++] = a_mode;
            m_request[i++] = a_pid;
            for (int j = 0; i < m_request.Length; i++, j++)
                m_request[i] = a_data[j];
            m_nrOfPid = 1;
        }

        /// <summary>
        /// Constructor for requests with two PIDs.
        /// </summary>
        /// <param name="a_mode">The mode.</param>
        /// <param name="a_pidHigh">The high PID (the first PID).</param>
        /// <param name="a_pidLow">The low PID (the second PID).</param>
        /// <param name="a_data">The data.</param>
        public KWPRequest(byte a_mode, byte a_pidHigh, byte a_pidLow, byte[] a_data)
        {
            int i = 0;
            byte length = (byte)(3 + a_data.Length);
            m_request = new byte[length + 1];
            m_request[i++] = length;
            m_request[i++] = a_mode;
            m_request[i++] = a_pidHigh;
            m_request[i++] = a_pidLow;
            for (int j = 0; i < m_request.Length; i++, j++)
                m_request[i] = a_data[j];
            m_nrOfPid = 2;
        }

        /// <summary>
        /// Get the KWPRequest represented as a byte array.
        /// </summary>
        /// <returns>Byte array representing the KWPRequest.</returns>
        public byte[] getData() { return m_request; }


        override public String ToString()
        {
            if (m_request.Length == 0)
                return "Empty reply";
            StringBuilder hex = new StringBuilder();
            hex.Append("Request: ");
            for (int i = 0; i < m_request[0]+1; i++)
            {
                //if (m_request[i] < 10)
                //    hex.Append("0");
                hex.Append(m_request[i].ToString("X2")+",");
            }
            hex.Remove(hex.Length - 1, 1);
            return hex.ToString();
        }
    }
}
