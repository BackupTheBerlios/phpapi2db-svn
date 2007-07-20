/*
** Trade.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** Stores Trade Information for an order and any ouright legs if it is a stategy
**
** Changes
** -------
**
*/

using System;
using System.Collections.Generic;
using System.Text;

using VDMIFIXMessage = EASYROUTERCOMCLIENTLib.IFIXMessage;
using VDMERLib.EasyRouter.Structure;

using System.Collections;

namespace VDMERLib.EasyRouter.Orders
{
    /// <summary>
    /// Class TradeInfo
    /// </summary>
    public class TradeInfo : OrderInfo
    {
        /// <summary>
        /// store trade legs of any
        /// </summary>
        ArrayList m_Legs = new ArrayList();

        /// <summary>
        /// Trade ExecID
        /// </summary>
        string m_sExecID;

        /// <summary>
        /// Trade ExecID
        /// </summary>
        public string ExecID
        {
            get { return m_sExecID; }
            set { m_sExecID = value; }
        }

        /// <summary>
        /// Trade Quantity
        /// </summary>
        int m_nExecQty;

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
        /// constructor
        /// </summary>
        /// <param name="sTickerMnemonic"></param>
        public TradeInfo(string sTickerMnemonic)
            : base(sTickerMnemonic)
        {

        }

        /// <summary>
        /// Indicates if outright legs are available
        /// </summary>
        /// <returns></returns>
        public bool HasLegs()
        {
            if (m_Legs.Count > 0)
                return true;
            return false;
        }

        /// <summary>
        /// Leg Count
        /// </summary>
        /// <returns>Number of trade legs for this trade</returns>
        public int LegCount()
        {
            return m_Legs.Count;
        }

        /// <summary>
        /// Get TradeLeg by index
        /// </summary>
        public TradeLegs GetLeg(int nIDX)
        {
            if (nIDX >= m_Legs.Count)
                return null;
            else
                return (TradeLegs)m_Legs[nIDX];
        }

        /// <summary>
        /// Parse FIX msg to obtain trade information
        /// </summary>
        /// <param name="FIXMsg"></param>
        /// <returns></returns>
        public override bool ParseFix(VDMIFIXMessage FIXMsg)
        {
            bool bSuccess = true;

            string sExecID = string.Empty;
            if (FIXMsg.GetString(out sExecID, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagExecID))
                m_sExecID = sExecID;

            int nExecQty = 0;
            if (FIXMsg.GetNumber(out nExecQty, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagLastShares))
                m_nExecQty = nExecQty;


            EASYROUTERCOMCLIENTLib.IFIXGroup defaultGroup = FIXMsg.GetGroupByTag(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESNoUnderlyingFills, null);

            if (defaultGroup != null)
            {
                int nDefaultCount = defaultGroup.get_NumberOfGroups(null);

                if (nDefaultCount == 1)
                {
                    //outright
                    //do nothing
                }
                else
                {
                    for (int j = 0; j < nDefaultCount; j++)
                    {
                        TradeLegs leg = new TradeLegs();

                        string sESUnderlyingTickerMnemonic = string.Empty;
                        if (FIXMsg.GetString(out sESUnderlyingTickerMnemonic, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESUnderlyingTickerMnemonic))
                            leg.UnderlyingTickerMnemonic = sESUnderlyingTickerMnemonic;

                        string sUnderlyingExecID = string.Empty;
                        if (FIXMsg.GetString(out sUnderlyingExecID, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESUnderlyingExecID))
                            leg.ExecID = sExecID;

                        int nUnderlyingExecQty = 0;
                        if (FIXMsg.GetNumber(out nUnderlyingExecQty, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESUnderlyingLastShares))
                            leg.ExecQty = nUnderlyingExecQty;

                        int nLegNumber = 0;
                        if (FIXMsg.GetNumber(out nLegNumber, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESUnderlyingLegNumber))
                            leg.LegNumber = nLegNumber;

                        double dPrice = 0;
                        if (FIXMsg.GetDouble(out dPrice, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESUnderlyingLastPx))
                            m_dTradePrice = dPrice;

                        string sSide = string.Empty;
                        if (FIXMsg.GetString(out sSide, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESUnderlyingSide))
                            leg.Side = sExecID;

                        m_Legs.Add(leg);
                    }
                }
            }



            bSuccess = base.ParseFix(FIXMsg);

            return bSuccess;
        }

        /// <summary>
        /// override base order price as this is a trade price
        /// </summary>
        /// <param name="FIXMsg"></param>
        public override void SetPrice(VDMIFIXMessage FIXMsg)
        {
            double dPrice = 0;
            if (FIXMsg.GetDouble(out dPrice, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagPrice))
            {
                m_dTradePrice = dPrice;
                m_dTotalFilledPrice = dPrice * m_nExecQty;
            }
            m_dPrice = null;
        }

       

        /// <summary>
        /// Get a string summary of this trade object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("TYPE[{0:10}] PBOID[{1:10}]SBOID[{2:10}] TE[{3:20}] TRADE[{4}] TRADEQTY[{5}] EXECID[{6}] NOLEGS[{0}]",GetExecType(m_sExecutionReportStatus), m_nPrimaryBOID, m_nSecondBOID, m_sTickerMnemonic , m_dTradePrice,m_nExecQty,m_sExecID,m_Legs.Count);
        }
    }
}
