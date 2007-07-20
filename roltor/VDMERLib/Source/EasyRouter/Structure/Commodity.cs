/*
** Commodity.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** Commodity - This represents a commodity type product such as futures,option
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
    /// Class Commodity
    /// </summary>
    public class Commodity
    {
        /// <summary>
        /// link to the EXExchange this commodity belongs to
        /// </summary>
        private ESExchange m_ESExchange = null;

        /// <summary>
        /// link to the EXExchange this commodity belongs to
        /// </summary>
        public ESExchange ESExchangeObj { get{return m_ESExchange;}}

        /// <summary>
        /// map of commodity tye to commodity symbol
        /// </summary>
        private Hashtable m_ESCommoditySymbolMap = new Hashtable();

        /// <summary>
        /// security type code
        /// </summary>
        private string m_sCommodity;

        /// <summary>
        /// security type code
        /// </summary>
        public string CommodityCode { get { return m_sCommodity; } }
       
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sCommodity"></param>
        /// <param name="exchange"></param>
        public Commodity(string sCommodity,ESExchange exchange)
        {
            m_sCommodity = sCommodity;
            m_ESExchange = exchange;
        }

        /// <summary>
        /// Add commodity symbol
        /// </summary>
        /// <param name="sSymbol"></param>
        /// <param name="sCommodityDesc"></param>
        /// <returns></returns>
        public CommoditySymbol AddCommoditySymbol(string sSymbol, string sCommodityDesc)
        {
            return GetCommoditySymbol(sSymbol,sCommodityDesc, true);
        }

        /// <summary>
        /// get a commodity symbol
        /// </summary>
        /// <param name="sSymbol"></param>
        /// <returns></returns>
        public CommoditySymbol GetCommoditySymbol(string sSymbol)
        {
            return GetCommoditySymbol(sSymbol, sSymbol, false);
        }
        
        /// <summary>
        /// Generic future to get and add Commodity symbol
        /// </summary>
        /// <param name="sSymbol"></param>
        /// <param name="sCommodityDesc"></param>
        /// <param name="bAdded"></param>
        /// <returns></returns>
        protected CommoditySymbol GetCommoditySymbol(string sSymbol,string sCommodityDesc,bool bAdded)
        {
            CommoditySymbol commoditysymbol = null;
            if (m_ESCommoditySymbolMap.ContainsKey(sSymbol))
            {
                commoditysymbol = (CommoditySymbol)m_ESCommoditySymbolMap[sSymbol];
                if (bAdded)
                {
                    commoditysymbol.CommoditySymbolCode = sSymbol;
                    commoditysymbol.CommodityDesc = sCommodityDesc;
                }
                    
            }
            else
            {
                commoditysymbol = new CommoditySymbol(sSymbol, sCommodityDesc,this);
                m_ESCommoditySymbolMap[sSymbol] = commoditysymbol;
            }
            return commoditysymbol;
        }

        /// <summary>
        /// Get the commodity symbol map that is associated to this commodity type
        /// </summary>
        public Hashtable Map
        {
            get { return m_ESCommoditySymbolMap; }
        }
    }
}
