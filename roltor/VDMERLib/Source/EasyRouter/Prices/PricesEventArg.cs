/*
** MarketDataEventArg.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** MArket Data base class
**
** Changes
** -------
**
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace VDMERLib.EasyRouter.Prices
{
   

    /// <summary>
    /// PricesEventArg
    /// </summary>
    public class PricesEventArg : Base
    {
        /// <summary>
        /// PriceDataType
        /// </summary>
        public enum PriceDataType
        {
            /// <summary>
            /// tradedata
            /// </summary>
            TradeData,
            /// <summary>
            ///marketmode
            /// </summary>
            MarketMode
        }
        /// <summary>
        /// Price type
        /// </summary>
        protected PriceDataType m_OrderType = PriceDataType.TradeData;

        /// <summary>
        /// Data type
        /// </summary>
        public PriceDataType DataType
        {
            get { return m_OrderType; }
        }
        
        /// <summary>
        /// ES Ticker Mnemonic
        /// </summary>
        protected string m_sSymbol = string.Empty;
        
        /// <summary>
        /// ES Ticker Mnemonic
        /// </summary>
        public string Symbol { get { return m_sSymbol; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public PricesEventArg(PriceDataType type)
        {
            m_OrderType = type;
        }

        /// <summary>
        /// Vanilla
        /// </summary>
        public PricesEventArg()
        { 
        
        }
        

    }
}
