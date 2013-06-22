using System;
using System.Collections.Generic;
using System.Text;

namespace T7.CAN
{
    /// <summary>
    /// The CANMessage class represents a generic CAN message that can be used by virtually
    /// all CAN devices. 
    /// </summary>
    class CANMessage
    {

            /// <summary>
            /// m_id is the CAN id
            /// </summary>
            private uint m_id;			// 11/29 bit Identifier
            /// <summary>
            /// m_timestamp is the time stamp for the message set by the CAN device
            /// </summary>
            private uint m_timestamp;   // Hardware Timestamp (0-9999mS)
            /// <summary>
            /// m_flags is flags set by the CAN device (vendor dependent)
            /// </summary>
            private byte m_flags;		// Message Flags
            /// <summary>
            /// m_length is the number of bytes in the message
            /// </summary>
            private byte m_length;		// Number of data bytes 0-8.
            /// <summary>
            /// m_data is the data contained in the CAN message.
            /// Data is ordered in reverse orded compared to the CAN message. If the message should
            /// contain [0x11,0x22,0x33,0x44,0x55,0x66,0x77,0x88] m_data should have the 
            /// value 0x887766554432211.
            /// </summary>
            private ulong m_data;		// Data Bytes 0..7

        /// <summary>
        /// Constructor for CANMessage
        /// </summary>
        /// <param name="a_id">CAN id</param>
        /// <param name="a_timestamp">Time stamp</param>
        /// <param name="a_flags">Flags</param>
        /// <param name="a_length">Length of data</param>
        /// <param name="a_data">The data</param>
        public CANMessage(uint a_id, uint a_timestamp, byte a_flags, byte a_length, ulong a_data)
        {
            m_id = a_id;
            m_timestamp = a_timestamp;
            m_flags = a_flags;
            m_length = a_length;
            m_data = a_data;
        }

        /// <summary>
        /// Constructor for CANMessage
        /// </summary>
        /// <param name="a_id">CAN id</param>
        /// <param name="a_flags">Flags</param>
        /// <param name="a_length">Length of data</param>
        public CANMessage(uint a_id, byte a_flags, byte a_length)
        {
            m_id = a_id;
            m_timestamp = 0;
            m_flags = a_flags;
            m_length = a_length;
            m_data = 0;
        }

        /// <summary>
        /// Constructor for CANMessage
        /// </summary>
        public CANMessage()
        {
            m_id = 0;
            m_timestamp = 0;
            m_flags = 0;
            m_length = 0;
            m_data = 0;
        }

        public uint getID() { return m_id; }
        public uint getTimeStamp() { return m_timestamp; }
        public byte getFlags() { return m_flags; }
        public byte getLength() { return m_length; }
        public ulong getData() { return m_data; }
        public void setID(uint a_id) { m_id = a_id; }
        public void setTimeStamp(uint a_timeStamp) { m_timestamp = a_timeStamp; }
        public void setFlags(byte a_flags) { m_flags = a_flags; }
        public void setLength(byte a_length) { m_length = a_length; }
        public void setData(ulong a_data) { m_data = a_data; }

        /// <summary>
        /// Set a byte in the data of a CANMessage
        /// </summary>
        /// <param name="a_byte">The byte to set</param>
        /// <param name="a_index">The index of the byte to be set [0..7]</param>
        public void setCanData(byte a_byte, uint a_index)
        {
            if (a_index > 7)
                throw new Exception("Index out of range");
            ulong tmp = (ulong)a_byte;
            tmp = tmp << (int)(a_index * 8);
            m_data = m_data | tmp;
        }

        /// <summary>
        /// Get a byte of the data contained in a CANMessage
        /// </summary>
        /// <param name="a_index"></param>
        /// <returns></returns>
        public byte getCanData(uint a_index)
        {
            return (byte)(m_data >> (int)(a_index * 8));
        }

        public ulong getDataEasySync()
        {
            ulong _data = 0;
            _data |= (ulong)((ulong)((m_data & 0xFF00000000000000)) >> (7 * 8));
            _data |= (ulong)((ulong)((m_data & 0x00FF000000000000)) >> (5 * 8));
            _data |= (ulong)((ulong)((m_data & 0x0000FF0000000000)) >> (3 * 8));
            _data |= (ulong)((ulong)((m_data & 0x000000FF00000000)) >> (1 * 8));
            _data |= (ulong)((ulong)((m_data & 0x00000000FF000000)) << (1 * 8));
            _data |= (ulong)((ulong)((m_data & 0x0000000000FF0000)) << (3 * 8));
            _data |= (ulong)((ulong)((m_data & 0x000000000000FF00)) << (5 * 8));
            _data |= (ulong)((ulong)((m_data & 0x00000000000000FF)) << (7 * 8));
            return _data;
        }

    }
}
