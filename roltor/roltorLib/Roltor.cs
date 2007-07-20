using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using VDMERLib.EasyRouter.EasyRouterClient;
using VDMERLib.EasyRouter.Orders;
using VDMERLib.EasyRouter.Prices;
using VDMERLib.EasyRouter.General;

namespace RoltorLib
{
    public struct OrderStruct
    {
        private int iOrderID;
        private int iAccountID;
        private int iContractID;
        private int iCurrencyID;
        private int iExchangeID;
        private int iState;
        private bool bIsBid;
        private int iQty;
        private int iQtyOpen;        
        private float fPrice;
        private string sText;

        public int OrderID { get { return iOrderID; } set { iOrderID = value; } }
        public int AccountID { get { return iAccountID; } set { iAccountID = value; } }
        public int ContractID { get { return iContractID; } set { iContractID = value; } }
        public int CurrencyID { get { return iCurrencyID; } set { iCurrencyID = value; } }
        public int ExchangeID { get { return iExchangeID; } set { iExchangeID = value; } }
        public int State { get { return iState; } set { iState = value; } }
        public bool IsBid { get { return bIsBid; } set { bIsBid = value; } }
        public int Qty { get { return iQty; } set { iQty = value; } }
        public int QtyOpen { get { return iQtyOpen; } set { iQtyOpen = value; } }
        public float Price { get { return fPrice; } set { fPrice = value; } }
        public string Text { get { return sText; } set { sText = value; } }

    }

    public class Roltor
    {
        public delegate bool BoolFunction(OrderStruct order);

        /// <summary>
        /// Call Master thread object to execute oneself - self masterbation with aid
        /// </summary>
        IExecute m_Executor;

        /// <summary>
        /// Reference to ER Trading system
        /// </summary>
        ERCSClient m_Client = null;

        /// <summary>
        /// 
        /// </summary>
        Dictionary<int,OrderStruct> dicOrders = new Dictionary<int,OrderStruct>();

        private RoltorRTDtcp rtdApi;

        public Roltor()
        {
            this.rtdApi = new RoltorLib.RoltorRTDtcp(this);
        }

        public Roltor(IExecute executor)
        {
            this.rtdApi = new RoltorLib.RoltorRTDtcp(this);
            m_Executor = executor;
            m_Client = ERCSClient.GetInstance();
            //we will chose Instruments 
            m_Client.EnableAutoSubscription = false;
            m_Client.EnablePriceSubscriptionToTEonStructuredEvent = true;

            m_Client.RecvOrderMsgEvent += new ERCSClient.RecvOrderMsg(m_Client_RecvOrderMsgEvent);
            m_Client.RecvPriceMsgEvent += new ERCSClient.RecvPriceMsg(m_Client_RecvPriceMsgEvent);
            m_Client.RecvGeneralMsgEvent += new ERCSClient.RecvGeneralMsg(m_Client_RecvGeneralMsgEvent);  
        }

        void m_Client_RecvGeneralMsgEvent(object from, VDMERLib.EasyRouter.General.GeneralMsgEventArg args)
        {
            if (args.DataType == GeneralMsgEventArg.GeneralDataType.Login)
            {
                Logon LoginInfo = (Logon)args;
                System.Diagnostics.Debug.WriteLine("Logged onto OLT!!!");
            }
        }

        void m_Client_RecvPriceMsgEvent(object from, VDMERLib.EasyRouter.Prices.PricesEventArg args)
        {
            if (args.DataType == VDMERLib.EasyRouter.Prices.PricesEventArg.PriceDataType.TradeData)
            {
                TradeData PriceInfo = (TradeData)args;
            }
        }

        void m_Client_RecvOrderMsgEvent(object from, VDMERLib.EasyRouter.Orders.OrderDataEventArg args)
        {
            if (args.DataType == VDMERLib.EasyRouter.Orders.OrderDataEventArg.OrderDataType.Order)
            {
                OrderInfo OrderInfo = (OrderInfo) args;                
            }
        }

        /*
         * Initialize connections to RTD and OLT
         */
        public bool Connect()
        {
            //rtdApi.ConnectRTDapi();
            //rtdApi.Login();
            m_Client.Logon("ying", "ying", "vdm44olt1", true);
            return true;
        }

        /*
         * Request order updates from RTD and start processing orders
         */
        public bool Start()
        {
            //rtdApi.SendRequest();
            return true;
        }

        /*
         * Stop order updates from RTD
         */
        public bool Stop()
        {
            //rtdApi.StopRequest();
            return true;
        }

        /*
         * Disconnect from RTD and OLT
         */
        public bool Close()
        {
            //rtdApi.CloseRTDapi();
            m_Client.Logoff();
            return true;
        }


        /*
         * Decion process for orders coming from RTD
         */

        /// <summary>
        /// Update Order
        /// </summary>
        /// <param name="iRTDOrderId">RTS Order Id</param>
        /// <param name="order"></param>
        public void UpdateOrder(int iRTDOrderId, OrderStruct order)
        {
            string strMessage = "";

            // Order is Active
            if(order.State == 1)
            {
                // is this the first time we have seen this order?
                if (!dicOrders.ContainsKey(iRTDOrderId))
                {
                    dicOrders[iRTDOrderId] = order;

                    // VoltAddOrder(iRTDOrderId);
                    m_Executor.PlaceOrder(order);


                    strMessage = string.Format("RTD->OLT : {0,-6} : {1} : {2,8} of {3,8} @ {4,8}",
                        "ADD",
                        iRTDOrderId,
                        order.Qty,
                        order.ContractID,
                        order.Price);
                    System.Diagnostics.Debug.WriteLine(strMessage);
                    System.Diagnostics.Debug.WriteLine("isBid -> " + order.IsBid);

                   
                }
                else
                {
                    // Has order quantity increased?
                    if (order.QtyOpen > dicOrders[iRTDOrderId].QtyOpen)
                    {
                        // VoltRemoveOrder(iRTDOrderId);
                        strMessage = "REMOVE " + iRTDOrderId + " FROM CHIX";
                        strMessage = string.Format("RTD->OLT : {0,-6} : {1} : {2,8} of {3,8} @ {4,8}",
                            "REMOVE",
                            iRTDOrderId,
                            order.Qty,
                            order.ContractID,
                            order.Price);
                        System.Diagnostics.Debug.WriteLine(strMessage);
                        System.Diagnostics.Debug.WriteLine("isBid -> " + order.IsBid);
                        
                        dicOrders[iRTDOrderId] = order;

                        // VoltAddOrder(iRTDOrderId);
                        strMessage = string.Format("RTD->OLT : {0,-6} : {1} : {2,8} of {3,8} @ {4,8}",
                            "ADD",
                            iRTDOrderId,
                            order.Qty,
                            order.ContractID,
                            order.Price);
                        System.Diagnostics.Debug.WriteLine(strMessage);
                        System.Diagnostics.Debug.WriteLine("isBid -> " + order.IsBid);
                    }                       
                    // must just be a fill, price change or quantity reduction
                    else if (order.QtyOpen != dicOrders[iRTDOrderId].QtyOpen
                        || order.Price != dicOrders[iRTDOrderId].Price)
                    {
                        dicOrders[iRTDOrderId] = order;

                        // VoltChangeOrder(iRTDOrderId);
                        strMessage = string.Format("RTD->OLT : {0,-6} : {1} : {2,8} of {3,8} @ {4,8}",
                            "CHANGE",
                            iRTDOrderId,
                            order.Qty,
                            order.ContractID,
                            order.Price);
                        System.Diagnostics.Debug.WriteLine(strMessage);
                        System.Diagnostics.Debug.WriteLine("isBid -> " + order.IsBid);
                    }
                    // if nothing changed then do nothing.
                }
                
            }
            // Order is in any other state other than active
            else 
            {
                // if we have an order in our dictionary then we should pull the order and remove from dictionary
                if (dicOrders.ContainsKey(iRTDOrderId))
                {
                    strMessage = string.Format("RTD->OLT : {0,-6} : {1} : {2,8} of {3,8} @ {4,8}",
                        "REMOVE",
                        iRTDOrderId,
                        order.Qty,
                        order.ContractID,
                        order.Price);
                    System.Diagnostics.Debug.WriteLine(strMessage);
                    System.Diagnostics.Debug.WriteLine("isBid -> " + order.IsBid);
                    dicOrders.Remove(iRTDOrderId);
                }
            }
            /*
                strMessage = " OrderState=" + order.State;
                strMessage += " Price=" + order.Price;
                strMessage += " Qty=" + order.Qty;
                strMessage += " OpenQty=" + order.QtyOpen;
                strMessage += " Contract=" + order.ContractID;
                strMessage += " Account=" + order.Text;

                System.Diagnostics.Debug.WriteLine(strMessage);
           */
        }


        public bool AddOrderOlt(OrderStruct order)
        {
            m_Client.SubscribeTE("");
            OrderInfo OrderInfo = new OrderInfo("BLAH");
            OrderInfo.Price = 0.1;
            OrderInfo.OrderQty = 1000000;
            OrderInfo.TimeInForce = VDMERLib.EasyRouter.TimeInForce.StandardOrders;
            OrderInfo.OrderType = VDMERLib.EasyRouter.OrderType.Limit;
            Hashtable table = new Hashtable();
            table.Add("10052", "214214214"); // 214214214 IS THE RTD ORDER ID AS A STRING

            return m_Client.SubmitNewOrder(OrderInfo, table);
        }
    }
}
