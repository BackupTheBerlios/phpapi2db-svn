/*
** Client.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** TCP Client used to connect to TCP Server and process FIX messages
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

namespace VDMERLib.EasyRouter.TCP
{
    /// <summary>
    /// Class Client
    /// </summary>
    public class Client
    {
        /// <summary>
        /// enum representing state of the tcp connection
        /// </summary>
        public enum State
        { 
            /// <summary>
            /// unknown
            /// </summary>
            Unknown,
            /// <summary>
            /// connecting
            /// </summary>
            Connecting,
            /// <summary>
            /// connected
            /// </summary>
            Connected,
            /// <summary>
            /// disconnected
            /// </summary>
            Disconnected,
            /// <summary>
            /// disconnected
            /// </summary>
            Disconnecting,
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="nPort">ip port to connect</param>
        /// <param name="sIPAddress">address to connect to</param>
        public Client(int nPort, string sIPAddress)
        {
            m_nPort = nPort;
            m_sIPAddress = sIPAddress;
            m_Client = new TcpClient();
            m_ConnectionState = State.Unknown;
        }

        /// <summary>
        /// destructor
        /// </summary>
        ~Client()
        {
            if (m_ConnectionState == State.Connected || m_ConnectionState == State.Connecting)
            {
                Stop();
            }
        }

        /// <summary>
        /// Begin thread to connect
        /// </summary>
        public bool Start()
        {
            if (m_ConnectionState == State.Disconnecting)
                return false;
            //do a clean up if we had previous connection open
            if (m_ConnectionState == State.Connected || m_ConnectionState == State.Disconnected)
                Stop();
            m_ConnectionThread = new Thread(new ThreadStart(this.Connect));
            m_ConnectionThread.Start();
            return true;
        }

        /// <summary>
        /// connection to tcp server
        /// </summary>
        public void Connect()
        {
            if(m_ConnectionState != State.Disconnecting)
            {
                while (m_bContinue)
                {
                    try
                    {
                        m_ConnectionState = State.Connecting; 
                        //attempt tcp connect
                        Socket soc = m_Client.Client;
                        AddressFamily fam = soc.AddressFamily;
                        ProtocolType tpye =  soc.ProtocolType;
                        
                        m_Client.Connect(m_sIPAddress, m_nPort);
                        m_bContinue = false;
                        m_ConnectionState = State.Connected; 

                        //raise connection event 
                        if(ConnectionEvent!=null)
                            ConnectionEvent(this,new ConnectionEventArgs(this.m_ConnectionState));

                        //get the io data streams
                        m_NetworkStream = m_Client.GetStream();
                        m_StreamOut = new StreamWriter(m_NetworkStream);
                        m_StreamIn = new StreamReader(m_NetworkStream,Encoding.ASCII,true,1024);

                        //set up the stream reader
                        m_Reader = new ClientReader(m_StreamIn);
                        //set up event handler to recieve data events from reader class on packet arrival from tcp server
                        SetEventFIXMessageHandler(true);
                        //start thresad to read data from server

                        m_Reader.Start();  
                    }
                    catch(Exception)
                    { 
                        //log   
                        SetEventFIXMessageHandler(false);
                        Thread.Sleep(m_nTimeOut); 
                    }
                }
            }
            m_ConnectionThread = null;
        }
        
        /// <summary>
        /// stop connection thread and tcp connection if established
        /// </summary>
        public void Stop()
        {
            if(m_ConnectionState == State.Disconnected)
            {
                if (m_Client != null)
                {
                    //if (m_Client.Connected)
                    {
                        if (m_NetworkStream != null)
                        {
                            m_StreamOut.Close();
                            m_StreamIn.Close();
                            m_NetworkStream.Close();
                            m_Reader.Stop();
                            m_Reader = null;
                            m_NetworkStream = null;
                            m_StreamOut = null;
                            m_StreamIn = null;
                        }
                        m_Client.Close();
                        m_Client = null;
                    }
                }
            }
            else if (m_ConnectionState != State.Disconnecting)
            {
                lock (this)
                {
                    m_ConnectionState = State.Disconnecting;
                    if (m_ConnectionThread != null)
                    {
                        m_bContinue = false;
                        m_ConnectionThread.Abort();
                        m_ConnectionThread.Join();
                        m_ConnectionThread = null;
                    }

                    if (m_Reader != null)
                    {
                        SetEventFIXMessageHandler(false);
                    }

                   
                    m_ConnectionState = State.Disconnected;
                }
            }
          
        }

        /// <summary>
        /// write data to the tcp server 
        /// </summary>
        /// <param name="msg"></param>
        bool SendFIXMsg(MESSAGEFIX3Lib.FIXMessage msg)
        {
            bool bSuccess = false;
            string sMessage = (string)msg.Render(ClientReader.Delimiter, MESSAGEFIX3Lib.FIXRenderConstants.esFIXRenderFix42);

            if (sMessage != null)
            {
                if (sMessage.Length > 0)
                {
                    try
                    {
                        //write to stream
                        m_StreamOut.Write(sMessage.ToCharArray());
                        m_StreamOut.Flush();
                        bSuccess = true;
                    }
                    catch(Exception e)
                    {
                        //log
                        Disconnect(new ErrorEventArgs(e));
                    }
                }
            }
            return bSuccess; 
        }

        /// <summary>
        /// set up event handler to recieve fix data from client reader tcp data stream
        /// </summary>
        /// <param name="bEnable"></param>
        protected void SetEventFIXMessageHandler(bool bEnable)
        {
            if (m_Reader != null)
            {
                if (bEnable)
                {
                    m_Reader.DataEvent += new ClientReader.EventFIXMessage(this.EventFIXMessage);
                    m_Reader.DisconnectEvent += new ClientReader.EventDisconnectMessage(this.Disconnect); 
                }
                else 
                {
                    m_Reader.DataEvent -= new ClientReader.EventFIXMessage(this.EventFIXMessage);
                    m_Reader.DisconnectEvent -= new ClientReader.EventDisconnectMessage(this.Disconnect); 
                }
            }
        }
        
        /// <summary>
        /// Event raised when FIX data mesage is recieved 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="args"></param>
        public void EventFIXMessage(object from, FIXMessageEventArgs args)
        {
            MESSAGEFIX3Lib.FIXMsgConstants msgType = args.MsgType;
            MESSAGEFIX3Lib.FIXMessage msg = args.Msg;

            if (msg != null)
            {
#if DEBUG
                //string sMessage = (string)msg.Render(ClientReader.Delimiter, MESSAGEFIX3Lib.FIXRenderConstants.esFIXRenderFix42);
                //System.Diagnostics.Debug.WriteLine(msgType.ToString() + " - " + sMessage);   
#endif
                RecvFIXMsg(msgType, msg);
            }
            args = null;
        }

        /// <summary>
        /// virtual function to handle incoming data
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="msg"></param>
        public virtual void RecvFIXMsg(MESSAGEFIX3Lib.FIXMsgConstants msgType,MESSAGEFIX3Lib.FIXMessage msg)  
        {
            switch (msgType)
            {
                case MESSAGEFIX3Lib.FIXMsgConstants.esFIXMsgOrderCancelReject: RecvFIXOrderMsg(msg); break;
                case MESSAGEFIX3Lib.FIXMsgConstants.esFIXMsgBusinessReject: RecvFIXOrderMsg(msg); break;
                case MESSAGEFIX3Lib.FIXMsgConstants.esFIXMsgExecutionReport: RecvFIXOrderMsg(msg); break;
                case MESSAGEFIX3Lib.FIXMsgConstants.esFIXMsgReject: RecvFIXOrderMsg(msg); break;

                case MESSAGEFIX3Lib.FIXMsgConstants.esFIXMsgMarketDataSnapFull: RecvFIXPriceMsg(msg); break;
                case MESSAGEFIX3Lib.FIXMsgConstants.esFIXMsgMarketDataSnapInc: RecvFIXPriceMsg(msg); break;
                case MESSAGEFIX3Lib.FIXMsgConstants.esFIXMsgQuote : RecvFIXPriceMsg(msg); break;

                case MESSAGEFIX3Lib.FIXMsgConstants.esFIXMsgSecuritySts: RecvFIXStatusMsg(msg); break;
                case MESSAGEFIX3Lib.FIXMsgConstants.esFIXMsgTradingSessSts : RecvFIXStatusMsg(msg); break;

                case MESSAGEFIX3Lib.FIXMsgConstants.esFIXMsgLogout: RecvFIXDriverMsg(msg); break;
                case MESSAGEFIX3Lib.FIXMsgConstants.esFIXMsgESComponentStatus : RecvFIXDriverMsg(msg); break;
                case MESSAGEFIX3Lib.FIXMsgConstants.esFIXMsgEHControl : RecvFIXDriverMsg(msg); break;
                case MESSAGEFIX3Lib.FIXMsgConstants.esFIXMsgEHError : RecvFIXDriverMsg(msg); break;
                default: /*do nothing*/ break;
            }
        }

        /// <summary>
        /// default implemtation to driver status message + logout from exchange orders - does nothing 
        /// </summary>
        /// <param name="msg"></param>
        public virtual void RecvFIXDriverMsg(MESSAGEFIX3Lib.FIXMessage msg){/*override*/}
        /// <summary>
        /// default implemtation to handle orders - does nothing 
        /// </summary>
        /// <param name="msg"></param>
        public virtual void RecvFIXOrderMsg(MESSAGEFIX3Lib.FIXMessage msg){/*override*/}
        /// <summary>
        /// default implemtation to handle market price data - does nothing 
        /// </summary>
        /// <param name="msg"></param>
        public virtual void RecvFIXPriceMsg(MESSAGEFIX3Lib.FIXMessage msg){/*override*/}
        /// <summary>
        /// default implemtation to handle market status - does nothing 
        /// </summary>
        /// <param name="msg"></param>
        public virtual void RecvFIXStatusMsg(MESSAGEFIX3Lib.FIXMessage msg) {/*override*/}

        /// <summary>
        ///  disconnect event has occured from tcp layer
        /// </summary>
        /// <param name="e">Exception that caused the disconnect store in the ErrorEventArgs</param>
        public void Disconnect(ErrorEventArgs e)
        {
            //kill connection objects and thrreads
            Stop();

            //signal listening event we are dead
            if (ConnectionEvent != null)
            {
#if DEBUG
                Exception error = e.GetException(); 
                System.Diagnostics.Trace.WriteLine(error.Message);
#endif
                //raise event to any listening client 
                ConnectionEvent(this,new ConnectionEventArgs(m_ConnectionState));  
            }
        }

        #region Members

        /// <summary>
        /// Connection thread
        /// </summary>
        Thread m_ConnectionThread = null;

        /// <summary>
        /// state of connection
        /// </summary>
        State m_ConnectionState = State.Unknown;

        /// <summary>
        /// connection port
        /// </summary>
        int m_nPort = 0;

        /// <summary>
        /// ipaddress of server to connect to
        /// </summary>
        string m_sIPAddress = string.Empty;

        /// <summary>
        /// c# socket class 
        /// </summary>
        TcpClient m_Client = null;

        /// <summary>
        /// set Connection variable to true for thread loop
        /// </summary>
        bool m_bContinue = true;

        /// <summary>
        /// Connection wait in millisecs 
        /// </summary>
        int m_nTimeOut = 1;

        /// <summary>
        /// event delegate
        /// </summary>
        /// <param name="from"></param>
        /// <param name="args"></param>
        public delegate void EventConnection(object from, ConnectionEventArgs args);

        /// <summary>
        /// connection event 
        /// </summary>
        public event EventConnection ConnectionEvent;

        /// <summary>
        /// network stream to get the read and write stream
        /// </summary>
        NetworkStream m_NetworkStream = null;

        /// <summary>
        /// stream out - write msg to server
        /// </summary>
        StreamWriter m_StreamOut = null;

        /// <summary>
        /// stream to read message from tcp server
        /// </summary>
        StreamReader m_StreamIn = null;

        /// <summary>
        /// handles incomming packets 
        /// </summary>
        ClientReader m_Reader;
        #endregion

        #region Properties
                
        /// <summary>
        /// IPAddress for the tcp server to connect to
        /// </summary>
        string IP
        {
            set { m_sIPAddress = value; }
            get { return m_sIPAddress; }
        }

        /// <summary>
        /// port used by the tcp server to connect to
        /// </summary>
        int Port
        {
            set { m_nPort = value; }
            get { return m_nPort; }
        }       	

        #endregion
    }
}
