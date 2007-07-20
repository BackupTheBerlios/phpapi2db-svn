/*
** ESUser.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** 
**
** Changes
** -------
**
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using MESSAGEFIX3Lib;
using EASYROUTERCOMCLIENTLib;

namespace VDMERLib.EasyRouter.OldAccount
{
    /// <summary>
    /// Class ESUser
    /// </summary>
    public class ESUser
    {
        /// <summary>
        /// ER client username
        /// </summary>
        private string m_sUserName  = "z";
        /// <summary>
        /// ER cleint Pwd
        /// </summary>
        private string m_sPassword  = "z";

        /// <summary>
        /// ES account id
        /// </summary>
        long? m_nAccountID; 
        
        /// <summary>
        /// Account Name
        /// </summary>
        string m_sAccountName = string.Empty;
        
        /// <summary>
        /// string (exchange) against ticker default list
        /// </summary>
        private Hashtable m_TickerMap = new Hashtable(7);  

        /// <summary>
        /// Vanilla
        /// </summary>
        public ESUser()
        { 
        
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sPassword"></param>
        /// <param name="sUserName"></param>
        public ESUser(string sPassword,string sUserName)
        {
            m_sUserName = sUserName;
            m_sPassword = sPassword;
        }

        /// <summary>
        /// Username
        /// </summary>
        public string Username
        {
            get { return m_sUserName; }
            set { m_sUserName = value; }
        }

        /// <summary>
        /// Password
        /// </summary>
        public string Password
        {
            get { return m_sPassword; }
            set { m_sPassword = value; }
        }

        /// <summary>
        /// Add Ticker defaults for this account
        /// </summary>
        /// <param name="sExchange"></param>
        /// <param name="group"></param>
        public void AddTicker(string sExchange, EASYROUTERCOMCLIENTLib.IFIXGroup group)
        { 
            if(!m_TickerMap.ContainsKey(sExchange))
            {
                TickerDefaults temp = new TickerDefaults(sExchange);
                temp.AddDefaults(group);
                m_TickerMap.Add(sExchange,temp);  
            }
        }

        /// <summary>
        /// Get Account ID
        /// </summary>
        public long? AccountID
        {
            get{{ return m_nAccountID;}}
        }

        /// <summary>
        /// Get Account Name
        /// </summary>
        public string AccountName
        {
            get{{ return m_sAccountName; }}
        }

        /// <summary>
        /// Get Process Account Details from FIX message
        /// </summary>
        /// <param name="FIXMsg"></param>
        public void ProcessAccount(EASYROUTERCOMCLIENTLib.IFIXMessage FIXMsg)
        {
            //repeats ESNoSecurityExchanges=2
            m_nAccountID = FIXMsg.get_AsNumber(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESAccountID);
            m_sAccountName = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESAccountName);

            bool bHasGroup = FIXMsg.HasGroup(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESNoSecurityExchanges);
          
            //note pass null 
            EASYROUTERCOMCLIENTLib.IFIXGroup group = FIXMsg.GetGroupByTag(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESNoSecurityExchanges, null);

            if (group != null)
            {
                int nCount = group.get_NumberOfGroups(null);
            
                for(int i = 0; i < nCount; i++)
                {
                    EASYROUTERCOMCLIENTLib.IFIXGroup singleGroup = group.GetGroupByIndex(i);

                    string sExchange = singleGroup.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagSecurityExchange);

                    AddTicker(sExchange, singleGroup);
                }
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
        public void ApplyDefaults(string sExchange, FIXMessage message)
        {
            int iClientID;
            if (!message.GetNumber(out iClientID, MESSAGEFIX3Lib.FIXTagConstants.esFIXTagESAccountID) && m_nAccountID.HasValue)
                message.set_AsNumber(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagESAccountID, (int)m_nAccountID.Value);
            TickerDefaults tickerDefaults = GetTickerDefaults(sExchange);
            if (tickerDefaults != null)
            {
                tickerDefaults.ApplyTickerDefaults(message);
            }
        }

    }
}
