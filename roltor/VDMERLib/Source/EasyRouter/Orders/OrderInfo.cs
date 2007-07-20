/*
** Order.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** Order information class 
**
** Changes
** -------
**
*/

using System;
using System.Collections.Generic;
using System.Text;

using VDMERLib.EasyRouter.Structure;
using MESSAGEFIX3Lib;
using VDMERLib.EasyRouter;
using System.Windows.Forms;
using VDMIFIXMessage = EASYROUTERCOMCLIENTLib.IFIXMessage;

namespace VDMERLib.EasyRouter.Orders
{
    public enum Status
    {
        Activated = 'c',
        Calculated = 'B',
        Cancelled = '4',
        DeferLegFill = 'X',
        DoneForDay = '3',
        Expired = 'C',
        Fill = '2',
        Flushed = 'Z',
        ImportNew = 'd',
        New = '0',
        OrderStatus = 'I',
        PartialFill = '1',
        PendingActivate = 'a',
        PendingCancel = '6',
        PendingNew = 'A',
        PendingReplace = 'E',
        Rejected = '8',
        RejectedActivate = 'b',
        Replaced = '5',
        Restated = 'D',
        Stop = '7',
        Suspended = '9',
        Trade = 'F',
        TradeCancel = 'H',
        TradeCorrect = 'G',
        Triggered = 'Y',
        Unknown = '?'
    }

    /// <summary>
    /// Class OrderInfo
    /// </summary>
    public class OrderInfo : OrderDataEventArg
    {
        /// <summary>
        /// Instrument details for this order
        /// </summary>
        protected TEInstrument m_instrument = null;

        /// <summary>
        /// Instrument details for this order
        /// </summary>
        public TEInstrument Instrument 
        {
            get {
                if (m_instrument == null)
                {
                    VDMERLib.EasyRouter.EasyRouterClient.ERCSClient objclient = VDMERLib.EasyRouter.EasyRouterClient.ERCSClient.GetInstance();
                    if (objclient!=null)
                        m_instrument = objclient.GetInstrument(m_sTickerMnemonic);
                }
                return m_instrument; } 
            set { m_instrument = value; } 
        }

        /// <summary>
        /// price of order
        /// </summary>
        protected double? m_dPrice = null;

        protected double? m_dTotalFilledPrice = null;

        /// <summary>
        /// Average Price of fills
        /// </summary>
        public double? AvgPx
        {
          get { return m_dTotalFilledPrice/m_nCumQty; }
        }

        /// <summary>
        /// price of order
        /// </summary>
        public double? Price
        {
            get { { return m_dPrice; } }
            set { { m_dPrice = value; } }
        }

        /// <summary>
        /// total order quantity
        /// </summary>
        protected int m_nOrderQty = 0;

        /// <summary>
        /// total order quantity
        /// </summary>
        public int OrderQty
        {
            get { { return m_nOrderQty; } }
            set { { m_nOrderQty = value; } }
        }

        private OrderType m_sOrderType;

        public bool IsNewOrder
        {
            get { { return m_nPrimaryBOID == 0; } }
        }
        /// <summary>
        /// Order Type
        /// </summary>
        ///
        public OrderType OrderType
        {
            get { return m_sOrderType; }
            set { m_sOrderType = value; }
        }

        private string GetFIXOrderType()
        {
            switch (m_sOrderType)
            {
                case OrderType.Limit: return FIXOrderTypeConstants.esFIXOrderTypeLimit;
                case OrderType.Market: return FIXOrderTypeConstants.esFIXOrderTypeMarket;
                default: return FIXOrderTypeConstants.esFIXOrderTypeUnknown;
            }
        }

        private bool SetFIXOrderType(string strOrderType)
        {
            switch (strOrderType)
            {
                case FIXOrderTypeConstants.esFIXOrderTypeLimit: m_sOrderType = OrderType.Limit; return true;
                case FIXOrderTypeConstants.esFIXOrderTypeMarket: m_sOrderType = OrderType.Market; return true;
                default: return false;
            }
        }

        private TimeInForce m_sTimeInForce;

        /// <summary>
        /// Time in Force
        /// </summary>
        public TimeInForce TimeInForce
        {
            get { return m_sTimeInForce; }
            set { m_sTimeInForce = value; }
        }
     
        private string GetFIXTimeInForce()
        {
            switch (m_sTimeInForce)
            {
                case TimeInForce.GoodTillCancelled: return FIXTimeInForceConstants.esFIXTimeInForceGoodTillCancel;
                case TimeInForce.StandardOrders: return FIXTimeInForceConstants.esFIXTimeInForceDay;
                case TimeInForce.FillOrKill: return FIXTimeInForceConstants.esFIXTimeInForceFillOrKill;
                default: return FIXTimeInForceConstants.esFIXTimeInForceUnknown;
            }
        }

        private bool SetFIXTimeInForce(string strTimeInForce)
        {
            switch (strTimeInForce)
            {
                case FIXTimeInForceConstants.esFIXTimeInForceGoodTillCancel: m_sTimeInForce = TimeInForce.GoodTillCancelled; return true;
                case FIXTimeInForceConstants.esFIXTimeInForceDay: m_sTimeInForce = TimeInForce.StandardOrders; return true;
                case FIXTimeInForceConstants.esFIXTimeInForceFillOrKill: m_sTimeInForce = TimeInForce.FillOrKill; return true;
                default: return false;
            }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="instrument"></param>
        /// <param name="sTickerMnemonic"></param>
        public OrderInfo(string sTickerMnemonic)
        {
            m_sTickerMnemonic = sTickerMnemonic;
        }

        /// <summary>
        /// ES Ticker mnemonic
        /// </summary>
        protected string m_sTickerMnemonic = string.Empty;

        /// <summary>
        /// ES Ticker mnemonic
        /// </summary>
        public string TickerMnemonic
        {
            get { { return m_sTickerMnemonic; } }
            set { { m_sTickerMnemonic = value; } }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="order"></param>
        public OrderInfo(OrderInfo order)
        {
            this.m_nLeavesQty = order.m_nLeavesQty;
            this.m_nOrderQty = order.m_nOrderQty;
            this.m_dPrice = order.m_dPrice;
            this.m_nSecondBOID = order.m_nSecondBOID;
            this.m_sExecutionReportStatus = order.m_sExecutionReportStatus;
            this.m_TransactTime = order.m_TransactTime;
            this.m_instrument = order.m_instrument;
            this.m_nCumQty = order.m_nCumQty;
            this.m_sClOrdID = order.m_sClOrdID;
            this.m_sSide = order.m_sSide;
            this.m_sExecutionReportStatus = order.m_sExecutionReportStatus;
            this.m_sTickerMnemonic = order.m_sTickerMnemonic;
            this.m_sTimeInForce = order.m_sTimeInForce;
            this.m_sOrderType = order.m_sOrderType;
            this.PrimaryBOID = order.PrimaryBOID;
            this.m_iAccountID = order.m_iAccountID;
        }

        //TODO: [16/04/07] YKC - Hack as i donot care about order types etc

        /// <summary>
        /// Generate a fix message
        /// </summary>
        /// <param name="MsgType"></param>
        /// <returns></returns>
        public FIXMessage CreateFixMessage(MESSAGEFIX3Lib.FIXMsgConstants MsgType)
        {
            FIXMessage message = new FIXMessage();
            if (Instrument != null)
            {
                message.MsgType = MsgType;
                Instrument.ApplyOrderDetails(message);

                AddConstants(message);
                //Add order details 
                if (m_dPrice.HasValue)
                    message.set_AsDouble(FIXTagConstants.esFIXTagPrice, m_dPrice.Value);
                message.set_AsNumber(FIXTagConstants.esFIXTagOrderQty, (int)m_nOrderQty);

                message.set_AsString(FIXTagConstants.esFIXTagSide, m_sSide);
                message.set_AsString(FIXTagConstants.esFIXTagTimeInForce, GetFIXTimeInForce());
                message.set_AsString(FIXTagConstants.esFIXTagOrdType, GetFIXOrderType());
                if (m_iAccountID.HasValue)
                    message.set_AsNumber(FIXTagConstants.esFIXTagESAccountID, (int)m_iAccountID.Value);

                return message;
            }
            return null;
        }

        //to do
        /// <summary>
        /// Basic defaults that are hard coded and not required
        /// </summary>
        /// <param name="message"></param>
        private void AddConstants(FIXMessage message)
        {
            //cannot be arsed as eurex does not care for this fields :) - TO DO
            message.set_AsString(FIXTagConstants.esFIXTagOpenClose, MESSAGEFIX3Lib.FIXOpenCloseConstants.esFIXOpen);
            message.set_AsNumber(FIXTagConstants.esFIXTagStopPx, 0);
            //Confusion in eat
            message.set_AsString(FIXTagConstants.esFIXTagTradingSessionID, MESSAGEFIX3Lib.FIXSessionConstants.esFIXSessionNormal);

            if(m_nPrimaryBOID != 0)
            {
                message.set_AsNumber(FIXTagConstants.esFIXTagESBOIDPrimary,m_nPrimaryBOID);     
            }
        }

        /// <summary>
        /// ES Primary BOID - ID for order
        /// </summary>
        protected int m_nPrimaryBOID = 0;

        /// <summary>
        /// ES Primary BOID - ID for order
        /// </summary>
        public int PrimaryBOID
        {
            get { return m_nPrimaryBOID; }
            set { m_nPrimaryBOID = value; }
        }

        /// <summary>
        /// ES Secondary BOID - ID for order
        /// </summary>
        protected int m_nSecondBOID = 0;

        /// <summary>
        /// ES Secondary BOID - ID for order
        /// </summary>
        public int SecondBOID
        {
            get { return m_nSecondBOID; }
            set { m_nSecondBOID = value; }
        }
        
        /// <summary>
        /// remaining order quantity
        /// </summary>
        protected int m_nLeavesQty = 0;

        /// <summary>
        /// remaining order quantity
        /// </summary>
        public int LeavesQty
        {
            get { return m_nLeavesQty; }
            set { m_nLeavesQty = value; }
        }

        /// <summary>
        /// total traded quantity
        /// </summary>
        protected int m_nCumQty = 0;

        /// <summary>
        /// total traded quantity
        /// </summary>
        public int CumQty
        {
            get { return m_nCumQty; }
            set { m_nCumQty = value; }
        }

        /// <summary>
        /// ES FIX CLORID
        /// </summary>
        protected string m_sClOrdID = string.Empty;

        /// <summary>
        /// ES FIX CLORID
        /// </summary>
        public string ClOrdID
        {
            get { return m_sClOrdID; }
            set { m_sClOrdID = value; }
        }

        protected long? m_iAccountID = null;

        /// <summary>
        /// AccountID
        /// </summary>
        /// 
        public long? AccountID
        {
            get { return m_iAccountID; }
            set { m_iAccountID = value; }
        }

        /// <summary>
        /// Host Order ID
        /// </summary>
        protected string m_sOrdID = string.Empty;

        /// <summary>
        /// Host Order ID
        /// </summary>
        public string OrdID
        {
            get { return m_sOrdID; }
            set { m_sOrdID = value; }
        }

        /// <summary>
        /// Status type for the order 
        /// </summary>
        protected Status m_sExecutionReportStatus = Status.Unknown;

       

        /// <summary>
        /// Status type for the order 
        /// </summary>
        virtual public Status ExecutionReportStatus
        {
            get { return m_sExecutionReportStatus; }
            set { m_sExecutionReportStatus = value; }
        }
        
        /// <summary>
        /// Decription of side from MESSAGEFIX3Lib.FIXSideConstants
        /// </summary>
        public string m_sSide = MESSAGEFIX3Lib.FIXSideConstants.esFIXSideUnknown;

        /// <summary>
        /// Decription of side from MESSAGEFIX3Lib.FIXSideConstants
        /// </summary>
        public string Side
        {
            get { return m_sSide; }
            set { m_sSide = value; }
        }

        /// <summary>
        /// Only valid if buy and sell are only options
        /// </summary>
        public bool IsBuy
        {
            get { return m_sSide == MESSAGEFIX3Lib.FIXSideConstants.esFIXSideBuy; }
            set { if (value == true)
                    m_sSide = MESSAGEFIX3Lib.FIXSideConstants.esFIXSideBuy; 
                  else  
                    m_sSide = MESSAGEFIX3Lib.FIXSideConstants.esFIXSideSell; }
        }

        /// <summary>
        /// Only valid if buy and sell are only option
        /// </summary>
        public bool IsSell
        {
            get { return m_sSide == MESSAGEFIX3Lib.FIXSideConstants.esFIXSideSell; }
            set
            {
                if (value == true)
                    m_sSide = MESSAGEFIX3Lib.FIXSideConstants.esFIXSideSell;
                else
                    m_sSide = MESSAGEFIX3Lib.FIXSideConstants.esFIXSideBuy;
            }
        }

        /// <summary>
        /// Time of transaction
        /// </summary>
        protected DateTime m_TransactTime;

        /// <summary>
        /// Time of transaction
        /// </summary>
        public DateTime TransactTime
        {
            get { return m_TransactTime; }
            set { m_TransactTime = value; }
        }

        /// <summary>
        /// Apply changes from another Order
        /// </summary>
        /// <param name="order"></param>
        public void ApplyChanges(OrderInfo order)
        {
            this.m_nLeavesQty = order.m_nLeavesQty;
            this.m_nOrderQty = order.m_nOrderQty;
            this.m_nCumQty = order.m_nCumQty;
            if (this.m_dTotalFilledPrice.HasValue)
                this.m_dTotalFilledPrice += order.m_dTotalFilledPrice;
            else
                this.m_dTotalFilledPrice = order.m_dTotalFilledPrice;

            if (order.m_dPrice.HasValue)
                this.m_dPrice = order.m_dPrice;
            else
                order.m_dPrice = this.Price; 
            this.m_nSecondBOID = order.m_nSecondBOID;
            
            this.m_sExecutionReportStatus = order.m_sExecutionReportStatus;
            
            if (order.m_TransactTime != null)
                this.m_TransactTime = order.m_TransactTime;
            else
                order.m_TransactTime = this.m_TransactTime;
        }

        /// <summary>
        /// Parse the FIX message for order details
        /// </summary>
        /// <param name="FIXMsg"></param>
        /// <returns></returns>
        public virtual bool ParseFix(VDMIFIXMessage FIXMsg)
        {
            bool bSuccess = true;

            try
            {
                SetPrice(FIXMsg);

                int nOrderQty = 0;
                if (FIXMsg.GetNumber(out nOrderQty, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagOrderQty))
                    m_nOrderQty = nOrderQty;

                DateTime TransactTime;
                if (FIXMsg.GetTime(out TransactTime, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagTransactTime))
                    m_TransactTime = TransactTime;

               
                string sClOrdID = string.Empty;
                if (FIXMsg.GetString(out sClOrdID, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagClOrdID))
                    m_sClOrdID = sClOrdID;

                string sOrdID = string.Empty;
                if (FIXMsg.GetString(out sOrdID, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagOrderID))
                    m_sOrdID = sOrdID;
                
                int nPrimaryBOID = 0;
                if (FIXMsg.GetNumber(out nPrimaryBOID, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESBOIDPrimary))
                    m_nPrimaryBOID = nPrimaryBOID;
#if DEBUG
                //if (nPrimaryBOID != 366)
                //    return false;
#endif

                int nSecondBOID = 0;
                if (FIXMsg.GetNumber(out nSecondBOID, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESBOIDSecondary))
                    m_nSecondBOID = nSecondBOID;

                int nLeavesQty = 0;
                if (FIXMsg.GetNumber(out nLeavesQty, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagLeavesQty))
                    m_nLeavesQty = nLeavesQty;

                int nCumQty = 0;
                if (FIXMsg.GetNumber(out nCumQty, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagCumQty))
                    m_nCumQty = nCumQty;

                int AccountID = 0;
                if (FIXMsg.GetNumber(out AccountID, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESAccountID))
                    m_iAccountID = AccountID;

                string sSide = string.Empty;
                if (FIXMsg.GetString(out sSide, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagSide))
                    m_sSide = sSide;

                string sTimeInForce = String.Empty;
                if (FIXMsg.GetString(out sTimeInForce, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagTimeInForce))
                    SetFIXTimeInForce(sTimeInForce);

                string sOrderType = String.Empty;
                if (FIXMsg.GetString(out sOrderType, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagOrdType))
                    SetFIXOrderType(sOrderType);
                
                SetStatus(FIXMsg);
            }
            catch (Exception)
            {
                bSuccess = false;
            }

            return bSuccess;
        }

        /// <summary>
        /// Set the order price
        /// </summary>
        /// <param name="FIXMsg"></param>
        public virtual void SetPrice(VDMIFIXMessage FIXMsg)
        {
            double dPrice = 0;
            if (FIXMsg.GetDouble(out dPrice, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagPrice))
                m_dPrice = dPrice;
        }

        /// <summary>
        /// set order status - exec type
        /// </summary>
        /// <param name="FIXMsg"></param>
        public virtual void SetStatus(VDMIFIXMessage FIXMsg)
        {
            string sStatus;
            if (FIXMsg.GetString(out sStatus, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagExecType))
                m_sExecutionReportStatus = (Status)sStatus[0];

            else if (FIXMsg.MsgType == EASYROUTERCOMCLIENTLib.FIXMsgConstants.esFIXMsgOrderCancelReject)
                m_sExecutionReportStatus = Status.Rejected;
        }

        /// <summary>
        /// Order details 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("TYPE[{0:15}] PBOID[{1:10}]SBOID[{2:10}] TE[{3:20}] PRICE[{4}] ORDERQTY[{5}] LEAVESQTY[{6}] CUMQTY[{7}]",GetExecType(m_sExecutionReportStatus),m_nPrimaryBOID,m_nSecondBOID,m_sTickerMnemonic ,m_dPrice, m_nOrderQty, m_nLeavesQty, m_nCumQty); 
        }

       
        /// <summary>
        /// Converts Exec Type to friendly description
        /// </summary>
        /// <param name="sExecType"></param>
        /// <returns></returns>
        public string GetExecType(Status sExecType)
        {
            switch (sExecType)
            {

                case Status.Activated:
                    return "Activated";
                case Status.Calculated:
                    return "Calculated";
                case Status.Cancelled:
                    return "Cancelled";
                case Status.DeferLegFill:
                    return "DeferLegFill";
                case Status.DoneForDay:
                    return "DoneForDay";
                case Status.Expired:
                    return "Expired";
                case Status.Fill:
                    return "Fill";
                case Status.Flushed:
                    return "Flushed";
                case Status.ImportNew:
                    return "ImportNew";
                case Status.New:
                    return "New";
                case Status.OrderStatus:
                    return "OrderStatus";
                case Status.PartialFill:
                    return "PartialFill";
                case Status.PendingActivate:
                    return "PendingActivate";
                case Status.PendingCancel:
                    return "PendingCancel";
                case Status.PendingNew:
                    return "PendingNew";
                case Status.PendingReplace:
                    return "PendingReplace";
                case Status.Rejected:
                    return "Rejected";
                case Status.RejectedActivate:
                    return "RejectedActivate";
                case Status.Replaced:
                    return "Replaced";
                case Status.Restated:
                    return "Restated";
                case Status.Stop:
                    return "Stop";
                case Status.Suspended:
                    return "Suspended";
                case Status.Trade:
                    return "Trade";
                case Status.TradeCancel:
                    return "TradeCancel";
                case Status.TradeCorrect:
                    return "TradeCorrect";
                case Status.Triggered:
                    return "Triggered";
                default:
                    return "Unknown";
            }
                
        }
    }
}

