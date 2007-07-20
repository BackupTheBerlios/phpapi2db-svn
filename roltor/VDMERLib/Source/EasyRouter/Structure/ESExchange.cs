/*
** ESExchange.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** ESExchange - This represents a logical product exchange such defined by Es or the Exchange 
**
** Changes
** -------
**
*/

using System;
using System.Collections.Generic;
using System.Text;

using System.Collections;
 
namespace VDMERLib.EasyRouter.Structure
{
    /// <summary>
    /// Class ESExchange
    /// </summary>
    public class ESExchange : StructureDataEventArg
    {
        /// <summary>
        /// Link to security exchange this belongs to
        /// </summary>
        private SecurityExchange m_Exchange = null;

        /// <summary>
        /// Link to security exchange this belongs to
        /// </summary>
        public SecurityExchange SecurityExchangeObj { get{return m_Exchange;}}

        /// <summary>
        /// add all commodity to this ESExchange
        /// </summary>
        private Hashtable m_ESCommodityMap = new Hashtable();

        /// <summary>
        /// ES Exchange code 
        /// </summary>
        private  string m_ESExchange;
        
        /// <summary>
        /// ES Exchange code
        /// </summary>
        public string ESExchangeCode { get { return m_ESExchange; } }

        /// <summary>
        /// ES Exchange Description
        /// </summary>
        private string m_sESExchangeDesc;
        
        /// <summary>
        /// ES Exchange Description
        /// </summary>
        public string ESExchangeDesc {  
            get { return m_sESExchangeDesc; }
            set { m_sESExchangeDesc = value; }

        }

        /// <summary>
        /// Have we subscribed 
        /// </summary>
        private bool m_bSubscribed = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sESExchange"></param>
        /// <param name="sExchangeDesc"></param>
        /// <param name="exchange"></param>
        public ESExchange(string sESExchange, string sExchangeDesc, SecurityExchange exchange)
            : base(StructureDataType.ESExchange)  
        {
            m_ESExchange = sESExchange;
            m_sESExchangeDesc = sExchangeDesc;
            m_Exchange = exchange; 
        }

        public void Update(string sESExchange, string sExchangeDesc, SecurityExchange exchange)
        {
            m_ESExchange = sESExchange;
            m_sESExchangeDesc = sExchangeDesc;
            m_Exchange = exchange;
        }

        /// <summary>
        /// Get and set to see if we have subscribed for this ESexchange
        /// </summary>
        public bool Subscribed
        { 
            get{ return m_bSubscribed; }
            set{ m_bSubscribed = value; }
        }

        /// <summary>
        /// add new commodity + commodity symbol to this esexchange
        /// </summary>
        /// <param name="sCommodity"></param>
        /// <param name="sSymbol"></param>
        /// <param name="sDesc"></param>
        /// <returns></returns>
        public CommoditySymbol AddCommodity(string sCommodity,string sSymbol,string sDesc)
        {
            CommoditySymbol commoditysymbol = null;
            if (!m_ESCommodityMap.ContainsKey(sCommodity))
            {
                Commodity commodity = new Commodity(sCommodity, this);
                m_ESCommodityMap[sCommodity] = commodity;
                commoditysymbol = commodity.AddCommoditySymbol(sSymbol, sDesc);
            }
            else
            {
                Commodity commodity = (Commodity)m_ESCommodityMap[sCommodity];
                commoditysymbol = commodity.AddCommoditySymbol(sSymbol, sDesc);
            }
            return commoditysymbol;
        }

        /// <summary>
        /// Find the commodity if exists and return
        /// </summary>
        /// <param name="sCommdity"></param>
        /// <returns></returns>
        public Commodity GetCommodity(string sCommdity)
        {
            Commodity commodity = null;
            if (m_ESCommodityMap.ContainsKey(sCommdity))
            {
                commodity = (Commodity)m_ESCommodityMap[sCommdity];
            }
            else
            {
                commodity = new Commodity(sCommdity, this);
                m_ESCommodityMap[sCommdity] = commodity;
            }
            return commodity;
        }

        /// <summary>
        /// list of all commodity security types for this esexchange
        /// </summary>
        public Hashtable Map
        {
            get { return m_ESCommodityMap; }
        }

        /// <summary>
        /// security exchange for the esexchange
        /// </summary>
        public string Exchange
        {
            get { return m_Exchange.ExchangeCode; } 
        }

    }
}
