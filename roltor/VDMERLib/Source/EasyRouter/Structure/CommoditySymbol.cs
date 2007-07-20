/*
** CommoditySymbol.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** CommoditySymbol - This represents a commodity symbol for the commodity
**
** Changes
** -------
**
*/

using System;
using System.Collections.Generic;
using System.Text;

using EASYROUTERCOMCLIENTLib;
using System.Collections;
 
namespace VDMERLib.EasyRouter.Structure
{
    /// <summary>
    /// Class CommoditySymbol
    /// </summary>
    public class CommoditySymbol : StructureDataEventArg
    {
        /// <summary>
        /// Commodity linked to this obejct
        /// </summary>
        private Commodity m_Commodity = null;

        /// <summary>
        /// Get Commodity linked to this obejct
        /// </summary>
        public Commodity CommodityObj { get{return m_Commodity;}}

        /// <summary>
        /// maps commodity symbol to all TE entity for this security type
        /// </summary>
        private Hashtable m_ESTEMap = new Hashtable();

        /// <summary>
        /// Commodity Symbol
        /// </summary>
        string m_sSymbol;

        /// <summary>
        /// Commodity symbol
        /// </summary>
        public string CommoditySymbolCode { get { return m_sSymbol; } }

        /// <summary>
        /// Commodity Symbol Desc
        /// </summary>
        string m_sCommodityDesc;

        /// <summary>
        /// Commodity symbol
        /// </summary>
        public string CommodityDesc { get { return m_sCommodityDesc; } }


        /// <summary>
        /// Is subscribed
        /// </summary>
        private bool m_bSubscribed = false;

        /// <summary>
        /// Accssor for subscription
        /// </summary>
        public bool Subscribed
        {
            get { return m_bSubscribed; }
            set { m_bSubscribed = value; }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="sSymbol"></param>
        /// <param name="sCommodityDesc"></param>
        /// <param name="commodity"></param>
        public CommoditySymbol(string sSymbol, string sCommodityDesc, Commodity commodity)
            : base(StructureDataType.Commodity)  
        {
            m_sSymbol = sSymbol;
            m_Commodity = commodity;
            m_sCommodityDesc = sCommodityDesc;
        }

        /// <summary>
        /// get TE instrument
        /// </summary>
        /// <param name="sSymbol"></param>
        /// <returns></returns>
        public TEInstrument GetTEInstrument(string sSymbol)
        {
            TEInstrument instrument = null;
            if (!m_ESTEMap.ContainsKey(sSymbol))
            {
                instrument = (TEInstrument)m_ESTEMap[sSymbol];
            }
            return instrument;
        }

        /// <summary>
        ///  Add new TE if it does not exsit
        /// </summary>
        /// <param name="sESTickerMnemonic"></param>
        /// <param name="FIXMsg"></param>
        /// <returns></returns>
        public TEInstrument AddTEInstrument(string sESTickerMnemonic,IFIXMessage FIXMsg)
        {
            TEInstrument instrument = null;

            if (!m_ESTEMap.ContainsKey(sESTickerMnemonic))
            {
                instrument = new TEInstrument(this);
                if(instrument.ParseFIX(FIXMsg))
                    m_ESTEMap[sESTickerMnemonic] = instrument;
            }
            else
            {
                instrument = (TEInstrument)m_ESTEMap[sESTickerMnemonic];
                //try mimimal update
                instrument.AdditonalUpdates(FIXMsg);
            }
            return instrument;
        }

        /// <summary>
        /// Get the TE map
        /// </summary>
        public Hashtable Map
        {
            get { return m_ESTEMap; }
        }

        /// <summary>
        /// security exchange for the esexchange
        /// </summary>
        public string Exchange
        {
            get { return m_Commodity.ESExchangeObj.Exchange; }
        }

        /// <summary>
        /// security exchange for the esexchange
        /// </summary>
        public string ESExchange
        {
            get { return m_Commodity.ESExchangeObj.ESExchangeCode; }
        }

        /// <summary>
        /// To string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Exchange + ESExchange + m_sSymbol + m_Commodity.CommodityCode;
        }
    }
}
