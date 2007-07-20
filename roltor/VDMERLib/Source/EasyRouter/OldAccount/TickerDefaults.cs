/*
** TickerDefaults.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** Defaults for order tickets for specific exchanges
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
    /// Class TickerDefaults
    /// </summary>
    public class TickerDefaults : Hashtable 
    {
        /// <summary>
        /// Security Exchange code
        /// </summary>
        private string m_sExchange;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sExchange"></param>
        public TickerDefaults(string sExchange)
        {
            m_sExchange = sExchange;
        }

        /// <summary>
        /// Security Exchange code
        /// </summary>
        public string Exchange
        {
            get
            {
                return m_sExchange;
            }
        }

        /// <summary>
        /// Retrieve ticker information from FIX message
        /// </summary>
        /// <param name="group"></param>
        public void AddDefaults(EASYROUTERCOMCLIENTLib.IFIXGroup group)
        {
            EASYROUTERCOMCLIENTLib.IFIXGroup defaultGroup = group.GetGroupByTag(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESNoDefaultFields, null);

            int nDefaultCount = defaultGroup.get_NumberOfGroups(null);

            for (int j = 0; j < nDefaultCount; j++)
            {
                EASYROUTERCOMCLIENTLib.IFIXGroup singleDefaultGroup = defaultGroup.GetGroupByIndex(j);
                //get the defaults
                //ESDefaultFieldFIXTag=1
                //ESDefaultFieldName=Account
                //ESDefaultFieldValue=xxx
                //ESDefaultFieldEdit=Y
                //ESDefaultFieldDisplay=Account Code
                //ESDefaultFieldInputStyle=String
                string sTag = singleDefaultGroup.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESDefaultFieldFIXTag);
                int nTag = int.Parse(sTag); 
                string sValue = singleDefaultGroup.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESDefaultFieldValue);
                Add(nTag, sValue);
                System.Diagnostics.Debug.Print("TICKER DEFAULTS [{0}] = [{1}] [{2}]", m_sExchange, nTag, sValue); 
            }
        }

        /// <summary>
        /// Apply ticker info to order ticket fix message
        /// </summary>
        /// <param name="message"></param>
        public void ApplyTickerDefaults(FIXMessage message)
        {
            IDictionaryEnumerator it = this.GetEnumerator(); 

            while(it.MoveNext())
            {
                if(it.Value.ToString() != string.Empty)   
                    message.set_AsString((MESSAGEFIX3Lib.FIXTagConstants)it.Key, it.Value.ToString());   
            }
        }


    }
}
