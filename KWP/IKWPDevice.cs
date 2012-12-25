using System;
using System.Collections.Generic;
using System.Text;
using T7.CAN;

namespace T7.KWP
{

    public enum RequestResult 
    {
        NoError,
        ErrorSending,
        Timeout,
        ErrorReceiving,
        Unknown,
        InvalidLength
    } 

    /// <summary>
    /// IKWPDevice is an interface class for KWP devices (Key Word Protocol). 
    /// See KWPCANDevice for a description of KWP.
    /// 
    /// All devices that supports KWP must implementd this interface.
    /// </summary>
    public abstract class IKWPDevice
    {
        /// <summary>
        /// This method starts a new KWP session. It must be called before the sendRequest
        /// method can be called.
        /// </summary>
        /// <returns>true on success, otherwise false.</returns>
        public abstract bool startSession();

        public abstract bool EnableCanLog
        {
            get;
            set;
        }

        public abstract void setCANDevice(ICANDevice a_canDevice);

        /// <summary>
        /// This method sends a KWP request and returns a KWPReply. The method returns
        /// when a reply has been received, after a failure or after a timeout.
        /// The open and startSession methods must be called and returned possitive result
        /// before this method is used.
        /// </summary>
        /// <param name="a_request">The KWPRequest.</param>
        /// <param name="r_reply">The reply to the KWPRequest.</param>
        /// <returns>RequestResult.</returns>
        public abstract RequestResult sendRequest(KWPRequest a_request, out KWPReply r_reply);

        /// <summary>
        /// This method opens a KWP device for usage.
        /// </summary>
        /// <returns>true on success, otherwise false.</returns>
        public abstract bool open();

        /// <summary>
        /// This method closes a KWP device for usage.
        /// </summary>
        /// <returns>true on success, otherwise false.</returns>
        public abstract bool close();

        /// <summary>
        /// This method checks if the IKWPDevice is opened or not.
        /// </summary>
        /// <returns>true if device is open, otherwise false.</returns>
        public abstract bool isOpen();
    }
}
