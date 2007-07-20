/*
** InstrumentManager.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** InstrumentManager - top level object storing all exchange instrument data
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
    /// Class InstrumentManager
    /// </summary>
    public class InstrumentManager
    {
        /*/// InstrumentManager
        ///         
        ///     Map - SecurityExchange
        ///         Map - ESExcahnge
        ///             Map - Security Type
        ///                 Map - Commodity
        ///                     List - Tradable Entities*/
        
        /// <summary>
        /// Direct ESTickerMnemonic to TEInstrument infromation
        /// </summary>
        private Hashtable m_directTEMap = new Hashtable();
        private Hashtable m_directISINMap = new Hashtable();

        /// <summary>
        /// Add REInstrument to Local hashtable of ESTickerMnemonic
        /// </summary>
        /// <param name="instrument">returns Tradable Entity Information</param>
        public void AddDirectTE(TEInstrument instrument)
        {
            if (!m_directTEMap.ContainsKey(instrument.ESTickerMnemonic))
            {
                m_directTEMap[instrument.ESTickerMnemonic] = instrument;
            }
        }

        /// <summary>
        /// Get TEInstrument from TicketMnemonic
        /// </summary>
        /// <param name="sTE">TicketMnemonic</param>
        /// <returns></returns>
        public TEInstrument GetDirectTE(string sTE)
        {
            TEInstrument instrument = null;
            if (m_directTEMap.ContainsKey(sTE))
            {
                instrument = (TEInstrument)m_directTEMap[sTE];
            }
            return instrument; 
        }

        /// <summary>
        /// Map of Security  Exchanges
        /// </summary>
        private Hashtable m_InstrumentMap = new Hashtable();

        /// <summary>
        /// Vanilla
        /// </summary>
        public InstrumentManager()
        { 
        
        
        }


        //TODO: [15/04/07] YKC - Renable when decide what to do with completions
        //int m_nSecurityCompletion = 0;
        //int m_nESExchangeCompletion = 0;
        //int m_nTEInstrumentCompletion = 0;
        //int m_nCommodityCompletion = 0;
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HandleCompletionUpdate(EASYROUTERCOMCLIENTLib.IFIXMessage FIXMsg)
        {
            //TODO: [15/04/07] YKC - Renable when decide what to do with completions
        
            //m_nSecurityCompletion = 0;
            //m_nESExchangeCompletion = 0;
            //m_nTEInstrumentCompletion = 0;
            //m_nCommodityCompletion = 0;

            string sExchange = string.Empty;
            if (FIXMsg.GetString(out sExchange,FIXTagConstants.esFIXTagSecurityExchange))
            {
                //has security
                string sESExchange = string.Empty;
                if (FIXMsg.GetString(out sESExchange,FIXTagConstants.esFIXTagESExchange))
                {
                    //has esexchange
                    string sSecurityType = string.Empty; 
                    if(FIXMsg.GetString(out sSecurityType,FIXTagConstants.esFIXTagSecurityType))
                    {
                        string sSymbol = string.Empty;
                        if (FIXMsg.GetString(out sSymbol, FIXTagConstants.esFIXTagSymbol))
                        {
                            string sESTickerMnemonic = string.Empty;
                            if (FIXMsg.GetString(out sESTickerMnemonic, FIXTagConstants.esFIXTagESTickerMnemonic))
                            {
                                System.Diagnostics.Debug.WriteLine(string.Format("COMPLETION [{0}][{1}][{2}][{3}][{4}]", sExchange, sESExchange, sSecurityType, sSymbol, sESTickerMnemonic));   
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine(string.Format("COMPLETION [{0}][{1}][{2}][{3}]", sExchange, sESExchange, sSecurityType,sSymbol ));   
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("COMPLETION [{0}][{1}][{2}]", sExchange, sESExchange, sSecurityType));   
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("COMPLETION [{0}][{1}]",sExchange,sESExchange));   
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("COMPLETION [{0}]",sExchange));   
                }
            }
            else
            { 
                //Completion
                System.Diagnostics.Debug.WriteLine("COMPLETION [Unknown]");   
            }

            return true;
        }

        /// <summary>
        /// Get the security exchange if it exists
        /// </summary>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public SecurityExchange GetSecurityExchange(string sKey)
        {
            SecurityExchange exchange = null;
            if (m_InstrumentMap.ContainsKey(sKey))
            {
                exchange = (SecurityExchange)m_InstrumentMap[sKey]; 
            }
            return exchange;
        }

        /// <summary>
        /// Process Security FIX Update message
        /// </summary>
        /// <param name="FIXMsg"></param>
        /// <returns></returns>
        public SecurityExchange HandleSecurityUpdate(IFIXMessage FIXMsg)
        {
            SecurityExchange exchange = null;
            string sType = FIXMsg.get_AsString(FIXTagConstants.esFIXTagSecurityResponseType);

            if (sType == MESSAGEFIX3Lib.FIXSecurityResponseTypeConstants.esFIXSecurityResponseTypeReturnSecurityExchanges) //Exchange
            {
                //BeginString=FIX.4.2|BodyLength=29|MsgType=SecurityDef<d>|
                //SecurityResponseType=SecurityResponseTypeReturnSecurityExchanges<U>|
                //SecurityExchange=3|ESSecurityExchangeDesc=FOREX|CheckSum=220|10=250:<end>

                string sExchange = FIXMsg.get_AsString(FIXTagConstants.esFIXTagSecurityExchange);
                string sExchangeDesc = FIXMsg.get_AsString(FIXTagConstants.esFIXTagESSecurityExchangeDesc);

                //Add exchange if does not exist 
                exchange = AddExchange(sExchange, sExchangeDesc);
            }
            return exchange;
        }

        /// <summary>
        /// Process ESExchange FIX Update message
        /// </summary>
        /// <param name="FIXMsg"></param>
        /// <returns></returns>
        public ESExchange HandleESExchangeUpdate(IFIXMessage FIXMsg)
        {
            ESExchange esExchange = null;
            string sType = FIXMsg.get_AsString(FIXTagConstants.esFIXTagSecurityResponseType);

            if (sType == MESSAGEFIX3Lib.FIXSecurityResponseTypeConstants.esFIXSecurityResponseTypeReturnESExchanges)
            {
                string sExchange = FIXMsg.get_AsString(FIXTagConstants.esFIXTagSecurityExchange);
                string sESExchange = FIXMsg.get_AsString(FIXTagConstants.esFIXTagESExchange);
                string sExchangeDesc = FIXMsg.get_AsString(FIXTagConstants.esFIXTagESExchangeDesc);
#if DEBUG
                System.Diagnostics.Debug.WriteLine("SEC DEF V EXCHANGE = " + sExchange + " ESEXCHANGE = " + sESExchange);
#endif
                SecurityExchange exchange = GetExchange(sExchange);
                if (exchange != null)
                    esExchange = exchange.AddESExchange(sESExchange, sExchangeDesc);
            }
            return esExchange;
        }

        /// <summary>
        /// Process Tradable Entity Instrument FIX Update message
        /// </summary>
        /// <param name="FIXMsg"></param>
        /// <returns></returns>
        public TEInstrument HandleTEInstrumentUpdate(IFIXMessage FIXMsg)
        {
            TEInstrument instrument = null;
            string sType = FIXMsg.get_AsString(FIXTagConstants.esFIXTagSecurityResponseType);

            if (sType == MESSAGEFIX3Lib.FIXSecurityResponseTypeConstants.esFIXSecurityResponseTypeReturnSecurities)
            {
                //BeginString=FIX.4.2|BodyLength=118|
                //MsgType=SecurityDef<d>|SecurityResponseType=SecurityResponseTypeReturnSecurities<4>
                //|SecurityExchange=3|ESExchange=F|Symbol=USD|SecurityType=FOR|ESTickerDesc=USD-CAD|
                //ESTickerSymbol=USD-CAD|ESTimeType=TimeTypeImmediateAndCancel<0x00000002>
                //|ESOrderType=OrderTypeLimit<0x00000006>|ESSupportEdit=0|
                //ESPriceFormatCode=0|ESTickerMnemonic=3FX:USD-CAD|CheckSum=056|

                string sExchange = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagSecurityExchange);
                string sESExchange = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESExchange);
                string sSecurityType = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagSecurityType);
                string sSymbol = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagSymbol);
                string sESTickerMnemonic = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESTickerMnemonic);
                
                SecurityExchange exchange = GetExchange(sExchange);
                //have the top level exchange SECURITY EXCHAANGE eg EUREX VALUES V
                if (exchange != null)
                {
                    //have the esxchange ESEXCHANGE eg V 
                    ESExchange esexchange = exchange.GetESExchange(sESExchange);
                    if (esexchange == null)
                        esexchange = exchange.AddESExchange(sESExchange, "Unknown");
                    
                    //find the security type eg OPT
                    Commodity commodity = esexchange.GetCommodity(sSecurityType);

                    CommoditySymbol commoditysymbol = null;
                    //test if we have commodity else create the commodity + commodity symbol objects
                    if (commodity == null)
                    {
                        commoditysymbol = esexchange.AddCommodity(sSecurityType, sSymbol, "Unknown");
                    }
                    else
                    {
                        //find commodity if exists ESCONTRACT eg SIE 
                        commoditysymbol = commodity.AddCommoditySymbol(sSymbol, "Unknown");
                    }
                    //see if we have this TE already VVO:SIE Mar 07 7000c O
                    instrument = commoditysymbol.AddTEInstrument(sESTickerMnemonic, FIXMsg);
                    //add to direct map
                    if (instrument != null)
                    {
                        AddDirectTE(instrument);

                        if (instrument.m_sMDExchange.Length > 0 & instrument.m_sSecurityID.Length > 0)
                        { 
                            string sTempSymbol = instrument.m_sSecurityID + instrument.m_sMDExchange;
                            if (m_directISINMap.ContainsKey(sTempSymbol))
                                m_directISINMap[sTempSymbol] = instrument; 
                        }
                    }
                }
            }
            return instrument;
        }

        /// <summary>
        /// used to locate an equity by Exchange Reuters ID and ISIN 
        /// </summary>
        /// <param name="sSecurityID"></param>
        /// <param name="sExchange"></param>
        /// <returns></returns>
        public TEInstrument FindByISINandExchange(string sSecurityID, string sExchange)
        {
            TEInstrument instrument = null;
            string sSymbol = sSecurityID + sExchange;
            if (m_directISINMap.ContainsKey(sSymbol))
                instrument = (TEInstrument)m_directISINMap[sSymbol];
            return instrument;
        }

        /// <summary>
        /// Process Commodity FIX Update message
        /// </summary>
        /// <param name="FIXMsg"></param>
        /// <returns></returns>
        public CommoditySymbol HandleCommodityUpdate(IFIXMessage FIXMsg)
        {
            CommoditySymbol commoditysymbol = null;

            string sType = FIXMsg.get_AsString(FIXTagConstants.esFIXTagSecurityResponseType);

            if (sType == MESSAGEFIX3Lib.FIXSecurityResponseTypeConstants.esFIXSecurityResponseTypeReturnSymbols)
            {
                //BeginString=FIX.4.2|BodyLength=48|MsgType=SecurityDef<d>|
                //SecurityResponseType=SecurityResponseTypeReturnSymbols<W>|
                //SecurityExchange=3|ESExchange=F|Symbol=EUR|SecurityDesc=EUR|
                //SecurityType=FOR|CheckSum=038||10=038|'

                string sExchange = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagSecurityExchange);
                string sESExchange = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESExchange);
                string sSymbol = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagSymbol);
                string sSecurityType = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagSecurityType);
                string sSecurityDesc = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagSecurityDesc);

                SecurityExchange exchange = GetExchange(sExchange);
                if (exchange != null)
                {
                    ESExchange esexchange = exchange.GetESExchange(sESExchange);
                    if (esexchange != null)
                    {
                        commoditysymbol = esexchange.AddCommodity(sSecurityType, sSymbol, sSecurityDesc);
                    }
                }
            }
            return commoditysymbol;
        }

        /// <summary>
        /// Add Security Exchange to instrument map if does not exist
        /// </summary>
        /// <param name="sExchange">Security Exchange ID</param>
        /// <param name="sDescription">"Security Exchange Description</param>
        /// <returns>Either Creates new SecurityExchange if does not exist or retrives existing from Instruemnt Map</returns> 
        SecurityExchange AddExchange(string sExchange,string sDescription)
        { 
            SecurityExchange exchange = null;
            if(m_InstrumentMap.ContainsKey(sExchange))
            {
                exchange = (SecurityExchange)m_InstrumentMap[sExchange]; 
            }
            else
            {
                exchange = new SecurityExchange(sExchange,sDescription);
                m_InstrumentMap[sExchange] = exchange;
            }
            return exchange; 
        }

        /// <summary>
        /// Get the security exchange 
        /// </summary>
        /// <param name="sExchange">Security Exchange ID</param>
        /// <returns>returns SecurityExchange if exists in Instrument map else null</returns>
        SecurityExchange GetExchange(string sExchange)
        {
            SecurityExchange exchange = null;
            if (m_InstrumentMap.ContainsKey(sExchange))
            {
                exchange = (SecurityExchange)m_InstrumentMap[sExchange];
            }
            return exchange;
        }

        /// <summary>
        /// Property to access the instrument map which stores SecurityExchange
        /// </summary>
        public Hashtable Map
        {
            get { return m_InstrumentMap; }
        }

        /// <summary>
        /// HAve we subscribed at ESExchange level for the products
        /// </summary>
        /// <param name="sExchange">Security Exchange ID</param>
        /// <param name="sESExchange">ESExchange ID</param>
        /// <returns>true if we have sent subscription request</returns>
        public bool Subscribed(string sExchange,string sESExchange)
        {
            SecurityExchange exchange = GetExchange(sExchange);
            bool bSubscribed = false;
            if (exchange != null)
            {
                bSubscribed = exchange.Subscribed(sESExchange);  
            }
            return bSubscribed;
        }

        /// <summary>
        /// Have we subscribed at commodity level for the products
        /// </summary>
        /// <param name="sExchange">Security Exchange ID</param>
        /// <param name="sESExchange">ESExchange ID</param>
        /// <param name="sSymbol">Symbol ID</param>
        /// <param name="sSecurityType">Security Type eg "OPT" for options</param>
        /// <returns>true if we have sent subscription request</returns>
        public bool Subscribed(string sExchange, string sESExchange, string sSymbol, string sSecurityType)
        {
            SecurityExchange exchange = GetExchange(sExchange);
            bool bSubscribed = false;
            if (exchange != null)
            {
                bSubscribed = exchange.Subscribed(sESExchange,sSymbol,sSecurityType);  
            }
            return bSubscribed;
        }

        /// <summary>
        /// Set ESExchange as subscribed
        /// </summary>
        /// <param name="sExchange"></param>
        /// <param name="sESExchange"></param>
        public void SetSubscribed(string sExchange,string sESExchange)
        {
            SecurityExchange exchange = GetExchange(sExchange);
            if (exchange != null)
            {
                exchange.SetSubscribed(sESExchange);
            }   
        }

        /// <summary>
        /// Set Commodity as subscribed
        /// </summary>
        /// <param name="sExchange"></param>
        /// <param name="sESExchange"></param>
        /// <param name="sSymbol"></param>
        /// <param name="sSecurityType"></param>
        public void SetSubscribed(string sExchange, string sESExchange, string sSymbol, string sSecurityType)
        {
            SecurityExchange exchange = GetExchange(sExchange);
            if (exchange != null)
            {
                exchange.SetSubscribed(sESExchange, sSymbol,sSecurityType);
            }  
        }

       
    }
}
