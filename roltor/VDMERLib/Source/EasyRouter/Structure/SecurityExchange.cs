/*
** SecurityExchange.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** SecurityExchange - This represents a phyical exchange such as LIFFE,IPE,XETRA,EUREX
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
    /// Class SecurityExchange
    /// </summary>
    public class SecurityExchange : StructureDataEventArg
    {
        /// <summary>
        /// Stores the ESExchange Level objects for this security exchange
        /// </summary>
        private Hashtable m_ESExchangeMap = new Hashtable();

        /// <summary>
        /// Exchange name
        /// </summary>
        private  string m_sExchange;

        /// <summary>
        /// Exchange name
        /// </summary>
        public string ExchangeCode { get { return m_sExchange; } }

        /// <summary>
        /// Exchange Description
        /// </summary>
        private  string m_sExchangeDesc;

        /// <summary>
        /// Exchange Description
        /// </summary>
        public string ExchangeDesc { get { return m_sExchangeDesc; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sExchange"></param>
        /// <param name="sExchangeDesc"></param>
        public SecurityExchange(string sExchange, string sExchangeDesc)
            : base(StructureDataType.SecurityExchange)  
        {
            m_sExchange = sExchange;
            m_sExchangeDesc = sExchangeDesc;
        }

        /// <summary>
        /// add new ESExchange
        /// </summary>
        /// <param name="sESExchange"></param>
        /// <param name="sESExchangeDescription"></param>
        /// <returns></returns>
        public ESExchange AddESExchange(string sESExchange, string sESExchangeDescription)
        {
            ESExchange exchange = null;
            if (!m_ESExchangeMap.ContainsKey(sESExchange))
            {
                exchange = new ESExchange(sESExchange, sESExchangeDescription,this);
                m_ESExchangeMap[sESExchange] = exchange;
            }
            return exchange;
        }

        /// <summary>
        /// get ESexchange
        /// </summary>
        /// <param name="sESExchange"></param>
        /// <returns></returns>
        public ESExchange GetESExchange(string sESExchange)
        {
            ESExchange exchange = null;
            if (m_ESExchangeMap.ContainsKey(sESExchange))
            {
                exchange = (ESExchange)m_ESExchangeMap[sESExchange];
            }
            return exchange;
        }

        /// <summary>
        /// Have we subscribed to this EsExchange for the commodity level
        /// </summary>
        /// <param name="sESExchange"></param>
        /// <returns></returns>
        public bool Subscribed(string sESExchange)
        {
            bool bReturn = false;
            ESExchange exchange = GetESExchange(sESExchange);
            if(exchange != null)
            {
                bReturn = exchange.Subscribed; 
            }
            return bReturn;
        }

        /// <summary>
        /// Have we subscribed at this exexchange-commodity level 
        /// </summary>
        /// <param name="sESExchange"></param>
        /// <param name="sSymbol"></param>
        /// <param name="sSecurityType"></param>
        /// <returns></returns>
        public bool Subscribed(string sESExchange,string sSymbol,string sSecurityType)
        {
            bool bReturn = false;
            ESExchange exchange = GetESExchange(sESExchange);
            if(exchange != null)
            {
                Commodity commodity = exchange.GetCommodity(sSecurityType);
                if (commodity != null)
                {
                    CommoditySymbol commoditysymbol = commodity.GetCommoditySymbol(sSymbol);
                    if (commoditysymbol != null)
                    {
                        bReturn = commoditysymbol.Subscribed;
                    }
                }
            }
            return bReturn;
        }

        /// <summary>
        /// Set subscribed to ESExchange
        /// </summary>
        /// <param name="sESExchange"></param>
        public void SetSubscribed(string sESExchange)
        {
            ESExchange exchange = GetESExchange(sESExchange);
            if (exchange != null)
            {
                exchange.Subscribed = true;
            }
        }

        /// <summary>
        /// Set subscribed to ESExchange-commodity level
        /// </summary>
        /// <param name="sESExchange"></param>
        /// <param name="sSymbol"></param>
        /// <param name="sSecurityType"></param>
        public void SetSubscribed(string sESExchange, string sSymbol, string sSecurityType)
        {
            ESExchange exchange = GetESExchange(sESExchange);
            if (exchange != null)
            {
                Commodity commodity = exchange.GetCommodity(sSecurityType);
                if (commodity != null)
                {
                    CommoditySymbol commoditysymbol = commodity.GetCommoditySymbol(sSymbol);
                    if (commoditysymbol != null)
                    {
                        commoditysymbol.Subscribed = true;
                    }
                }
            }
        }

        /// <summary>
        /// Return the hashmap of ESExchange level objects
        /// </summary>
        public Hashtable Map
        {
            get { return m_ESExchangeMap; }
        }
    }
}
