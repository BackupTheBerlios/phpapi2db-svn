using System;
using System.Collections.Generic;
using System.Text;
using MESSAGEFIX3Lib;
using VDMERLib.EasyRouter;
using VDMERLib.EasyRouter.User; 
using System.Collections;

namespace VDMERLib.EasyRouter.Risk
{
    /// <summary>
    /// Account Info
    /// </summary>
    public class Account : Position 
    {
        private string m_strAccountName;
        
        /// <summary>
        /// Account Name
        /// </summary>
        public string AccountName
        {
          get { return m_strAccountName; }
        }
        
        private string m_strAccountCode;

        /// <summary>
        /// Account Code
        /// </summary>
        public string AccountCode
        {
          get { return m_strAccountCode; }
        }
        
        /// <summary>
        /// Account Description
        /// </summary>
        protected string m_strDescription;

        /// <summary>
        /// Account Description
        /// </summary>
        public string Description
        {
            get { return m_strDescription; }
        }

        private string m_strCurrency;

        /// <summary>
        /// Account Currency
        /// </summary>
        public string Currency
        {
          get { return m_strCurrency; }
        }

        private Dictionary <string,Position> m_Positions = new Dictionary<string,Position>();

        /// <summary>
        /// Positions
        /// </summary>
        public Dictionary<string, Position> Positions { get { return m_Positions; } }

        /// <summary>
        /// string (exchange) against ticker default list
        /// </summary>
        private Hashtable m_TickerMap = new Hashtable(7);  


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="AccountID"></param>
        /// <returns></returns>
        public Account(long iAccountID) : base(iAccountID)
        {
            m_iAccountID = iAccountID;
        }
        
        public bool DecodeRiskAccountFIXMessage(IFIXMessage FIXMsg)
        {
            m_strAccountName = FIXMsg.get_AsString(FIXTagConstants.esFIXTagESAccountName);
            m_strAccountCode = FIXMsg.get_AsString(FIXTagConstants.esFIXTagESAccountCode);
            m_strDescription = FIXMsg.get_AsString(FIXTagConstants.esFIXTagESAccountDesc);
            m_strCurrency = FIXMsg.get_AsString(FIXTagConstants.esFIXTagESAccountCurrency);
                    
                   /* If .GetDouble(dblValue, esFIXTagESAllowableMarginCredit) Then m_udtAccountCache(aCOL_ALLOWABLE_MARGIN_CREDIT, lngRow).vntValue = dblValue
                    If .GetDouble(dblValue, esFIXTagESGrossLiquidity) Then m_udtAccountCache(aCOL_GROSS_LIQUIDITY, lngRow).vntValue = dblValue
                    If .GetDouble(dblValue, esFIXTagESMarginFactor) Then m_udtAccountCache(aCOL_MARGIN_FACTOR, lngRow).vntValue = dblValue
                    
                    If .GetNumber(lngValue, esFIXTagESAccountStatus) Then m_udtAccountCache(aCOL_ACCOUNT_ACTIVE, lngRow).vntValue = GetRiskAccountStatusDesc(lngValue)
                    If .GetNumber(lngValue, esFIXTagESRiskPermissioningLevel) Then m_udtAccountCache(aCOL_RISK_PERMISSIONING_LEVEL, lngRow).vntValue = GetRiskPermissioningLevelDesc(lngValue)
                    m_udtAccountCache(aCOL_ACCOUNT_ACTIVE, lngRow).lngValidityStatus = esESStatusSeverityInfo
                    m_udtAccountCache(aCOL_RISK_PERMISSIONING_LEVEL, lngRow).lngValidityStatus = esESStatusSeverityInfo
               */
            return true;
                
        }
        /// <summary>
        /// Decode price FIX message
        /// </summary>
        /// <param name="FIXMsg"></param>
        /// <returns></returns>
        public bool DecodeFIXMessage(IFIXMessage FIXMsg)
        {
            try
            {
                IFIXGroup fixCountGroup = FIXMsg.GetGroupByTag(FIXTagConstants.esFIXTagESNoSecurityPositions, null);
                if (fixCountGroup != null)
                {
                    for (int i = 0; i < fixCountGroup.get_NumberOfGroups(null); i++)
                    {
                        IFIXGroup fixRptGroup = fixCountGroup.GetGroupByIndex(i);

                        string strMnemonic = fixRptGroup.get_AsString(FIXTagConstants.esFIXTagESTickerMnemonic);
                        string strSecExchange = fixRptGroup.get_AsString(FIXTagConstants.esFIXTagSecurityExchange);
                        string strContractGroup = fixRptGroup.get_AsString(FIXTagConstants.esFIXTagESContractGroup);
                        string strContractGroupOffset = fixRptGroup.get_AsString(FIXTagConstants.esFIXTagESMarginGroupOffset);
                        Position thePosition;
                        if (m_Positions.ContainsKey(strMnemonic))
                            thePosition = m_Positions[strMnemonic];
                        else
                        {
                            thePosition = new Position(m_iAccountID);
                            thePosition.Symbol = strMnemonic;
                            m_Positions.Add(strMnemonic, thePosition);
                        }
                        thePosition.DecodeFIXGroup(fixRptGroup);
                    }
                }
                IFIXGroup group = FIXMsg.GetGroupByTag(FIXTagConstants.esFIXTagESNoSecurityExchanges, null);

                if (group != null)
                {
                    int nCount = group.get_NumberOfGroups(null);

                    for (int i = 0; i < nCount; i++)
                    {
                        IFIXGroup singleGroup = group.GetGroupByIndex(i);

                        string sExchange = singleGroup.get_AsString(FIXTagConstants.esFIXTagSecurityExchange);

                        AddTicker(sExchange, singleGroup);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Add Ticker defaults for this account
        /// </summary>
        /// <param name="sExchange"></param>
        /// <param name="group"></param>
        public void AddTicker(string sExchange, IFIXGroup group)
        {
            if (!m_TickerMap.ContainsKey(sExchange))
            {
                TickerDefaults temp = new TickerDefaults(sExchange);
                temp.AddDefaults(group);
                m_TickerMap.Add(sExchange, temp);
            }
        }

        /// <summary>
        /// Get ticker defaults
        /// </summary>
        /// <param name="sExchange"></param>
        /// <returns></returns>
        public TickerDefaults GetTickerDefaults(string sExchange)
        {
            TickerDefaults tickerDefaults = null;
            if (m_TickerMap.ContainsKey(sExchange))
            {
                tickerDefaults = (TickerDefaults)m_TickerMap[sExchange];
            }
            return tickerDefaults;
        }

        /// <summary>
        /// Apply ticker defaults
        /// </summary>
        /// <param name="sExchange"></param>
        /// <param name="message"></param>
        public void ApplyDefaults(string sExchange,FIXMessage message)
        {
            TickerDefaults tickerDefaults = GetTickerDefaults(sExchange);
            if (tickerDefaults != null)
            {
                tickerDefaults.ApplyTickerDefaults(message);
            }
        }

        /// <summary>
        /// Apply ticker defaults
        /// </summary>
        /// <param name="sExchange"></param>
        /// <param name="message"></param>
        public void ApplyDefaults(string sExchange, FIXMessage message,Hashtable map)
        {
            TickerDefaults tickerDefaults = GetTickerDefaults(sExchange);
            if (tickerDefaults != null)
            {
                tickerDefaults.ApplyTickerDefaults(message,map);
            }
        }

    }
}
