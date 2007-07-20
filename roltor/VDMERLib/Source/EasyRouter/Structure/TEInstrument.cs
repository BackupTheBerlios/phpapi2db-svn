

using System;
using System.Collections.Generic;
using System.Text;

using MESSAGEFIX3Lib;
using EASYROUTERCOMCLIENTLib;
using System.Collections;
using VDMERLib.EasyRouter;

namespace VDMERLib.EasyRouter.Structure
{
    /// <summary>
    /// Class Easyscreen TEInstrument 
    /// </summary>
    public class TEInstrument : StructureDataEventArg 
    {
        /// <summary>
        /// 
        /// </summary>
        VDMERLib.EasyRouter.Prices.SecurityStatus m_SecurityStatus = null;

        /// <summary>
        /// Get "R" - additional info
        /// </summary>
        private bool m_bSubscribedAdditionalInfo = false;

        /// <summary>
        /// 
        /// </summary>
        public VDMERLib.EasyRouter.Prices.SecurityStatus SecurityStatus
        {
            set
            {
                m_SecurityStatus = value;
            }
            get
            {
                return m_SecurityStatus;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ArrayList Status
        {
            get 
            {
                if (m_SecurityStatus != null)
                    return m_SecurityStatus.Status;
                return null;
            }
        }

        /// <summary>
        /// commodity symbol object
        /// </summary>
        private CommoditySymbol m_CommoditySymbol = null;

        /// <summary>
        /// get commodity symbol object
        /// </summary>
        public CommoditySymbol CommoditySymbolObj { get {return m_CommoditySymbol;}}

        /// <summary>
        /// Exchange symbol
        /// </summary>
        private string m_sSymbol = string.Empty;

        /// <summary>
        /// Exchange symbol
        /// </summary>
        public string Symbol { get { return m_sSymbol; } }

        /// <summary>
        /// ?
        /// </summary>
        private string m_sESTickerSymbol = string.Empty;
        
        /// <summary>
        /// ?
        /// </summary>
        public string TickerSymbol { get { return m_sESTickerSymbol; } }

        /// <summary>
        /// Used for reuter code for exchange when used with the ISIN (securityID)
        /// </summary>
        public string m_sMDExchange = string.Empty;

        /// <summary>
        /// Used for equity to descibe m_sMDExchange and securityID , eg isin sedol etc
        /// </summary>
        public string m_sIDSource = string.Empty; 

        /// <summary>
        /// FIXTimeType
        /// </summary>
        private TimeInForce m_eTimeType = 0;

        /// <summary>
        /// FIXTimeType 
        /// </summary>
        /// 
        public TimeInForce SupportedTimeTypes
        {
            get { return m_eTimeType; }
        }

        /// <summary>
        /// FIXOrderType 
        /// </summary>
        private OrderType m_eOrderType = 0;

        /// <summary>
        /// FIXOrderType 
        /// </summary>

        public OrderType SupportedOrderTypes
        {
            get { return m_eOrderType; }
        }

        /// <summary>
        /// ?
        /// </summary>
        private bool m_bESSupportEdit = false;

        /// <summary>
        /// ?
        /// </summary>
        private int m_lESPriceFormatCode = 0;

        public int PriceFormatCode
        {
            get { return m_lESPriceFormatCode; }
        }
        
        /// <summary>
        /// Easyscreen internal symbol
        /// </summary>
        private string m_sESTickerMnemonic = string.Empty;

        /// <summary>
        /// Easyscreen internal symbol
        /// </summary>
        public string ESTickerMnemonic { get { return m_sESTickerMnemonic; } }

        /// <summary>
        /// Easyscreen symbol description
        /// </summary>
        public string m_sESTickerDesc = string.Empty;

        /// <summary>
        /// Easyscreen symbol description
        /// </summary>
        public string ESTickerDescription { get { return m_sESTickerDesc; } }

        /// <summary>
        /// Option type - defaults to Call
        /// </summary>
        int m_nOptionType = 1;  

        /// <summary>
        /// 
        /// </summary>
        public string SecurityIDSource { get { return m_sIDSource; } }

        /// <summary>
        /// Exchange 
        /// </summary>
        public string m_sSecurityID = string.Empty;

        /// <summary>
        /// Statagy code
        /// </summary>
        public string m_sStrategyCode = string.Empty;

        public bool IsStrategy { get {return m_sStrategyCode.Length > 0; } }

        /// <summary>
        /// Risk Combined commodity code
        /// </summary>
        public string m_sCombinedCommodityCode = string.Empty;

        /// <summary>
        /// Clearing house
        /// </summary>
        public string m_sClearingHouse = string.Empty;

        /// <summary>
        /// CFI Code
        /// </summary>
        private string m_sCFICode = string.Empty;

        /// <summary>
        /// Currency
        /// </summary>
        private string m_sCurrency = string.Empty;

        /// <summary>
        /// unit tick value
        /// </summary>
        private double m_dUnitTickValue = 0.0;

        /// <summary>
        /// Get unit tick value
        /// </summary>
        public double UnitTick
        {
            get { return m_dUnitTickValue; }
        }

        private double m_dCompositeDelta = 0.0;
        private double m_dESShortOptAdjust = 0.0;
        private int m_nESDecimalPlaces = 0;

        public int DecimalPlaces
        {
            get { return m_nESDecimalPlaces; }
        }

        /// <summary>
        /// Price movement for ofcourse the price
        /// </summary>
        private double m_dPriceMovement = 0.0;
        
        /// <summary>
        /// accessor
        /// </summary>
        public double PriceMovement
        {
            get { return m_dPriceMovement; }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commoditySymbol"></param>
        public TEInstrument(CommoditySymbol commoditySymbol)
            : base(StructureDataType.TEInstrument)  
        {
            m_CommoditySymbol = commoditySymbol;
        }

        /// <summary>
        /// Parse msg for instrument data
        /// </summary>
        /// <param name="FIXMsg"></param>
        /// <returns></returns>
        public bool ParseFIX(EASYROUTERCOMCLIENTLib.IFIXMessage FIXMsg)
        {
            try
            {
                //exchaange
                string sExchange = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagSecurityExchange);
                //Esexchange
                string sESExchange = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESExchange);
                //type of instrument
                string sSecurityType = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagSecurityType);
                //commodity symbol
                string sSymbol = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagSymbol);
                //easyscreen TE symbol
                string sESTickerMnemonic = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESTickerMnemonic);
             
                string sESTickerDesc = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESTickerDesc);
                //exchange code
                string sTickerSymbol = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESTickerSymbol);
                int nESOrderType = FIXMsg.get_AsNumber(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESSupportPriceType);
                int nESTimeType = FIXMsg.get_AsNumber(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESSupportTimeType);
                bool bESSupportEdit = FIXMsg.get_AsBoolean(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESSupportEdit);
                int lESPriceFormatCode = FIXMsg.get_AsNumber(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESPriceFormatCode);

                m_sSymbol = sSymbol;
                m_sESTickerMnemonic = sESTickerMnemonic;
                m_sESTickerDesc = sESTickerDesc;
                m_sESTickerSymbol = sTickerSymbol;
                m_eOrderType = (OrderType)nESOrderType;
                m_eTimeType = (TimeInForce)nESTimeType;
                m_bESSupportEdit = bESSupportEdit;
                m_lESPriceFormatCode = lESPriceFormatCode;

                int nOptionType = 0;
                if(FIXMsg.GetNumber(out nOptionType,EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagPutOrCall))
                {
                    //defaults to 1 - call if never here
                    m_nOptionType = nOptionType; 
                }

                m_sCFICode = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagCFICode);

                m_sCurrency = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagCurrency);
                m_dUnitTickValue = FIXMsg.get_AsDouble(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESUnitTickValue);
                m_dCompositeDelta = FIXMsg.get_AsDouble(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESCompositeDelta);
                m_dESShortOptAdjust = FIXMsg.get_AsDouble(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESShortOptAdjust);  
                m_nESDecimalPlaces = FIXMsg.get_AsNumber(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESDecimalPlaces); 
                
                AdditonalUpdates(FIXMsg);
            }
            catch(Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// get security exchange code
        /// </summary>
        public string SecurityExchange
        {
            get
            {
                return CommoditySymbolObj.CommodityObj.ESExchangeObj.SecurityExchangeObj.ExchangeCode;
            }
        }

        /// <summary>
        /// Get esexchange code
        /// </summary>
        public string ESExchange
        {
            get
            {
                return CommoditySymbolObj.CommodityObj.ESExchangeObj.ESExchangeCode; 
            }
        }

        /// <summary>
        /// Get commodity symbol
        /// </summary>
        public string Commodity
        {
            get
            {
                return CommoditySymbolObj.CommoditySymbolCode; 
            }
        }

        /// <summary>
        /// Get commodity type
        /// </summary>
        public string CommodityType
        {
            get
            {
                return CommoditySymbolObj.CommodityObj.CommodityCode;   
            }
        }

        /// <summary>
        /// Apply order details
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool ApplyOrderDetails(FIXMessage message)
        {
            bool bRetVal = true;
            
            //add details

            message.set_AsString(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagESTickerMnemonic,m_sESTickerMnemonic);
            message.set_AsString(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagESTickerSymbol,m_sESTickerSymbol);
            message.set_AsString(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagESExchange, ESExchange);
            message.set_AsNumber(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagESPriceFormatCode,m_lESPriceFormatCode);
            message.set_AsString(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagSecurityExchange, SecurityExchange);
            message.set_AsNumber(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagPutOrCall, m_nOptionType);
            message.set_AsString(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagSecurityType, CommodityType);
            message.set_AsString(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagSymbol,m_sSymbol);
            message.set_AsString(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagESStrategyCode,m_sStrategyCode);
            message.set_AsString(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagSecurityID,m_sSecurityID);
            message.set_AsString(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagIDSource,m_sIDSource);
            message.set_AsString(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagCFICode,m_sCFICode);
            message.set_AsString(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagCurrency,m_sCurrency);
            message.set_AsNumber(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagESDecimalPlaces,m_nESDecimalPlaces);
            message.set_AsDouble(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagESUnitTickValue,m_dUnitTickValue);
            message.set_AsString(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagESCombinedCommodityCode, m_sCombinedCommodityCode);
            message.set_AsString(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagESClearingHouse, m_sClearingHouse);
            message.set_AsDouble(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagESCompositeDelta, m_dCompositeDelta);
            message.set_AsDouble(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagESShortOptAdjust, m_dESShortOptAdjust);
         
            ApplyRiskArray(message);

            return bRetVal;
        }

        /// <summary>
        /// Additional instrument data
        /// </summary>
        /// <param name="FIXMsg"></param>
        public void AdditonalUpdates(EASYROUTERCOMCLIENTLib.IFIXMessage FIXMsg)
        {
            EASYROUTERCOMCLIENTLib.IFIXGroup defaultGroup = FIXMsg.GetGroupByTag(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagNoRelatedSym, null);

            string sSecurityID = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagSecurityID);
            if (sSecurityID != string.Empty)
                m_sSecurityID = sSecurityID;

            string sIDSource = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagIDSource);
            if (sSecurityID != string.Empty)
                m_sSecurityID = sSecurityID;

            string sMDExchange = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESMarketDataSecurityExchange);
            if (sMDExchange != string.Empty)
                m_sMDExchange = sMDExchange;

            if (defaultGroup != null)
            {
                int nDefaultCount = defaultGroup.get_NumberOfGroups(null);

                int nEarliestYearMonth = 0;
                string sUnderlyingMnemonic = string.Empty;
                string sUnderlyingCFICode = string.Empty;
                string sUnderlyingSymbol = string.Empty;
                string sUnderlyingCurrency = string.Empty;
                string sUnderlyingOppCurrency = string.Empty;
                double dUnderlyingUnitTickValue = 0.0;
                int nYearMonth = 0;
                int nOptionType = -1;
              

                for (int j = 0; j < nDefaultCount; j++)
                {
                    EASYROUTERCOMCLIENTLib.IFIXGroup singleDefaultGroup = defaultGroup.GetGroupByIndex(j);


                    if (singleDefaultGroup.HasGroup(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESUnderlyingExpiryYearMonth))
                    {
                        string sYearMonth = singleDefaultGroup.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESUnderlyingExpiryYearMonth);
                        nYearMonth = int.Parse(sYearMonth);

                        if ((nEarliestYearMonth == 0) || (nYearMonth < nEarliestYearMonth))
                        {
                            nEarliestYearMonth = nYearMonth;
                            sUnderlyingMnemonic = singleDefaultGroup.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESUnderlyingLegTickerMnemonic);
                            sUnderlyingCFICode = singleDefaultGroup.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagCFICode);
                            sUnderlyingSymbol = singleDefaultGroup.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagUnderlyingSymbol);
                            sUnderlyingCurrency = singleDefaultGroup.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagUnderlyingCurrency);
                            sUnderlyingOppCurrency = singleDefaultGroup.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESUnderlyingOppCurrency);
                            dUnderlyingUnitTickValue = singleDefaultGroup.get_AsDouble(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESUnitTickValue);
                            nOptionType = singleDefaultGroup.get_AsNumber(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagUnderlyingPutOrCall);

                            //Get Month
                            m_nMonth = int.Parse(sYearMonth.Substring(4,2));
                            //Get Year
                            m_nYear = int.Parse(sYearMonth.Substring(0,4));
                        }
                    }
                }

                //if(sUnderlyingMnemonic != string.Empty)
                if (sUnderlyingCFICode != string.Empty)
                    m_sCFICode = sUnderlyingCFICode;
                //if(sUnderlyingSymbol != string.Empty)
                if (sUnderlyingCurrency != string.Empty)
                    m_sCurrency = sUnderlyingCurrency;
                //if(sUnderlyingOppCurrency != string.Empty)
                if (dUnderlyingUnitTickValue != 0.0)
                    m_dUnitTickValue = dUnderlyingUnitTickValue;

                if (nOptionType != -1)
                    m_nOptionType = nOptionType;
                
            }
        }

        /// <summary>
        /// Expiry Month
        /// </summary>
        int m_nMonth = int.MinValue;

        /// <summary>
        /// Expiry Month
        /// </summary>
        public int Month
        {
            get { return m_nMonth; } 
        }

        /// <summary>
        /// Expiry Year
        /// </summary>
        int m_nYear = int.MinValue;

        /// <summary>
        /// Expiry Year
        /// </summary>
        public int Year
        {
            get { return m_nYear; } 
        }

        /// <summary>
        /// handle structure update
        /// </summary>
        /// <param name="FIXMsg"></param>
        public void ReceiveStructureUpdate(EASYROUTERCOMCLIENTLib.IFIXMessage FIXMsg)
        {
            m_bSubscribedAdditionalInfo = true;

            int nNumber = FIXMsg.get_AsNumber(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESDecimalPlaces);
            
            int nPrefRiskValue = 0;
            if (FIXMsg.GetNumber(out nPrefRiskValue, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESPreferredRiskArrayType))
                m_nFIXTagESPreferredRiskArrayType = nPrefRiskValue;
    
            if(nNumber != 0)
                m_nESDecimalPlaces = nNumber;

            double dPriceMovement = 0.0;
            if (FIXMsg.GetDouble(out dPriceMovement, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESPriceMovement ))
                m_nFIXTagESPreferredRiskArrayType = nPrefRiskValue;

            if (dPriceMovement != 0)
                m_dPriceMovement = dPriceMovement; 

            string sStrategyCode = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESStrategyCode);
            if (sStrategyCode != string.Empty)
                m_sStrategyCode = sStrategyCode;

            string sSecurityID = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagSecurityID);
            if (sSecurityID != string.Empty)
                m_sSecurityID = sSecurityID;
            
            string sIDSource = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagIDSource);
            if (sIDSource != string.Empty)
                m_sIDSource = sIDSource;

            string sCombinedCommodityCode = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESCombinedCommodityCode);
            if (sCombinedCommodityCode != string.Empty)
                m_sCombinedCommodityCode = sCombinedCommodityCode;

            string sClearingHouse = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESClearingHouse);
            if (sClearingHouse != string.Empty)
            {
                m_sClearingHouse = sClearingHouse;

                System.Diagnostics.Debug.WriteLine(this.ESTickerMnemonic + " " + m_sClearingHouse);
            }

            int nOptionType = 0;
            if (FIXMsg.GetNumber(out nOptionType, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagPutOrCall))
            {
                //defaults to 1 - call if never here
                m_nOptionType = nOptionType;
            }

            //RISK ARRAYS
            string sRiskArray = string.Empty;
            int nRiskArrayType = 0;

            if (FIXMsg.HasGroup(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESNoRiskArrays))
            {
                EASYROUTERCOMCLIENTLib.IFIXGroup defaultGroup = FIXMsg.GetGroupByTag(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESNoRiskArrays, null);

                if (defaultGroup != null)
                {
                    int nDefaultCount = defaultGroup.get_NumberOfGroups(null);

                    for (int j = 0; j < nDefaultCount; j++)
                    {
                        EASYROUTERCOMCLIENTLib.IFIXGroup singleDefaultGroup = defaultGroup.GetGroupByIndex(j);


                        int nValue = singleDefaultGroup.get_AsNumber(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESRiskArrayType);
                        if (nValue != nRiskArrayType)
                        {
                            CreateRiskArray(nValue, sRiskArray);
                            nRiskArrayType = nValue;
                        }
                        string sValue = singleDefaultGroup.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESRiskArrayValue);

                        if (sValue != null)
                        {
                            sRiskArray = sRiskArray + sValue + "\\";
                        }
                    }
                }
            }
            CreateRiskArray(nRiskArrayType, sRiskArray);

        }

        Hashtable m_RiskMap = new Hashtable();

        /// <summary>
        /// default in eat
        /// </summary>
        int m_nFIXTagESPreferredRiskArrayType = int.MinValue;

        /// <summary>
        /// create risk array
        /// </summary>
        /// <param name="nValue"></param>
        /// <param name="sValue"></param>
        public void CreateRiskArray(int nValue, string sValue)
        {
            if (sValue.Length > 0)
            {
                sValue = sValue.Substring(0, sValue.Length - 1);
                string[] split = sValue.Split('\\');
                if (m_RiskMap.ContainsKey(nValue))
                {
                    m_RiskMap.Remove(nValue);
                }
                m_RiskMap[nValue] = split;
            }
        }

        /// <summary>
        /// Apply risk array
        /// </summary>
        /// <param name="message"></param>
        public void ApplyRiskArray(FIXMessage message)
        {
            ApplyRiskArray(message,m_nFIXTagESPreferredRiskArrayType);
        }

        /// <summary>
        /// Apply risk array to fix order message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="nFIXTagESPreferredRiskArrayType"></param>
        public void ApplyRiskArray(FIXMessage message,int nFIXTagESPreferredRiskArrayType )
        {
            //MESSAGEFIX3Lib.FIXESRiskArrayTypeConstants

             if (m_RiskMap.ContainsKey(nFIXTagESPreferredRiskArrayType))
             {
                 ApplyRiskArray(message, (string[])m_RiskMap[nFIXTagESPreferredRiskArrayType]);
             }
             else
             {
                if(m_RiskMap.ContainsKey((int)MESSAGEFIX3Lib.FIXESRiskArrayTypeConstants.esFIXESRiskArrayTypeESExternalFeed))
                {
                    ApplyRiskArray(message, (string[])m_RiskMap[(int)MESSAGEFIX3Lib.FIXESRiskArrayTypeConstants.esFIXESRiskArrayTypeESExternalFeed]);
                }
                else if(m_RiskMap.ContainsKey((int)MESSAGEFIX3Lib.FIXESRiskArrayTypeConstants.esFIXESRiskArrayTypeESIntraday))
                {
                    ApplyRiskArray(message, (string[])m_RiskMap[(int)MESSAGEFIX3Lib.FIXESRiskArrayTypeConstants.esFIXESRiskArrayTypeESIntraday]);
                }
                else if(m_RiskMap.ContainsKey((int)MESSAGEFIX3Lib.FIXESRiskArrayTypeConstants.esFIXESRiskArrayTypeESSettlement))
                {
                    ApplyRiskArray(message, (string[])m_RiskMap[(int)MESSAGEFIX3Lib.FIXESRiskArrayTypeConstants.esFIXESRiskArrayTypeESSettlement]);
                }
             }
        }

        /// <summary>
        /// Apply risk array to fix order message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="riskArray"></param>
        public void ApplyRiskArray(FIXMessage message,string[] riskArray)
        { 
            if(riskArray.Length > 0)
            {
                FIXGroup group = message.AddGroup(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagESNoRiskArrays);
                              
                if (group != null)
                {
                    for (int i = 0; i < riskArray.Length; i++)
                    {
                        if (riskArray[i].Length > 0)
                        {
                            try
                            {
                                FIXGroup subgroup = group.AddGroup(null);
                                if (subgroup != null)
                                {
                                    subgroup.set_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESRiskArrayValue, riskArray[i]);
                                }
                            }
                            catch (Exception e)
                            {
                                System.Diagnostics.Debug.WriteLine(e.ToString());     
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Request TE instrument
        /// </summary>
        /// <param name="client"></param>
        public void RequestStructure(VDMERLib.EasyRouter.EasyRouterClient.ERCSClient client)
        {
            if (m_bSubscribedAdditionalInfo == false)
            {
                client.RequestStructure(SecurityExchange, "", "", "", m_sESTickerMnemonic);
            }
        }

        /// <summary>
        /// Has been subscribed to
        /// </summary>
        public bool SubscribedAdditional
        {
            get { { return m_bSubscribedAdditionalInfo; } }
        }
    }
}
