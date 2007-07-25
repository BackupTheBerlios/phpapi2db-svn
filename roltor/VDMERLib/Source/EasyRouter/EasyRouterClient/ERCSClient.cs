/*
** Client.cs
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
using System.Runtime.InteropServices;
using System.Collections; 

using VDMERLib.EasyRouter.TCP; 
using EASYROUTERCOMCLIENTLib;
using VDMERLib.EasyRouter.User;
using VDMERLib.EasyRouter.Structure;

using VDMERLib.EasyRouter.Prices;
using VDMERLib.EasyRouter.EasyRouterClient.Data;
using MESSAGEFIX3Lib;
using VDMERLib.EasyRouter.Orders;
using VDMERLib.EasyRouter.General;
using VDMERLib.EasyRouter.Risk;
using System.Diagnostics;
using VDMERLib.Database.DataAccess;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace VDMERLib.EasyRouter.EasyRouterClient
{
    /// <summary>
    /// Class ERCSClient
    /// </summary>
    public class ERCSClient
    {
        /// <summary>
        /// Constantf for price subscription
        /// </summary>
        private const int SubscribeAll = 0;
        private const int SubscribeBest = 1;
        private const int SubscribeDepth = 2;

        /// <summary>
        /// URL connection for ER
        /// </summary>
        private string m_sConnection = "JABY-HOPE/erwebadmin";//"lon-dev-main/olt";  

        /// <summary>
        /// Easy Router com client proxy
        /// </summary>
        private EASYROUTERCOMCLIENTLib.EasyRouterClass m_Router;

        /// <summary>
        /// Fuck knows 
        /// </summary>
        private string m_sEATScreen = "??? [10205].809";

        private Dictionary<string, IProfile> m_PersistedClasses;

        /// <summary>
        /// Class to handle storing/parsing of order/trade data
        /// </summary>
        OrderManagement m_OrderManagement = null;

        /// <summary>
        /// Class to handle storing/parsing of price data
        /// </summary>
        PriceManagement m_PriceManagement = null;

        /// <summary>
        /// Class to handle storing/parsing of account/risk data
        /// </summary>
        AccountManager m_AccountManagement = null;

        /// <summary>
        /// Class to handle storing/parsing of account/risk data
        /// </summary>
        public AccountManager Accounts
        {
            get { return this.m_AccountManagement; }
        }

        public bool InBatchMode
        {
#warning "need to implement batch mode"
            get { return false; }
        }
        /*/// <summary>
        /// new FIX message event 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="args"></param>
        public delegate void RecvFixMsg(object from, FIXDataEventArgs args);*/

        /// <summary>
        /// new structured event delegate
        /// </summary>
        /// <param name="from"></param>
        /// <param name="args"></param>
        public delegate void RecvStructuredMsg(object from, StructureDataEventArg args);

        /// <summary>
        /// new structured event delegate
        /// </summary>
        /// <param name="from"></param>
        /// <param name="args"></param>
        public delegate void RecvPriceMsg(object from, PricesEventArg args);

        /// <summary>
        /// new component event delegate
        /// </summary>
        /// <param name="from"></param>
        /// <param name="args"></param>
        public delegate void RecvExchangeStatusMsg(object from, ComponentStatusArgs args);

        /// <summary>
        /// new order event delegate
        /// </summary>
        /// <param name="from"></param>
        /// <param name="args"></param>
        public delegate void RecvOrderMsg(object from, OrderDataEventArg args);

        /// <summary>
        /// General Msg
        /// </summary>
        /// <param name="from"></param>
        /// <param name="args"></param>
        public delegate void RecvGeneralMsg(object from, GeneralMsgEventArg args);

        /// <summary>
        /// Risk Msg
        /// </summary>
        /// <param name="from"></param>
        /// <param name="args"></param>
        public delegate void RecvRiskMsg(object from, RiskEventArg args);


        /// <summary>
        /// Profile Message
        /// </summary>
        /// <param name="from"></param>
        /// <param name="args"></param>
        public delegate void RecvProfileMsg(object from, ProfileMsgEventArgs args);

        /// <summary>
        /// request ESSxchange
        /// </summary>
        static public Hashtable m_subscriptionMap = new Hashtable(101);

        /*/// <summary>
        /// connection event 
        /// </summary>
        public event RecvFixMsg RecvFIXMsgEvent;*/

        /// <summary>
        /// Eventing structured messages
        /// </summary>
        public event RecvStructuredMsg RecvStructuredMsgEvent;

        /// <summary>
        /// Eventing structured messages
        /// </summary>
        public event RecvPriceMsg RecvPriceMsgEvent;

        /// <summary>
        /// Eventing exchange status messages
        /// </summary>
        public event RecvExchangeStatusMsg RecvExchangeStatusMsgEvent;

        /// <summary>
        /// Eventing exchange status messages
        /// </summary>
        public event RecvOrderMsg RecvOrderMsgEvent;

        /// <summary>
        /// Eventing general status messages
        /// </summary>
        public event RecvGeneralMsg RecvGeneralMsgEvent;

        /// <summary>
        /// Eventing risk messages
        /// </summary>
        public event RecvRiskMsg RecvRiskMsgEvent;

        /// <summary>
        /// Eventing profile messages
        /// </summary>
        public event RecvProfileMsg RecvProfileMsgEvent;

        /// <summary>
        /// Set DSN to use for custon tick data
        /// </summary>
        public string m_sDSN = string.Empty;

         /// <summary>
        /// User 
        /// </summary>
        ESUser m_ERUser;

        /// <summary>
        /// stores instrument and handles instrument subscription
        /// </summary>
        InstrumentManager m_InstrumentManager;

        /// <summary>
        /// if true will auto subcrible to all tradeble instruments - can be time consuming 
        /// </summary>
        private bool m_bEnableAutoSubcription = false;

        /// <summary>
        /// if true will auto subcrible to all tradeble instruments - can be time consuming 
        /// </summary>
        public bool EnableAutoSubscription
        {
            set { m_bEnableAutoSubcription = value; }
        }

        /// <summary>
        /// Database acccess DSN
        /// </summary>
        public string DSN
        {
            set { m_sDSN = value; }
        }

        /// <summary>
        /// if true will subcrible to tradeble instrument on recieving structured msg 
        /// </summary>
        public bool EnablePriceSubscriptionToTEonStructuredEvent
        {
            set { m_bEnableAutoPriceSubcription = value; }
        }

        /// <summary>
        /// if true will subcrible to tradeble instrument on recieving structured msg 
        /// </summary>
        bool m_bEnableAutoPriceSubcription = true;

        /// <summary>
        /// lock object used for static creation
        /// </summary>
        protected static object theLock = new object();

        /// <summary>
        /// static instance
        /// </summary>
        protected static ERCSClient m_objClient = null;

        /// <summary>
        /// Get Single Instance
        /// </summary>
        /// <returns></returns>
        static public ERCSClient GetInstance()
        {
            lock (theLock)
            {
                if (m_objClient==null)
                    m_objClient = new ERCSClient();
            }
            return m_objClient;
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="sUsername"></param>
        /// <param name="sPassword"></param>
        /// <param name="sConnection"></param>
        protected ERCSClient()
        {
            if (m_Router == null)
            {
                m_Router = new EASYROUTERCOMCLIENTLib.EasyRouterClass();
                m_InstrumentManager = new InstrumentManager();
                m_OrderManagement = new OrderManagement(m_InstrumentManager);
                m_PriceManagement = new PriceManagement();
                m_ERUser = new ESUser();
                m_AccountManagement = new AccountManager();
                m_PersistedClasses = new Dictionary<string, IProfile>();
            }
            RegisterCallbacks();
        }

        /// <summary>
        /// 
        /// </summary>
        ~ERCSClient()
        {
            UnRegisterCallBack();
            if (m_CustomAccessor != null)
                m_CustomAccessor.Close();  
        }

        /// <summary>
        /// Unused ....
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
            bool bRetVal = true;
            
            return bRetVal;
        }

        /// <summary>
        /// Unused........
        /// </summary>
        public void UnInitialize()
        {
            
        }

        /// <summary>
        /// Register com events from er client 
        /// </summary>
        public virtual void RegisterCallbacks()
        {
            m_Router.RecvStructureMsg += new DEasyRouterEvents_RecvStructureMsgEventHandler(m_Router_RecvStructureMsg);
            m_Router.RecvGeneralMsg += new DEasyRouterEvents_RecvGeneralMsgEventHandler(m_Router_RecvGeneralMsg);
            m_Router.RecvOrderMsg += new DEasyRouterEvents_RecvOrderMsgEventHandler(m_Router_RecvOrderMsg);
            m_Router.RecvMarketDataMsg += new DEasyRouterEvents_RecvMarketDataMsgEventHandler(m_Router_RecvMarketDataMsg);
        }

        /// <summary>
        /// UnRegister com events from er client 
        /// </summary>
        public virtual void UnRegisterCallBack()
        {
            try
            {
                m_Router.RecvStructureMsg -= new DEasyRouterEvents_RecvStructureMsgEventHandler(m_Router_RecvStructureMsg);
                m_Router.RecvGeneralMsg -= new DEasyRouterEvents_RecvGeneralMsgEventHandler(m_Router_RecvGeneralMsg);
                m_Router.RecvOrderMsg -= new DEasyRouterEvents_RecvOrderMsgEventHandler(m_Router_RecvOrderMsg);
                m_Router.RecvMarketDataMsg -= new DEasyRouterEvents_RecvMarketDataMsgEventHandler(m_Router_RecvMarketDataMsg);
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("EXIT");    
            }
        }

        /// <summary>
        /// recieve market update
        /// </summary>
        /// <param name="FIXMsg"></param>
        void m_Router_RecvMarketDataMsg(EASYROUTERCOMCLIENTLib.IFIXMessage FIXMsg)
        {
            EASYROUTERCOMCLIENTLib.FIXMsgConstants type = FIXMsg.MsgType;

            switch(type)
            {
                case EASYROUTERCOMCLIENTLib.FIXMsgConstants.esFIXMsgMarketDataSnapFull:
                    {
                        TradeData data = m_PriceManagement.ProcessPrice(FIXMsg, true);
                        if (RecvPriceMsgEvent != null)
                            RecvPriceMsgEvent(this, data);
                        data = null;
                        break;
                    }
                case EASYROUTERCOMCLIENTLib.FIXMsgConstants.esFIXMsgMarketDataSnapInc:
                    {
                        TradeData data = m_PriceManagement.ProcessPrice(FIXMsg, true);
                        if (RecvPriceMsgEvent != null)
                            RecvPriceMsgEvent(this, data);
                        data = null;
                        break;
                    }
                case EASYROUTERCOMCLIENTLib.FIXMsgConstants.esFIXMsgSecuritySts:
                    {
                        SecurityStatus data = new SecurityStatus();

                        if (data.DecodeFIX(FIXMsg))
                        {
                            TEInstrument instrument = m_InstrumentManager.GetDirectTE(data.Symbol); 
                            if(instrument != null)
                            {
                                instrument.SecurityStatus = null;
                                instrument.SecurityStatus = data;
                            }
                            
                            if (RecvPriceMsgEvent != null)
                            {
                                RecvPriceMsgEvent(this, data);
                            }
                        }
                        data = null;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Recieves Account Message
        /// </summary>
        /// <param name="FIXMsg"></param>
        void m_Router_RecvOrderMsg(EASYROUTERCOMCLIENTLib.IFIXMessage FIXMsg)
        {
            EASYROUTERCOMCLIENTLib.FIXMsgConstants type = FIXMsg.MsgType;

            switch (type)
            {
                case EASYROUTERCOMCLIENTLib.FIXMsgConstants.esFIXMsgESAccount:
                    //System.Diagnostics.Debug.WriteLine(EASYROUTERCOMCLIENTLib.FIXMsgConstants.esFIXMsgESAccount.ToString());
                    //m_ERUser.ProcessAccount(FIXMsg);
                    m_AccountManagement.DecodeFIX((MESSAGEFIX3Lib.IFIXMessage)FIXMsg);

                    break;
                case EASYROUTERCOMCLIENTLib.FIXMsgConstants.esFIXMsgOrderCancelReject:
                    {
                        //System.Diagnostics.Debug.WriteLine(EASYROUTERCOMCLIENTLib.FIXMsgConstants.esFIXMsgOrderCancelReject.ToString());
                        OrderInfo order = m_OrderManagement.ProcessOrderCancelReject(FIXMsg);

                        if (order != null && RecvOrderMsgEvent != null)
                            RecvOrderMsgEvent(this, order);

                        break;
                    }
                case EASYROUTERCOMCLIENTLib.FIXMsgConstants.esFIXMsgExecutionReport:
                    {
                        //System.Diagnostics.Debug.WriteLine(EASYROUTERCOMCLIENTLib.FIXMsgConstants.esFIXMsgExecutionReport.ToString());
                        OrderInfo order = m_OrderManagement.ProcessExecutionReport(FIXMsg);

                        if (order != null && RecvOrderMsgEvent != null)
                            RecvOrderMsgEvent(this, order);

                        break;
                    }
                case EASYROUTERCOMCLIENTLib.FIXMsgConstants.esFIXMsgESRiskAccount:
                    {
                        //System.Diagnostics.Debug.WriteLine(EASYROUTERCOMCLIENTLib.FIXMsgConstants.esFIXMsgESRiskAccount.ToString());
                        m_AccountManagement.DecodeFIX((MESSAGEFIX3Lib.IFIXMessage)FIXMsg);
                        if (RecvRiskMsgEvent != null)
                        {
                            foreach (KeyValuePair<long, Account> kvp in m_AccountManagement)
                            {
                                Account theAccount = kvp.Value;
                                if (theAccount.IsDirty)
                                {
                                    RecvRiskMsgEvent(this, theAccount);
                                    theAccount.IsDirty = false;
                                }
                                foreach (KeyValuePair<string, Position> kvp2 in theAccount.Positions)
                                {
                                    Position thePosition = kvp2.Value;
                                    if (thePosition.IsDirty)
                                    {
                                        RecvRiskMsgEvent(this, thePosition);
                                        theAccount.IsDirty = false;
                                    }
                                }
                            }
                        }
                        break;
                    }
                case EASYROUTERCOMCLIENTLib.FIXMsgConstants.esFIXMsgESPosition:
                    {
                        //System.Diagnostics.Debug.WriteLine(EASYROUTERCOMCLIENTLib.FIXMsgConstants.esFIXMsgESPosition.ToString());
                        m_AccountManagement.DecodeFIX((MESSAGEFIX3Lib.IFIXMessage)FIXMsg);
                        if (RecvRiskMsgEvent != null)
                        {
                            foreach (KeyValuePair<long, Account> kvp  in m_AccountManagement)
                            {
                                Account theAccount = kvp.Value;
                                if (theAccount.IsDirty)
                                {
                                    RecvRiskMsgEvent(this, theAccount);
                                    theAccount.IsDirty = false;
                                }
                                foreach (KeyValuePair<string, Position> kvp2 in theAccount.Positions)
                                {
                                    Position thePosition = kvp2.Value;
                                    if (thePosition.IsDirty)
                                    {
                                        RecvRiskMsgEvent(this, thePosition); 
                                        theAccount.IsDirty = false;
                                    }
                                }
                            }
                        }
                        break;
                    }
                default:
                    System.Diagnostics.Debug.WriteLine(EASYROUTERCOMCLIENTLib.FIXMsgConstants.esFIXMsgESAccount.ToString());
                    break;
            }
        }

        /// <summary>
        /// Recieve account and logon message
        /// </summary>
        /// <param name="FIXMsg"></param>
        void m_Router_RecvGeneralMsg(EASYROUTERCOMCLIENTLib.IFIXMessage FIXMsg)
        {
            EASYROUTERCOMCLIENTLib.FIXMsgConstants type = FIXMsg.MsgType;

            switch (type)
            {
                case EASYROUTERCOMCLIENTLib.FIXMsgConstants.esFIXMsgESAccount:
                    System.Diagnostics.Debug.WriteLine(type.ToString());
                    break;
                case EASYROUTERCOMCLIENTLib.FIXMsgConstants.esFIXMsgLogon:
                    {
                        Logon logon = new Logon();
                        if(FIXMsg.HasGroup(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagText))
                        {
                            string sMessage = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagText);  
                            string[] split = sMessage.Split('|');
                            if (split.Length > 0)
                                logon.Error = split[0];
                            else
                                logon.Error = "Logon Error Undefined";
                            logon.ErrorCode = FIXMsg.get_AsNumber(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESLogonFailureCode);
                            logon.LoggedOn = false;
                        }
                        else
                        {
                            System.Diagnostics.Trace.WriteLine(type.ToString());
                            RequestStructure("0");
                            logon.LoggedOn = true;
                            LoadSystemSettings();
                        }
                        logon.ConnectionType = (Logon.ConnectionTypes)FIXMsg.get_AsNumber(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESEasyRouterType);
                        logon.ConnectionDescription = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESEasyRouterDesc);
                        
                        if (RecvGeneralMsgEvent != null)
                            RecvGeneralMsgEvent(this, logon); 
                        break;
                    }
                case EASYROUTERCOMCLIENTLib.FIXMsgConstants.esFIXMsgEHControl:
                    break;
                case EASYROUTERCOMCLIENTLib.FIXMsgConstants.esFIXMsgESComponentStatus:
                    UpdateComponentStatus(FIXMsg);
                    break;
                case EASYROUTERCOMCLIENTLib.FIXMsgConstants.esFIXMsgLogout:
                    {
                        Logon logoff = new Logon();
                        logoff.Error = "Successfully Logged Out";
                        logoff.LoggedOn = false;
                        logoff.LoggedOff = true;
                        if (RecvGeneralMsgEvent != null)
                            RecvGeneralMsgEvent(this, logoff);
                        break;
                    }
                default:
                    System.Diagnostics.Debug.WriteLine(type.ToString());
                    break;
            }
        }

        /// <summary>
        /// Variable used to determine if it is an exchange component not a router component
        /// </summary>
        private const string RouterModule = "0"; 

        /// <summary>
        /// Variable used to determine if it is an exchange component not a router component
        /// </summary>
        private const int RouterChannel = 1;


        public bool LoadSystemSettings()
        {
            try
            {
                string[] arrNames;
                object objProfileNames, objForms, objUIDs, objSettings;
                string strSettings;
                
                m_Router.GetProfileNames(out objProfileNames);
                
                m_Router.GetProfile("DEFAULT", out objForms, out objUIDs, out objSettings);
                if (objForms != null)
                {
                    string[] strForms = (string[])objForms;
                    for (int iCount = 0; iCount < strForms.Length; iCount++)
                    {
                        if (RecvProfileMsgEvent != null)
                        {
                            ProfileMsgEventArgs ProfileArgs = new ProfileMsgEventArgs();
                            ProfileArgs.ScreenID = (ScreenIDs)Int32.Parse(strForms[iCount]);
                            ProfileArgs.Profile = ((string[])objSettings)[iCount];
                            ProfileArgs.InstanceID = Int32.Parse(((string[])objUIDs)[iCount]);
                            RecvProfileMsgEvent(this, ProfileArgs);
                        }
                    }
                }

                m_Router.GetProfileEntry("SYSTEM", "0", "0", out strSettings);              

                if (strSettings.Length == 0)
                    return false;

                Accounts.Allocations.Add(new Allocation("New Allocation"));
                bool bSucceeded = true;
                XmlReader rd = XmlReader.Create(new StringReader(strSettings));
                rd.Read(); //Start Document
                while (rd.Read())
                {
                    if (m_PersistedClasses.ContainsKey(rd.Name))
                    {
                        IProfile nextProfile = m_PersistedClasses[rd.Name];
                        bSucceeded &= nextProfile.ReadProperties(rd);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogFileWriter.TraceEvent(TraceEventType.Error, ex.ToString());
                return false;
            }
        }

        public void SaveSystemSettings()
        {
            try
            {
                StringBuilder theString = new StringBuilder();
                XmlWriter SystemSettings = XmlWriter.Create(theString);

                if (m_PersistedClasses.Count == 0)
                {
                    m_Router.SetProfileEntry("SYSTEM", "0", "0", "");
                    return;
                }

                SystemSettings.WriteStartDocument();
                foreach (KeyValuePair<string, IProfile> kvp in m_PersistedClasses) 
                {
                    IProfile nextProfile = kvp.Value;
                    SystemSettings.WriteStartElement(nextProfile.FormName);
                    nextProfile.WriteProperties(SystemSettings);
                    SystemSettings.WriteEndElement();
                }
                SystemSettings.WriteEndDocument();
                SystemSettings.Flush();
                m_Router.SetProfileEntry("SYSTEM", "0", "0", theString.ToString());
                m_Router.CommitProfiles();
            }
            catch (Exception ex)
            {
                LogFileWriter.TraceEvent(TraceEventType.Error, ex.ToString());
            }

        }

        /// <summary>
        /// handle component status events 
        /// </summary>
        /// <param name="FIXMsg"></param>
        void UpdateComponentStatus(EASYROUTERCOMCLIENTLib.IFIXMessage FIXMsg)
        {
            string sModule = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESComponentModule);
            int nModule = FIXMsg.get_AsNumber(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESComponentChannel);
            string sCategory = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESComponentCategory);

            if (sModule != RouterModule && nModule != RouterChannel)
            {
                SecurityExchange exchange = m_InstrumentManager.GetSecurityExchange(sModule);

                if (exchange != null)
                {
                    string strExchangeDesc = exchange.ExchangeDesc;

                    if ((sCategory == MESSAGEFIX3Lib.FIXComponentCategoryConstants.esFIXComponentCategoryOrders) ||
                        (sCategory == MESSAGEFIX3Lib.FIXComponentCategoryConstants.esFIXComponentCategoryPricesDepth) ||
                        (sCategory == MESSAGEFIX3Lib.FIXComponentCategoryConstants.esFIXComponentCategoryPricesBest) ||
                        (sCategory == MESSAGEFIX3Lib.FIXComponentCategoryConstants.esFIXComponentCategoryStructure))
                    {
                        int nTotalActive = FIXMsg.get_AsNumber(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESComponentTotalActive);
                        int nTotalWaiting = FIXMsg.get_AsNumber(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESComponentTotalActive);
                        int nTotalInactive = FIXMsg.get_AsNumber(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESComponentTotalWaiting);
                        int nTotalStale = FIXMsg.get_AsNumber(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESComponentTotalStale);

                        ComponentStatusArgs args = new ComponentStatusArgs(sCategory,sModule,exchange.ExchangeDesc);
                        args.SetStatus(nTotalActive, nTotalWaiting, nTotalInactive);

                        if(RecvExchangeStatusMsgEvent!=null)
                        {
                            RecvExchangeStatusMsgEvent(this, args);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// handle structre data
        /// </summary>
        /// <param name="FIXMsg"></param>
        void m_Router_RecvStructureMsg(EASYROUTERCOMCLIENTLib.IFIXMessage FIXMsg)
        {
            string sType = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagSecurityResponseType);

            if (sType == MESSAGEFIX3Lib.FIXSecurityResponseTypeConstants.esFIXSecurityResponseTypeReturnESExchanges)
            {
                ESExchange esexchange = m_InstrumentManager.HandleESExchangeUpdate(FIXMsg);
                string sExchange = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagSecurityExchange);
                string sESExchange = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESExchange);
                System.Diagnostics.Debug.WriteLine("SEC DEF V EXCHANGE = " + sExchange + " ESEXCHANGE = " + sESExchange);
                if (RecvStructuredMsgEvent != null && esexchange != null)
                    RecvStructuredMsgEvent(this, esexchange);

                if (esexchange != null)
                {
                    if (m_bEnableAutoSubcription)
                    {
                        if (esexchange.Subscribed == false)
                        {
                            RequestStructure(sExchange, sESExchange);
                            m_subscriptionMap.Add(sExchange + sESExchange, null);
                            esexchange.Subscribed = true;
                        }
                    }
                }
               
            }
            else if (sType == MESSAGEFIX3Lib.FIXSecurityResponseTypeConstants.esFIXSecurityResponseTypeReturnSecurityExchanges)
            { 
                //Security
                SecurityExchange exchange = m_InstrumentManager.HandleSecurityUpdate(FIXMsg); 
                //string sExchange = FIXMsg.get_AsString(FIXTagConstants.esFIXTagSecurityExchange);
                //string sExchangeDesc = FIXMsg.get_AsString(FIXTagConstants.esFIXTagESSecurityExchangeDesc);
                //System.Diagnostics.Debug.WriteLine("SEC DEF U MESSAGE EXCHANGE = " + sExchange + " DESCRIPTION = " + sExchangeDesc);
                if (RecvStructuredMsgEvent != null && exchange != null)
                    RecvStructuredMsgEvent(this, exchange); 
                return;
            }
            else if (sType == MESSAGEFIX3Lib.FIXSecurityResponseTypeConstants.esFIXSecurityResponseTypeEndOfSecurityResponses)
            { 
                //Completion
                m_InstrumentManager.HandleCompletionUpdate(FIXMsg); 
                if (RecvStructuredMsgEvent != null)
                    RecvStructuredMsgEvent(this, new StructureDataEventArg(StructureDataType.EndOfResponses)); 
                
                //string sExchange = FIXMsg.get_AsString(FIXTagConstants.esFIXTagSecurityExchange);
                //System.Diagnostics.Debug.WriteLine("SEC DEF X MESSAGE COMPLETION EXCHANGE = " + sExchange);

                return;
            }
            else if (sType == MESSAGEFIX3Lib.FIXSecurityResponseTypeConstants.esFIXSecurityResponseTypeReturnSymbols)
            {
                CommoditySymbol commodity = m_InstrumentManager.HandleCommodityUpdate(FIXMsg); 
                //string sExchange = FIXMsg.get_AsString(FIXTagConstants.esFIXTagSecurityExchange);
                //string sESExchange = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESExchange);
                //string sSymbol = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagSymbol);
                //string sSecurityType = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagSecurityType);

                //System.Diagnostics.Debug.WriteLine("SEC DEF W MESSAGE COMPLETION EXCHANGE = " + sExchange + " Es Exchange = " + sESExchange  + " Symbol = " + sSymbol);
                if (RecvStructuredMsgEvent != null && commodity != null)
                    RecvStructuredMsgEvent(this, commodity);

                //hack for OPT
                //if (commodity.CommodityObj.CommodityCode == "OPT")
                if (m_bEnableAutoSubcription)
                {
                    if (commodity != null)
                    {
                        string sKey = commodity.ToString();//sExchange + sESExchange + sSymbol + sSecurityType;

                        if (commodity.Subscribed != true)
                        {
                            //RequestStructure(sExchange, sESExchange, sSymbol, sSecurityType);
                            RequestStructure(commodity.Exchange, commodity.ESExchange, commodity.CommoditySymbolCode, commodity.CommodityObj.CommodityCode);
                            commodity.Subscribed = true;
                        }
                    }
                }
            }
            else if (sType == MESSAGEFIX3Lib.FIXSecurityResponseTypeConstants.esFIXSecurityResponseTypeReturnSecurities) 
            {
                TEInstrument instrument = m_InstrumentManager.HandleTEInstrumentUpdate(FIXMsg);
                if (instrument != null)
                {
                    if (RecvStructuredMsgEvent != null)
                        RecvStructuredMsgEvent(this, instrument);
                    //Get price
                    if (instrument.SubscribedAdditional == false)
                        instrument.RequestStructure(this);  
                    //request price
                    if(m_bEnableAutoPriceSubcription == true)
                        m_Router.SendMarketDataRequest(instrument.SecurityExchange, instrument.ESTickerMnemonic, EFIXValueSubscriptionRequestType.eSubscriptionSubscribe, SubscribeAll);
                }
            }

            string sTicker = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESTickerMnemonic);
            if (sTicker != null)
            {
                if (sTicker.Length > 0)
                {
                    System.Diagnostics.Debug.WriteLine("SEC DEF MESSAGE TE = " + sTicker);
                    string sSecurityType = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagSecurityResponseType);
                    if (sSecurityType != null)
                    {
                        if (sSecurityType == MESSAGEFIX3Lib.FIXSecurityResponseTypeConstants.esFIXSecurityResponseTypeRelationshipUpdate)
                        { 
                            //update
                            System.Diagnostics.Debug.WriteLine("SEC DEF MESSAGE TE = " + sTicker);
                            string sESTickerMnemonic = FIXMsg.get_AsString(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESTickerMnemonic);

                            TEInstrument instrument = m_InstrumentManager.GetDirectTE(sESTickerMnemonic);
                            if (instrument != null)
                            {
                                instrument.ReceiveStructureUpdate(FIXMsg); 
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calls er client to get connection details 
        /// </summary>
        /// <returns></returns>
        public bool GetRemoteConnection()
        {
            //"Return the array of remote connections, a default, and whether the client should be allowed to enter their own values")]
            //HRESULT GetRemoteConnectionDetails([out] VARIANT* ConnectionList, [out] VARIANT* Default, [out] VARIANT_BOOL* IsAllowedToEdit, [out, retval] VARIANT_BOOL* Success)

            bool bRetVal = false;

            object objConnection = new object();
            object objDefault = new object();
            bool bEdit;

            if (m_Router.GetRemoteConnectionDetails(out objConnection, out objDefault,out bEdit))
            {
                bRetVal = true;
            }
            return bRetVal;
        }

        /// <summary>
        /// try to logon on to ER
        /// </summary>
        /// <returns></returns>
        public bool Logon(string sUsername, string sPassword, string sConnection, bool bDeferToExistingSession)
        {
            m_ERUser.Username = sUsername;
            m_ERUser.Password = sPassword;
            m_sConnection = sConnection;

            if (m_Router.SendLogon(GetUsername(), CreatePasswordString(), m_sConnection, bDeferToExistingSession, m_sEATScreen))
            {
                Console.Write("Logon");
                return true;
            }
            else 
            {
                Console.Write("Not Logon");
                return false;
            }
        }

        /// <summary>
        /// log off
        /// </summary>
        public void Logoff()
        {
            m_Router.CommitProfiles(); 
            m_subscriptionMap.Clear();
            m_Router.SendLogout(); 
        }

        /// <summary>
        /// Request one structure data for specific TE
        /// </summary>
        /// <param name="sSecurityExchange"></param>
        /// <param name="sESExchange"></param>
        /// <param name="sSymbol"></param>
        /// <param name="sSecurityType"></param>
        /// <param name="sESTickerMnemonic"></param>
        /// <returns></returns>
        public bool RequestStructure(string sSecurityExchange, string sESExchange, string sSymbol, string sSecurityType, string sESTickerMnemonic)
        {
            return m_Router.SendSecurityDefRq(sSecurityExchange, sESExchange, sSymbol, sSecurityType, sESTickerMnemonic);
        }
        
        /// <summary>
        /// Request all TE structure data for a specific commodity symbol and security type
        /// </summary>
        /// <param name="sSecurityExchange"></param>
        /// <param name="sESExchange"></param>
        /// <param name="sSymbol"></param>
        /// <param name="sSecurityType"></param>
        /// <returns></returns>
        public bool RequestStructure(string sSecurityExchange, string sESExchange, string sSymbol, string sSecurityType)
        {
            return m_Router.SendSecurityDefRq(sSecurityExchange, sESExchange, sSymbol, sSecurityType, "");
        }
        
        /// <summary>
        /// Request all TE structure data for a specific commodity
        /// </summary>
        /// <param name="sSecurityExchange"></param>
        /// <param name="sESExchange"></param>
        /// <param name="sSymbol"></param>
        /// <returns></returns>
        public bool RequestStructure(string sSecurityExchange, string sESExchange, string sSymbol)
        {
            return m_Router.SendSecurityDefRq(sSecurityExchange, sESExchange, sSymbol, "", "");
        }
        
        /// <summary>
        /// Request all commodity for an ESExchange
        /// </summary>
        /// <param name="sSecurityExchange"></param>
        /// <param name="sESExchange"></param>
        /// <returns></returns>
        public bool RequestStructure(string sSecurityExchange, string sESExchange)
        {
            return m_Router.SendSecurityDefRq(sSecurityExchange, sESExchange, "", "", "");
        }

        /// <summary>
        /// Request all data for a Mnemonic
        /// </summary>
        /// <param name="sTEMnemonic"></param>
        /// <returns></returns>
        public bool RequestData(string sTEMnemonic)
        {
            return m_Router.SendSecurityDefRq(sTEMnemonic.Substring(0, 1), "", "", "", sTEMnemonic);
        }

        /// <summary>
        /// Subscribe Price for TE
        /// </summary>
        /// <param name="sTEMnemonic">ES Instruemnt ID</param>
        /// <returns></returns>
        public bool SubscribeTE(string sTEMnemonic)
        {
            TEInstrument instrument = (TEInstrument)m_InstrumentManager.GetDirectTE(sTEMnemonic);
            if (instrument == null)
                RequestData(sTEMnemonic);
            
            return m_Router.SendMarketDataRequest(sTEMnemonic.Substring(0, 1), sTEMnemonic, EFIXValueSubscriptionRequestType.eSubscriptionSubscribe, SubscribeAll);
        }
        
        /// <summary>
        /// Unsubscribe price for TE 
        /// </summary>
        /// <param name="sTEMnemonic">ES Instruemnt ID</param>
        /// <returns>retrun false if no TE is set, else true if request is sent</returns>
        public bool UnsubscribeTE(string sTEMnemonic)
        {
            TEInstrument instrument = (TEInstrument)m_InstrumentManager.GetDirectTE(sTEMnemonic);
            if (instrument != null)
            {
                return m_Router.SendMarketDataRequest(sTEMnemonic.Substring(0, 1), sTEMnemonic, EFIXValueSubscriptionRequestType.eSubscriptionUnsubscribe, SubscribeAll);
            }
            else
            {
                return false;
            }
        }
        
        /// <summary>
        /// Request all ESExchange for a specific security exchange - 
        /// </summary>
        /// <param name="sSecurityExchange">"0" for all security exchanges</param>
        /// <returns></returns>
        public bool RequestStructure(string sSecurityExchange)
        {
            return m_Router.SendSecurityDefRq(sSecurityExchange, "", "", "", "");
        }
        
        /// <summary>
        /// generate the magic password - takem from eat
        /// </summary>
        /// <returns></returns>
        public string CreatePasswordString()
        {
            String sPassword = string.Empty;
            try
            {
                Random rand = new Random();

                //get the day
                int nDay = DateTime.Now.Day;

                //create a 31 character string of crap
                double dRand = rand.NextDouble();

                char cConstant = 'A';
                int nConstant = (char)cConstant;
                char cConstantZ = 'Z';
                int nConstantZ = (char)cConstantZ;
                int nConstantAZ = nConstant + nConstantZ;

                for (int i = 0; i < 31; i++)
                {
                    int nTemp = (int)(dRand * 25.0);


                    char cTemp = (char)(nTemp + nConstant);
                    sPassword += cTemp;
                    dRand = rand.NextDouble();
                }

                char cMagicTemp = sPassword[nDay - 1];
                int nDayChar = (int)cMagicTemp;

                nConstantAZ = nConstantAZ - nDayChar;

                sPassword += (char)nConstantAZ;
                sPassword += m_ERUser.Password;

                char cFirst = sPassword[nDay - 1];
                char cLast = sPassword[sPassword.Length - 1];

                bool bRet = (cFirst + cLast - 'A') == 'Z';
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            }
            return sPassword; 
        }

        /// <summary>
        /// return username as uppercase
        /// </summary>
        /// <returns></returns>
        public string GetUsername()
        {
            return m_ERUser.Username.ToUpper();  
        }

        /// <summary>
        /// Connection string property
        /// </summary>
        public string Connection
        {
            set 
            {
                m_sConnection = value;
            }    
        }

        /// <summary>
        /// Eat Screen string property
        /// </summary>
        public string EATScreen
        {
            set 
            {
                m_sEATScreen = value;
            }    
        }

        /// <summary>
        /// Submit new order
        /// </summary>
        /// <param name="newOrder">Deatils fo teh order to be placed</param>
        /// <returns></returns>
        public bool SubmitNewOrder(OrderInfo newOrder)
        {
            //add defaults
            MESSAGEFIX3Lib.FIXMsgConstants MsgType = MESSAGEFIX3Lib.FIXMsgConstants.esFIXMsgNewOrderSingle;
            FIXMessage message = newOrder.CreateFixMessage(MsgType);

            if (message == null)
                return false;

            Account account = m_AccountManagement.FindAccount(newOrder.AccountID, message);

            if (account != null)
            {
                account.ApplyDefaults(newOrder.Instrument.SecurityExchange, message);
                return m_Router.SendMsg((EASYROUTERCOMCLIENTLib.IFIXMessage)message);    
            }
            return false;
        }

        /// <summary>
        /// Submit new order
        /// </summary>
        /// <param name="newOrder">Deatils fo teh order to be placed</param>
        /// <returns></returns>
        public bool SubmitNewOrder(OrderInfo newOrder,Hashtable table)
        {
            //add defaults
            MESSAGEFIX3Lib.FIXMsgConstants MsgType = MESSAGEFIX3Lib.FIXMsgConstants.esFIXMsgNewOrderSingle;
            FIXMessage message = newOrder.CreateFixMessage(MsgType);

            if (message == null)
                return false;

            Account account = m_AccountManagement.FindAccount(newOrder.AccountID, message);

            if (account != null)
            {
                account.ApplyDefaults(newOrder.Instrument.SecurityExchange, message);
                if(table.Count > 0)
                    account.ApplyDefaults(newOrder.Instrument.SecurityExchange, message,table);
               
                return m_Router.SendMsg((EASYROUTERCOMCLIENTLib.IFIXMessage)message);
            }
            return false;
        }

         /// <summary>
        /// Submit new order
        /// </summary>
        /// <param name="sTE">ES Ticker Mnemomic (internal ER symbol for instrument</param>
        /// <param name="bBuy">is it a buy or sell</param>
        /// <param name="dPrice">price of order</param>
        /// <param name="nVolume">volume of order</param>
        /// <returns></returns>
        public bool SubmitNewOrder(string sTE, bool bBuy, double? dPrice, int nVolume,  OrderType sOrderType, TimeInForce sTimeInForce)
        {
            /*
                BodyLength=232
                MsgType=NewOrderSingle<D>
                ESTickerMnemonic=3FX:EUR-GBP
                ESTickerSymbol=EUR-GBP
                ESExchange=F
                ESPriceFormatCode=0
                SecurityExchange=3
                PutOrCall=OptionTypePut<0>
                SecurityType=FOR
                Symbol=EUR
                CFICode=MRCXXX
                Currency=EUR
                ESDecimalPlaces=4
                ESUnitTickValue=0.000000
                ESCompositeDelta=0.000000
                ESShortOptAdjust=0.000000
                Side=SideSell<2>
                OpenClose=Open<O>
                ESAccountID=9
                ESAccountName=apj
                HandlInst=2
                ESDefaultFieldSanityCheck=True
                OrdType=OrderTypeLimit<2>
                TimeInForce=TimeInForceImmediateOrCancel<3>
                Price=0.67501
                StopPx=0
                TradingSessionID=2
                OrderQty=1 
             */
            
            OrderInfo order = new OrderInfo(sTE);
            order.Price = dPrice;
            order.OrderQty = nVolume;

            if (bBuy == true)
            { 
                order.Side = MESSAGEFIX3Lib.FIXSideConstants.esFIXSideBuy;
            }
            else
            {
                order.Side = MESSAGEFIX3Lib.FIXSideConstants.esFIXSideSell;
            }

            order.OrderType = sOrderType;
            order.TimeInForce = sTimeInForce;
            return SubmitNewOrder(order);
        }

        /// <summary>
        /// Pull existing order
        /// </summary>
        /// <param name="sPrimaryOrder">Priamry BOID id of the order (internal order ER id)</param>
        /// <returns></returns>
        public bool PullOrder(int nPBOID)
        {
           OrderHistory history = m_OrderManagement.GetOrder(nPBOID);

            if (history != null)
            {
                return PullOrder(history.CurrentOrder);                
            }
            else 
            {
                LogFileWriter.TraceEvent(TraceEventType.Error, "Attempt to pull an order {0} that doesn't exist ", nPBOID.ToString());
                return false;
            }
        }

        /// <summary>
        /// Pulls order specified
        /// </summary>
        /// <param name="order">order info object</param>
        /// <returns></returns>
        public bool PullOrder(OrderInfo order)
        {
            if (order.Instrument == null)
            {
                LogFileWriter.TraceEvent(TraceEventType.Error, "Attempt to pull an order {0} with no structure defined", order.ToString());
                return false;
            }

            //MESSAGEFIX3Lib.FIXMsgConstants.esFIXMsgNewOrderSingle
            FIXMessage message = order.CreateFixMessage(MESSAGEFIX3Lib.FIXMsgConstants.esFIXMsgNewOrderSingle);

            Account account = m_AccountManagement.FindAccount(order.AccountID, message);

            if (account != null)
            {
                account.ApplyDefaults(order.Instrument.SecurityExchange, message);
                message.MsgType = MESSAGEFIX3Lib.FIXMsgConstants.esFIXMsgOrderCancelRequest;
                message.set_AsNumber(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagESVAOState, 0);
                m_Router.SendMsg((EASYROUTERCOMCLIENTLib.IFIXMessage)message);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Pass in the order information to be modified 
        /// </summary>
        /// <param name="order">order info object</param>
        /// <param name="dPrice">new price - enter old price if unchanged</param>
        /// <param name="nVolume">new volume - enter old volume if unchanged</param>
        /// <returns></returns>
        public bool EditOrder(OrderInfo order, double? dPrice, int nVolume)
        {
            bool bRetVal = true;

            switch (order.ExecutionReportStatus)
            {
                case Status.PendingNew:
                case Status.PendingReplace:
                case Status.PendingActivate:
                case Status.PendingCancel:
                case Status.Cancelled:
                case Status.Fill:
                    LogFileWriter.TraceEvent(TraceEventType.Error, "Attempt to edit an order {0} with invalid execution report status {1}", order.ToString(), order.ExecutionReportStatus.ToString());
                    return false;
            }

            if (order.Instrument == null)
            {
                LogFileWriter.TraceEvent(TraceEventType.Error, "Attempt to edit an order {0} with no structure defined", order.ToString());
                return false;
            }

            FIXMessage message = order.CreateFixMessage(MESSAGEFIX3Lib.FIXMsgConstants.esFIXMsgOrderModifyRequest);

            Account account = m_AccountManagement.FindAccount(order.AccountID, message);

            if (account != null)
            {
                account.ApplyDefaults(order.Instrument.SecurityExchange, message);


                message.RemoveGroup(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagOrderQty);

                message.RemoveGroup(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagLeavesQty);

                int nDelta = nVolume - order.LeavesQty;
                int nOrderQty = order.OrderQty + nDelta;

                message.set_AsNumber(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagOrderQty, nOrderQty);
                message.set_AsNumber(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagLeavesQty, nVolume);
                message.set_AsNumber(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagESOrderDeltaQty, nDelta);

                message.RemoveGroup(EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagPrice);
                if (dPrice.HasValue)
                    message.set_AsDouble(MESSAGEFIX3Lib.FIXTagConstants.esFIXTagPrice, dPrice.Value);

                bRetVal = m_Router.SendMsg((EASYROUTERCOMCLIENTLib.IFIXMessage)message);
            }
            else
            {
                bRetVal = false;
            }
            return bRetVal;
        }

        /// <summary>
        /// Returns a sorted list of orders
        /// </summary>        
        public OrderManagement OrderManagement { get { return m_OrderManagement; } }

        /// <summary>
        /// Returns a dictionary of prices
        /// </summary>        
        public PriceManagement PriceManagement { get { return m_PriceManagement; } }
        

        /// <summary>
        /// Get the instrument details info
        /// </summary>
        /// <param name="sTE">ES Ticker Mnenomic (internal ER instrument identifier</param>
        /// <returns></returns>
        public TEInstrument GetInstrument(string sTE)
        {
            TEInstrument instruemnt = m_InstrumentManager.GetDirectTE(sTE);
            return instruemnt; 
        }

        /// <summary>
        /// private db accessor class object
        /// </summary>
        CustomAccess m_CustomAccessor = null;

        /// <summary>
        /// APJ - Allows entry of tick data into a db source
        /// </summary>
        /// <param name="sColumnName"></param>
        /// <param name="dTick"></param>
        /// <returns></returns>
        public bool AddHistoricalData(string sColumnName, double dTick)
        {
            if (m_sDSN != string.Empty)
            {
                if (m_CustomAccessor == null)
                {
                    m_CustomAccessor = new CustomAccess(m_sDSN); 
                    //m_CustomAccessor.DSN = m_sDSN;
                    if (m_CustomAccessor.Open())
                    {
                        //opened
                    }
                    else
                    {
                        m_CustomAccessor = null;
                        return false;
                    }
                }

                m_CustomAccessor.InsertTick(m_ERUser.Username, DateTime.Now, sColumnName, dTick);   
                //m_CustomAccessor.ClearParameters();
                return true;
            }
            return false;
        }

        public void AddPersistedClass(IProfile clsProfile)
        {
            m_PersistedClasses.Add(clsProfile.FormName, clsProfile);
        }

        public void RemovePersistedClass(IProfile clsProfile)
        {
            m_PersistedClasses.Remove(clsProfile.FormName);
        }

        public void WriteProfile(IProfile clsProfile)
        {
            StringBuilder theString = new StringBuilder();
            XmlWriter ProfileSettings = XmlWriter.Create(theString);

            ProfileSettings.WriteStartDocument();
            clsProfile.WriteProperties(ProfileSettings);
            ProfileSettings.WriteEndDocument();
            ProfileSettings.Flush();
            if (!clsProfile.InstanceID.HasValue)
                clsProfile.InstanceID = Environment.TickCount;
            m_Router.SetProfileEntry("DEFAULT", ((int)clsProfile.ScreenID).ToString(), clsProfile.InstanceID.ToString(), theString.ToString());
        }
    }
}
