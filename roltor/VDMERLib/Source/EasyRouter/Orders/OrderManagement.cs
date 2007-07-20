/*
** OrderManagement.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** Handle all order / trade messages
**
** Changes
** -------
**
*/

using System;
using System.Collections.Generic;
using System.Text;

using System.Collections;
using VDMERLib.EasyRouter.General;
using System.Diagnostics;
using VDMERLib.EasyRouter.Structure;

using VDMIFIXMessage = EASYROUTERCOMCLIENTLib.IFIXMessage;
using VDMFIXTagConstants = EASYROUTERCOMCLIENTLib.FIXTagConstants;
using VDMFIXExecutionReportConstants = MESSAGEFIX3Lib.FIXExecutionReportConstants;

namespace VDMERLib.EasyRouter.Orders 
{
    /// <summary>
    /// Class OrderManagement
    /// </summary>
    public class OrderManagement : SortedDictionary<int, OrderHistory>
    {
        /// <summary>
        /// Access to instrument data
        /// </summary>
        InstrumentManager m_InstrumentManager = null;

        /// <summary>
        /// Stores primary boid against OrderHistory
        /// </summary>
        
        OrdersAtPrice m_BuyOrdersAtPrice = new OrdersAtPrice();
        OrdersAtPrice m_SellOrdersAtPrice = new OrdersAtPrice();

        /// <summary>
        /// Vanilla
        /// </summary>
        /// <param name="instrumentMamager"></param>
        public OrderManagement(InstrumentManager instrumentManager)
            : base(new DescendingComparer())
        {
            m_InstrumentManager = instrumentManager;
            
        }

        public class DescendingComparer : System.Collections.Generic.IComparer<int>
        {

            public int Compare(int x, int y)
            {
                return x.CompareTo(y)*-1;
            }

        }

        /// <summary>
        /// Get all OrderHistory for specific boid
        /// </summary>
        /// <param name="nKey"></param>
        /// <returns></returns>
        public OrderHistory GetOrder(int nKey)
        {
            if (this.ContainsKey(nKey))
                return (OrderHistory)this[nKey];
            return null;
        }

        /// <summary>
        /// Process FIX message for order or trade history
        /// </summary>
        /// <param name="FIXMsg"></param>
        /// <returns></returns>
        internal OrderInfo ProcessExecutionReport(VDMIFIXMessage FIXMsg)
        {
            OrderInfo order = null;

            int nPBOID = 0;
            int nSBOID = 0;
            string sExecType = string.Empty;

            GetMessageInfo(FIXMsg, out nPBOID, out nSBOID, out sExecType);

            if (nPBOID == 0)
            {
                LogFileWriter.TraceEvent(TraceEventType.Error, "No BOID in message when processing execution report");
                return null;
            }

            string sESTickerMnemonic = string.Empty;

            if (!FIXMsg.GetString(out sESTickerMnemonic, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESTickerMnemonic))
            {
                LogFileWriter.TraceEvent(TraceEventType.Error, "No Ticker Mnemonic in message when processing execution report");
                return null;
            }
                    
            //process
            if ((sExecType == VDMFIXExecutionReportConstants.esFIXExecutionReportFill) ||
               (sExecType == VDMFIXExecutionReportConstants.esFIXExecutionReportPartialFill))
            {
                order = new TradeInfo(sESTickerMnemonic);
            }
            else
            {
                order = new OrderInfo(sESTickerMnemonic);
            }

            if (!order.ParseFix(FIXMsg))
            {
                LogFileWriter.TraceEvent(TraceEventType.Error, "Failed to parse execution report");
                return null;
            }

            OrderHistory history = null;
            double? PreEditPrice = null;
            if (this.ContainsKey(order.PrimaryBOID))
            { 
                history = (OrderHistory)this[order.PrimaryBOID];
                PreEditPrice = history.CurrentOrder.Price;
                history.UpdateOrder(order);
            }
            else
            {
                history = new OrderHistory(order);
                if (history.CurrentOrder != null)
                {
                    this[history.Key] = history;
                }
            }

            OrdersAtPrice orders = order.IsBuy ? m_BuyOrdersAtPrice : m_SellOrdersAtPrice;
            if (order.Price.HasValue)
            {
                if (order.LeavesQty == 0)
                    orders.Remove(order.Price.Value, order);
                else if (order.ExecutionReportStatus == Status.Replaced && PreEditPrice.HasValue && PreEditPrice != order.Price)
                {
                    orders.Remove(PreEditPrice.Value, order);
                    orders.Add(order.Price.Value, order);
                }
                else
                    orders.Add(order.Price.Value, order);
            }

            return history.CurrentOrder;
        }

        public bool IsMyOrderAtPrice(double price, bool isBuy)
        {
            OrdersAtPrice orders = isBuy ? m_BuyOrdersAtPrice : m_SellOrdersAtPrice;
            return orders.ContainsKey(price);
        }

        public OrderInfo GetOrderAtPrice(double price, bool isBuy, int orderPosition)
        {
            OrdersAtPrice ordersatprice = isBuy ? m_BuyOrdersAtPrice : m_SellOrdersAtPrice;
            Dictionary<int, OrderInfo> orders = ordersatprice[price];
            if (orders == null)
                return null;
            else
            {
                int i = 0;
                foreach (OrderInfo order in orders.Values)
                {
                    if (i == orderPosition)
                        return order;
                    i++;
                }
                return null;
            }
        }

        /// <summary>
        /// to change - same as the above
        /// </summary>
        /// <param name="FIXMsg"></param>
        /// <returns></returns>
        internal OrderInfo ProcessOrderCancelReject(VDMIFIXMessage FIXMsg)
        {
            OrderInfo order = null;
                           
            int nPBOID = 0;
            int nSBOID = 0;
            string sExecType = string.Empty;

            GetMessageInfo(FIXMsg, out nPBOID, out nSBOID, out sExecType);

            if (nPBOID > 0)
            {
                //if (nPBOID != 447) return null;

                string sESTickerMnemonic = string.Empty;

                if (FIXMsg.GetString(out sESTickerMnemonic, EASYROUTERCOMCLIENTLib.FIXTagConstants.esFIXTagESTickerMnemonic))
                {
                    if (sESTickerMnemonic.Length > 0)
                    {
                        
                        //if (instrument != null)
                        {
                            //process
                            if ((sExecType == VDMFIXExecutionReportConstants.esFIXExecutionReportFill) ||
                               (sExecType == VDMFIXExecutionReportConstants.esFIXExecutionReportPartialFill))
                            {
                                order = new TradeInfo(sESTickerMnemonic);
                            }
                            else
                            {
                                order = new OrderInfo(sESTickerMnemonic);
                            }

                            if (order.ParseFix(FIXMsg))
                            {
                                if (this.ContainsKey(order.PrimaryBOID))
                                {
                                    OrderHistory history = (OrderHistory)this[order.PrimaryBOID];
                                    history.UpdateOrder(order); 
                                }
                                else
                                {
                                    OrderHistory history = new OrderHistory(order);
                                    if (history.CurrentOrder != null)
                                    {
                                        this[history.Key] = history;
                                    }
                                }
                            }
                        }

                    }
                }
            }
            return order;
        }

        /// <summary>
        /// Get Boid and exec type for FIX Order/trade message
        /// </summary>
        /// <param name="FIXMsg"></param>
        /// <param name="nPBOID"></param>
        /// <param name="nSBOID"></param>
        /// <param name="sExecType"></param>
        /// <returns></returns>
        internal bool GetMessageInfo(EASYROUTERCOMCLIENTLib.IFIXMessage FIXMsg,out int nPBOID,out int nSBOID,out string sExecType)
        {
            if (FIXMsg.HasGroup(VDMFIXTagConstants.esFIXTagExecType))
            {
                sExecType = FIXMsg.get_AsString(VDMFIXTagConstants.esFIXTagExecType);
            }
            else
            {
                sExecType = String.Empty;  
            }

            if(FIXMsg.HasGroup(VDMFIXTagConstants.esFIXTagESBOIDPrimary))
            {
                nPBOID = FIXMsg.get_AsNumber(VDMFIXTagConstants.esFIXTagESBOIDPrimary); 
            }
            else
            {
                nPBOID = 0;
            }

            if(FIXMsg.HasGroup(VDMFIXTagConstants.esFIXTagESBOIDSecondary))
            {
                nSBOID = FIXMsg.get_AsNumber(VDMFIXTagConstants.esFIXTagESBOIDSecondary); 
            }
            else
            {
                nSBOID = 0;
            }

            bool bISPending = false;
 
            if(((FIXMsg.MsgType == EASYROUTERCOMCLIENTLib.FIXMsgConstants.esFIXMsgExecutionReport) && (nSBOID <= 0))
                && ((sExecType == VDMFIXExecutionReportConstants.esFIXExecutionReportPendingNew) ||
                    (sExecType == VDMFIXExecutionReportConstants.esFIXExecutionReportPendingCancel) ||
                    (sExecType == VDMFIXExecutionReportConstants.esFIXExecutionReportPendingReplace)))
            {
                bISPending = true;
            }
            return bISPending;
        }

        /// <summary>
        /// Gets the underlying current Order Information for apecific boid
        /// </summary>
        /// <param name="nPBOID"></param>
        /// <returns></returns>
        public OrderInfo GetCurrentOrder(int nPBOID)
        {
            if (this.ContainsKey(nPBOID))
            {
                OrderHistory history = (OrderHistory)this[nPBOID];
                if (history != null)
                {
                    return history.CurrentOrder;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the underlying Order History Information for specific boid
        /// </summary>
        /// <param name="nPBOID"></param>
        /// <returns></returns>
        public OrderHistory GetOrderHistory(int nPBOID)
        {
            if (this.ContainsKey(nPBOID))
                return (OrderHistory)this[nPBOID];
            else
                return null;
        }
    }

    
}
