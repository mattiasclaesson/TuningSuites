using System;
using System.Collections.Generic;
using System.Text;

namespace T7.KWP
{
    /// <summary>
    /// KWPReply represents a KWP reply. See KWPCANHandler for description of KWP.
    /// </summary>
    public class KWPReply
    {
        byte[] m_reply;
        uint m_nrOfPid;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="a_reply">Data representing all bytes in the reply.</param>
        /// <param name="a_nrOfPid">Number of PIDs. Should originate from the KWPRequest.</param>
        public KWPReply(byte[] a_reply, uint a_nrOfPid)
        {
            if (a_nrOfPid > 2)
                throw new Exception("Nr of PID out of range");
            m_reply = a_reply;
            if (m_reply == null)
            {
                Console.WriteLine("Reply was NULL");
            }
            m_nrOfPid = a_nrOfPid;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public KWPReply()
        {
        }

        /// <summary>
        /// Set the data in the KWPReply.
        /// </summary>
        /// <param name="a_data">The data.</param>
        public void setData(byte[] a_data)
        {
            m_reply = a_data;
        }

        /// <summary>
        /// Set the number of PIDs there are in this KWPReply.
        /// </summary>
        /// <param name="a_nr">Number of PIDs.</param>
        public void setNrOfPID(uint a_nr) { m_nrOfPid = a_nr; }

        /// <summary>
        /// Get the number of PIDs there are in this KWPReply.
        /// </summary>
        /// <returns>Number of PIDs in this reply.</returns>
        public uint getNrOfPID() { return m_nrOfPid; }

        /// <summary>
        /// Get the length of the KWPReply.
        /// Note. The array containing the data in a KWPReply may be longer than the KWP length.
        /// </summary>
        /// <returns>The length of the KWP reply.</returns>
        public byte getLength()
        {
            return m_reply[0];
        }

        /// <summary>
        /// Get the mode of the KWPReply.
        /// </summary>
        /// <returns>The mode of the KWP reply.</returns>
        public byte getMode()
        {
            if (m_reply != null)
            {
                return m_reply[1];
            }
            return 0;
        }

        /// <summary>
        /// Get the PID of this KWPReply.
        /// This method should be used if there is only one PID.
        /// </summary>
        /// <returns>The PID.</returns>
        public byte getPid()
        {
            return m_reply[2];
        }

        /// <summary>
        /// Get the high PID (the first PID).
        /// This method should be used if there are two PIDs in a KWPReply.
        /// </summary>
        /// <returns>The high PID.</returns>
        public byte getPidHigh()
        {
            return m_reply[2];
        }

        /// <summary>
        /// Get the low PID (the second PID).
        /// This method should be used if there are two PIDs in a KWPReply.
        /// </summary>
        /// <returns>The low PID.</returns>
        public byte getPidLow()
        {
            return m_reply[3];
        }

        /// <summary>
        /// Returns the KWPReply as a byte array.
        /// Note that the length of the byte array might be longer than then length
        /// of the contained KWP reply.
        /// </summary>
        /// <returns>Byte array representing the KWP reply.</returns>
        public byte[] getData()
        {
            uint length = 0;
            if (getLength() == 1)
            {
                length = 1;
                byte[] data = new byte[length];
                data[0] = m_reply[1];
                return data;
            }
            else
            {
                length = getLength() - m_nrOfPid - 1;
                byte[] data = new byte[length];
                uint i;
                if (m_nrOfPid == 1)
                    i = 3;
                else
                    i = 4;
                for (uint j = 0; j < data.Length; i++, j++)
                    data[j] = m_reply[i];
                return data;
            }
            /*uint length = 0;
            if (getLength() == 1)
                length = 1;
            else
                length = getLength() - m_nrOfPid - 1;
            byte[] data = new byte[length];
            if (length == 1)
            {
                data[0] = m_reply[1];
                return data;
            }
            uint i;
            if (m_nrOfPid == 1)
                i = 3;
            else
                i = 4;
            for (uint j = 0; j < data.Length; i++, j++)
                data[j] = m_reply[i];
            return data;*/
        }

        override public String ToString()
        {
            if(m_reply == null)
                return "Empty reply";
            if(m_reply.Length == 0)
                return "Empty reply";
            StringBuilder hex = new StringBuilder();
            hex.Append("Reply:   ");
            for (int i = 0; i < m_reply[0]+1; i++)
            {
                hex.Append(m_reply[i].ToString("X2") + ",");
            }
            hex.Remove(hex.Length - 1, 1);
            return hex.ToString();
        }
    }
}
