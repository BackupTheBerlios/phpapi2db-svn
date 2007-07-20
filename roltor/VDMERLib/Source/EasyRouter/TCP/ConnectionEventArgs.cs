/*
** Client.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** Event Arg class containing tcp connection state information
**
** Changes
** -------
**
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace VDMERLib.EasyRouter.TCP
{
    /// <summary>
    /// Class ConnectionEventArgs
    /// </summary>
    public class ConnectionEventArgs : EventArgs
    {
        /// <summary>
        /// tcp connection state
        /// </summary>
        Client.State m_State;

        /// <summary>
        /// constructor for connection event 
        /// </summary>
        /// <param name="state"></param>
        public ConnectionEventArgs(Client.State state)
        {
            m_State = state; 
        }

        /// <summary>
        /// get the current connectin state
        /// </summary>
        public Client.State ConnectionState
        {
            get { return m_State; }  
        }
    }
}
