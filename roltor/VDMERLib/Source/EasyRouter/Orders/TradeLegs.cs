/*
** TradeLegs.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** Stores Trade Leg Information for an order
**
** Changes
** -------
**
*/

using System;
using System.Collections.Generic;
using System.Text;

using VDMERLib.EasyRouter.Structure;
using MESSAGEFIX3Lib;
using VDMERLib.EasyRouter.User;

using VDMIFIXMessage = EASYROUTERCOMCLIENTLib.IFIXMessage;

namespace VDMERLib.EasyRouter.Orders
{
    /// <summary>
    /// Class TradeLegs
    /// </summary>
    public class TradeLegs
    {
        /// <summary>
        /// ES Ticker Mnemonic
        /// </summary>
        string m_sUnderlyingTickerMnemonic = string.Empty;

        /// <summary>
        /// Get  ES Ticker Mnemonic Property
        /// </summary>
        public string UnderlyingTickerMnemonic 
        {
            get { return m_sUnderlyingTickerMnemonic; }
            set { m_sUnderlyingTickerMnemonic = value; } 
        }

        /// <summary>
        /// Vanilla
        /// </summary>
        public TradeLegs()
        { 
        
        }

        /// <summary>
        /// Trade ExecID
        /// </summary>
        string m_sExecID = string.Empty;

        /// <summary>
        /// Get Trade Exec ID
        /// /// </summary>
        public string ExecID
        {
            get { return m_sExecID; }
            set { m_sExecID = value; }
        }

        /// <summary>
        /// Trade Quantity
        /// </summary>
        int m_nExecQty = 0;

        /// <summary>
        /// Trade Quantity
        /// </summary>
        public int ExecQty
        {
            get { return m_nExecQty; }
            set { m_nExecQty = value; }
        }

        /// <summary>
        /// Trade Price
        /// </summary>
        double m_dTradePrice = 0;

        /// <summary>
        /// Trade Price
        /// </summary>
        public double TradePrice
        {
            get { return m_dTradePrice; }
            set { m_dTradePrice = value; }
        }

        /// <summary>
        /// Side of order as a string represneted by MESSAGEFIX3Lib.FIXSideConstants
        /// </summary>
        public string m_sSide = MESSAGEFIX3Lib.FIXSideConstants.esFIXSideUnknown;

        /// <summary>
        /// Side of order as a string represneted by MESSAGEFIX3Lib.FIXSideConstants
        /// </summary>
        public string Side
        {
            set { m_sSide = value; }
        }

        /// <summary>
        /// Usage, only if the instrument supports buy or sell
        /// </summary>
        public bool IsBuy
        {
            get { return m_sSide == MESSAGEFIX3Lib.FIXSideConstants.esFIXSideBuy; }
        }

        /// <summary>
        /// Usage, only if the instrument supports buy or sell
        /// </summary>
        public bool IsSell
        {
            get { return m_sSide == MESSAGEFIX3Lib.FIXSideConstants.esFIXSideSell; }
        }

        /// <summary>
        /// Leg number
        /// </summary>
        int m_nLegNumber = 0;

        /// <summary>
        /// Leg number
        /// </summary>
        public int LegNumber
        {
            get { return m_nLegNumber; }
            set { m_nLegNumber = value; }
        }

    }
}
