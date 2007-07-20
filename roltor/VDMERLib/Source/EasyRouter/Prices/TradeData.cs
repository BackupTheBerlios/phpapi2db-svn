/*
** TradeData.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** TradeData 
**
** Changes
** -------
**
*/

using System;
using System.Collections.Generic;
using System.Text;

using EASYROUTERCOMCLIENTLib;
using System.Windows.Forms;

namespace VDMERLib.EasyRouter.Prices
{
    /// <summary>
    /// Class TradeData
    /// </summary>
    public class TradeData : PricesEventArg
    { 
        /// <summary>
        /// Price Type (Implied or Actual)
        /// </summary>
        /// 
        public enum Implied
        {
            Actual = 0, 
            Exchange = 1,
            Local = 2,
            BestFromActual = 3,
            BestFromActualAndImplied = 4,
            BestFromImplied = 5,
            BestImpliedNoActual = 6,
            Undefined = 7
        }
        /// <summary>
        /// Ask Price Type
        /// </summary>
        /// 
        private Implied m_eAskOrderType;

        public Implied AskOrderType
        {
            get { return m_eAskOrderType; }
        }

        /// <summary>
        /// Bid Price Type
        /// </summary>
        /// 
        private Implied m_eBidOrderType;

        public Implied BidOrderType
        {
            get { return m_eBidOrderType; }
        }
        
        /// <summary>
        /// bid
        /// </summary>
        PriceVolume m_Bid;

        public PriceVolume Bid { get { return m_Bid; } }
        
        /// <summary>
        /// ask
        /// </summary>
        PriceVolume m_Ask;

        public PriceVolume Ask { get { return m_Ask; } }
        
        /// <summary>
        /// Trade
        /// </summary>
        PriceVolume m_Trade;
        public PriceVolume Trade { get { return m_Trade; } }
        
        /// <summary>
        /// Close
        /// </summary>
        private double? m_dClose;
        
        /// <summary>
        /// Close
        /// </summary>
        public double? Close { get { return m_dClose; } }
        /// <summary>
        /// Settlement
        /// </summary>
        private double? m_dSettlement;
        /// <summary>
        /// Settlement
        /// </summary>
        public double? Settlement { get { return m_dSettlement; } }
        /// <summary>
        /// Settlement time
        /// </summary>
        private string m_sSettlementTime = string.Empty;
        /// <summary>
        /// Settlement time
        /// </summary>
        public string SettlementTime { get { return m_sSettlementTime; } }
        /// <summary>
        /// Settlement date
        /// </summary>
        private string m_sSettlementDate = string.Empty;
        /// <summary>
        /// Settlement date
        /// </summary>
        public string SettlementDate { get { return m_sSettlementDate; } }
        /// <summary>
        /// Prev Settlement
        /// </summary>
        private double? m_dPrevSettlement;
        /// <summary>
        /// Prev Settlement
        /// </summary>
        public double? PrevSettlement { get { return m_dPrevSettlement; } }
        /// <summary>
        /// Prev Settlement time
        /// </summary>
        private string m_sPrevSettlementTime = string.Empty;
        /// <summary>
        /// Prev Settlement time
        /// </summary>
        public string PrevSettlementTime { get { return m_sPrevSettlementTime; } }
        /// <summary>
        /// Prev Settlement date
        /// </summary>
        private string m_sPrevSettlementDate = string.Empty;
        /// <summary>
        /// Prev Settlement date
        /// </summary>
        public string PrevSettlementDate { get { return m_sPrevSettlementDate; } }
        /// <summary>
        /// Total traded volume
        /// </summary>
        private int? m_nTotalTradeVolume;
        /// <summary>
        /// Total traded volume
        /// </summary>
        public int? TotalTradeVolume { get { return m_nTotalTradeVolume; } }

        private double? m_SessionHigh;

        public double? SessionHigh
        {
            get { return m_SessionHigh; }
            set { m_SessionHigh = value; }
        }

        private double? m_SessionLow;

        public double? SessionLow
        {
            get { return m_SessionLow; }
            set { m_SessionLow = value; }
        }
        
        /// <summary>
        /// Indicates if is a snapshot
        /// </summary>
        bool m_bSnapshot = false;

        /// <summary>
        /// public Property to get snapshot
        /// </summary>
        public bool SnapShot
        {
            get { return m_bSnapshot; }
            set { m_bSnapshot = value; }
        }
               
        /// <summary>
        /// Vanilla
        /// </summary>
        public TradeData()
        { 

        }

        public void Update(TradeData priceUpdate)
        {
            if (priceUpdate.Bid != null && priceUpdate.Bid.HasChanged)
            {
                m_Bid.Price = priceUpdate.Bid.Price;
                m_Bid.Volume = priceUpdate.Bid.Volume;
            }

            if (priceUpdate.Ask != null && priceUpdate.Ask.HasChanged)
            {
                m_Ask.Price = priceUpdate.Ask.Price;
                m_Ask.Volume = priceUpdate.Ask.Volume;
            }

            if (priceUpdate.Trade != null && priceUpdate.Trade.HasChanged)
            {
                m_Trade.Price = priceUpdate.Trade.Price;
                m_Trade.Volume = priceUpdate.Trade.Volume;
            }

            if (priceUpdate.Close.HasValue) 
                m_dClose = priceUpdate.Close;

            if (priceUpdate.Settlement.HasValue)
            {
                m_dSettlement = priceUpdate.Settlement;
                m_sSettlementTime = priceUpdate.SettlementTime;
                m_sSettlementDate = priceUpdate.SettlementDate;
            }

            if (priceUpdate.PrevSettlement.HasValue)
            {
                m_dPrevSettlement = priceUpdate.PrevSettlement;
                m_sPrevSettlementTime = priceUpdate.PrevSettlementTime;
                m_sPrevSettlementDate = priceUpdate.PrevSettlementDate;
            }

            if (priceUpdate.TotalTradeVolume.HasValue)
                m_nTotalTradeVolume = priceUpdate.TotalTradeVolume;

            if (priceUpdate.SessionHigh.HasValue)
                m_SessionHigh = priceUpdate.SessionHigh;

            if (priceUpdate.SessionLow.HasValue)
                m_SessionLow = priceUpdate.SessionLow;

            IsDirty = true;
        }

        private void SetPrice(out PriceVolume thePrice, IFIXGroup singleGroup)
        {
            int iVolume = singleGroup.get_AsNumber(FIXTagConstants.esFIXTagMDEntrySize);
            if (iVolume == 0)
            {
                thePrice = new PriceVolume(null, null);
            }
            else
                thePrice = new PriceVolume(iVolume, singleGroup.get_AsDouble(FIXTagConstants.esFIXTagMDEntryPx));            
        }

        /// <summary>
        /// Decode price FIX message
        /// </summary>
        /// <param name="FIXMsg"></param>
        /// <param name="bSnapFull"></param>
        /// <returns></returns>
        public bool DecodeFIX(IFIXMessage FIXMsg,bool bSnapFull)
        {
            bool bReturn = false;
            m_sSymbol = FIXMsg.get_AsString(FIXTagConstants.esFIXTagESTickerMnemonic);

            int nTotal = 0;
            if (FIXMsg.GetNumber(out nTotal, FIXTagConstants.esFIXTagTotalVolumeTraded))
                m_nTotalTradeVolume = nTotal;
            
            IFIXGroup group = FIXMsg.GetGroupByTag(FIXTagConstants.esFIXTagNoMDEntries, null);

            if (group != null)
            {
                int nCount = group.get_NumberOfGroups(null);

                for (int i = 0; i < nCount; i++)
                {
                    IFIXGroup singleGroup = group.GetGroupByIndex(i);

                    string sType = singleGroup.get_AsString(FIXTagConstants.esFIXTagMDEntryType);

                    int nImplied = singleGroup.get_AsNumber(FIXTagConstants.esFIXTagESMDEntryIsImplied);
                    string sTime = singleGroup.get_AsString(FIXTagConstants.esFIXTagMDEntryTime);

                    bReturn = true;
                                        
                    switch (sType)
                    {
                        case MESSAGEFIX3Lib.FIXMarketDataTypeConstants.esFIXMarketDataTypeBestAsk:
                            {
                                switch (nImplied)
                                {
                                    case (int)MESSAGEFIX3Lib.FIXESIsImpliedConstants.esFIXESIsImpliedBestFromActual:
                                    case (int)MESSAGEFIX3Lib.FIXESIsImpliedConstants.esFIXESIsImpliedBestFromActualAndImplied:
                                    case (int)MESSAGEFIX3Lib.FIXESIsImpliedConstants.esFIXESIsImpliedBestFromImplied:
                                    case (int)MESSAGEFIX3Lib.FIXESIsImpliedConstants.esFIXESIsImpliedBestImpliedNoActual:
                                        m_eAskOrderType = (Implied)nImplied;
                                        SetPrice(out m_Ask, singleGroup);
                                        break;
                                }
                                break;
                            }
                        case MESSAGEFIX3Lib.FIXMarketDataTypeConstants.esFIXMarketDataTypeBestBid:
                            {
                                switch (nImplied)
                                {
                                    case (int)MESSAGEFIX3Lib.FIXESIsImpliedConstants.esFIXESIsImpliedBestFromActual:
                                    case (int)MESSAGEFIX3Lib.FIXESIsImpliedConstants.esFIXESIsImpliedBestFromActualAndImplied:
                                    case (int)MESSAGEFIX3Lib.FIXESIsImpliedConstants.esFIXESIsImpliedBestFromImplied:
                                    case (int)MESSAGEFIX3Lib.FIXESIsImpliedConstants.esFIXESIsImpliedBestImpliedNoActual:
                                        m_eBidOrderType = (Implied)nImplied;
                                        SetPrice(out m_Bid, singleGroup);
                                        break;
                                }
                                break;
                                break;
                            }

                        case MESSAGEFIX3Lib.FIXMarketDataTypeConstants.esFIXMarketDataTypeSettlementPrice:
                            {
                                int nType = singleGroup.get_AsNumber(FIXTagConstants.esFIXTagOpenCloseSettlementFlag);

                                MESSAGEFIX3Lib.FIXSettlementFlagConstants type = (MESSAGEFIX3Lib.FIXSettlementFlagConstants)nType;

                                double dPrice = singleGroup.get_AsDouble(FIXTagConstants.esFIXTagMDEntryPx);
                                if (type == MESSAGEFIX3Lib.FIXSettlementFlagConstants.esFIXSettlementOpenCloseYesterday)
                                {
                                    m_dSettlement = dPrice;
                                    m_sSettlementTime = singleGroup.get_AsString(FIXTagConstants.esFIXTagMDEntryTime);
                                    m_sSettlementDate = singleGroup.get_AsString(FIXTagConstants.esFIXTagMDEntryDate);
                                }
                                else
                                {
                                    m_dPrevSettlement = dPrice;
                                    m_sPrevSettlementTime = singleGroup.get_AsString(FIXTagConstants.esFIXTagMDEntryTime);
                                    m_sPrevSettlementDate = singleGroup.get_AsString(FIXTagConstants.esFIXTagMDEntryDate);
                                }
                                break;
                            }
                        case MESSAGEFIX3Lib.FIXMarketDataTypeConstants.esFIXMarketDataTypeClosingPrice:
                            {
                                m_dClose = singleGroup.get_AsDouble(FIXTagConstants.esFIXTagMDEntryPx);
                                break;
                            }
                        case MESSAGEFIX3Lib.FIXMarketDataTypeConstants.esFIXMarketDataTypeTrade:
                            {
                                SetPrice(out m_Trade, singleGroup); 
                                break;
                            }
                        case MESSAGEFIX3Lib.FIXMarketDataTypeConstants.esFIXMarketDataTypeSessionHighPrice:
                            {
                                m_SessionHigh = singleGroup.get_AsDouble(FIXTagConstants.esFIXTagMDEntryPx);
                                break;
                            }
                        case MESSAGEFIX3Lib.FIXMarketDataTypeConstants.esFIXMarketDataTypeSessionLowPrice:
                            {
                                m_SessionLow = singleGroup.get_AsDouble(FIXTagConstants.esFIXTagMDEntryPx);
                                break;
                            }
                    }
                }
            }
            m_bSnapshot = bSnapFull;
            return bReturn;
        }

        public class PriceVolume
        {
            private int? m_Volume = null;

            public int? Volume
            {
                get { return m_Volume; }
                set { m_Volume = value; m_HasChanged = true; }
            }

            private double? m_Price = null;

            public double? Price
            {
                get { return m_Price; }
                set { m_Price = value; m_HasChanged = true; }
            }
            private bool m_HasChanged = false;

            public bool HasChanged
            {
                get { return m_HasChanged; }
            }
            
            public PriceVolume(int? volume, double? price)
            {
                m_Volume = volume;
                m_Price = price;
                m_HasChanged = true;
            }
            
        }
       
    }
}
