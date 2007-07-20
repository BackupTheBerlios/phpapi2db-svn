/*
** Client.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** Event Arg class containing the FIX Message
**
** Changes
** -------
**
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using MESSAGEFIX3Lib;

namespace VDMERLib.EasyRouter.TCP  
{
    /// <summary>
    /// Class FIXMessageEventArgs
    /// </summary>
    public class FIXMessageEventArgs : EventArgs
    {   
        /// <summary>
        /// Msg FIX Constant 
        /// </summary>
        MESSAGEFIX3Lib.FIXMsgConstants m_MsgType = MESSAGEFIX3Lib.FIXMsgConstants.esFIXMsgUnknown;

        /// <summary>
        /// ES FIX Message
        /// </summary>
        MESSAGEFIX3Lib.FIXMessage m_Msg = null;

        /// <summary>
        /// Event object holding fix data from exchange handler
        /// </summary>
        /// <param name="msg">FIX 4.2 Message</param>
        public FIXMessageEventArgs(MESSAGEFIX3Lib.FIXMessage msg)
        {
            if (msg != null)
            {
                m_Msg = msg;
                m_MsgType = m_Msg.MsgType;
            }
        }

        /// <summary>
        /// destructor
        /// </summary>
        ~FIXMessageEventArgs()
        {
            m_Msg = null;
        }

        /// <summary>
        /// public property accessor for FIX message object
        /// </summary>
        public MESSAGEFIX3Lib.FIXMessage Msg
        {
            get { return m_Msg; }
        }

        /// <summary>
        /// public property accessor for FIX message type for contained FIX message
        /// </summary>
        public MESSAGEFIX3Lib.FIXMsgConstants MsgType
        {
            get { return m_MsgType; }
        }

        /// <summary>
        /// To string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (m_Msg != null)
            {
                return m_MsgType.ToString();
                /*
                string sMessage = (string)m_Msg.Render(ClientReader.Delimiter, MESSAGEFIX3Lib.FIXRenderConstants.esFIXRenderFix42);
                if (sMessage != null)
                {
                    if (sMessage != string.Empty)
                    {
                        return m_MsgType.ToString() + " : " + sMessage;  
                    }
                }*/
            }
            return base.ToString();
        }
    }
}
