/*
** Client.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** TCP ClientReader used to process FIX message strings from tcp input stream
**
** Changes
** -------
**
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;

using MESSAGEFIX3Lib;

namespace VDMERLib.EasyRouter.TCP
{
    /// <summary>
    /// Class ClientReader
    /// </summary>
    public class ClientReader
    {
         /// <summary>
        /// true for thread loop
        /// </summary>
        bool m_bContinue = true;

        /// <summary>
        /// data input stream
        /// </summary>
        StreamReader m_StreamIn = null;

        /// <summary>
        /// Reader thread for data from tcp server 
        /// </summary>
        Thread m_ReaderThread = null; 

        /// <summary>
        /// SOH character used to parse FIX string 
        /// </summary>
        private static readonly string m_sDelimiter = "\x01";

        /// <summary>
        /// event delegate
        /// </summary>
        /// <param name="from"></param>
        /// <param name="args"></param>
        public delegate void EventFIXMessage(object from, FIXMessageEventArgs args);

        /// <summary>
        /// connection event 
        /// </summary>
        public event EventFIXMessage DataEvent;

        /// <summary>
        /// event delegate
        /// </summary>
        /// <param name="args"></param>
        public delegate void EventDisconnectMessage(ErrorEventArgs args);

        /// <summary>
        /// connection event 
        /// </summary>
        public event EventDisconnectMessage DisconnectEvent;

        /// <summary>
        /// DataReader
        /// </summary>
        private StringBuilder m_sTCPBuffer = new StringBuilder(5001);

        /// <summary>
        /// vanilla constuctor - take in the read stream from TCP server
        /// </summary>
        /// <param name="streamIn"></param>
        public ClientReader(StreamReader streamIn)
        {
            m_StreamIn = streamIn;
        }

        /// <summary>
        /// Function to process data off the tcp stream - called from the worker thread
        /// </summary>
        public void ReadStream()
        {

            try
            {
                MESSAGEFIX3Lib.FIXMessage msg = new FIXMessage();

                char[] message = new char[1024];

                while (m_bContinue)
                {
                    int nSize = m_StreamIn.ReadBlock(message, 0, 1024);

                    if (nSize <= 0)
                        continue;
#if DEBUG
                    System.Diagnostics.Trace.WriteLine("In Buffer" + nSize);
#endif
                    m_sTCPBuffer.Append(message, 0, nSize);

#if DEBUG
                    System.Diagnostics.Trace.WriteLine(m_sTCPBuffer.ToString());
#endif

                    bool bContinue = true;

                    while(bContinue)
                    {
                        string sData = m_sTCPBuffer.ToString();

                        int nheader = sData.IndexOf("8=");
                        int nTail = sData.IndexOf("10=");

                        if (nTail <= 0)
                        {
                            bContinue = false;
                            continue;
                        }
                        int nEnd = sData.IndexOf("!", nTail);
                        int nEnd1 = sData.IndexOf("8=", nTail);

                        if (nEnd1 == -1)
                        {
                            bContinue = false;
                        }
                        else 
                        {
                            sData = sData.Substring(nheader, nEnd1 - nheader);

#if DEBUG
                            System.Diagnostics.Trace.WriteLine("Buffer Current" + m_sTCPBuffer.Length);
#endif
                            m_sTCPBuffer.Remove(0, nEnd1);
#if DEBUG
                            System.Diagnostics.Trace.WriteLine("Buffer Data Remainder" + m_sTCPBuffer.Length);
#endif

                            //find the specific string

                            string sTemp = string.Empty;

                            //parse fix messagge
                            bool bSuccess = msg.Parse(sData, m_sDelimiter, false);

                            if (bSuccess)
                            {
                                if (DataEvent != null)
                                {
                                    DataEvent(this, new FIXMessageEventArgs(msg));
                                }
                            }
                            else
                            {
                                //Failed to parse data
#if DEBUG
                                System.Diagnostics.Trace.WriteLine("Parse Error = " + sTemp);
#endif
                            }

                            //clear the object
                            msg.Clear();
                         }
                    }
                 }
            }
            catch (Exception e)
            {
                //Log - raise an event 
                if (DisconnectEvent != null)
                    DisconnectEvent(new ErrorEventArgs(e));
            }
        }

        /// <summary>
        /// Start the reading thread - process data off tcp strem in
        /// </summary>
        public void Start()
        { 
            m_ReaderThread = new Thread(new ThreadStart(this.ReadStream));
            m_ReaderThread.Start();
        }

        /// <summary>
        /// Stop the worker thread , processing the tcp server data 
        /// </summary>
        public void Stop()
        {
            m_bContinue = false;
            m_ReaderThread.Abort();
            m_ReaderThread.Join();  
        }

        /// <summary>
        /// Returns the SOH delimiter for fix message
        /// </summary>
        static public string Delimiter
        {
            get { return m_sDelimiter; } 
        }
    }
}
