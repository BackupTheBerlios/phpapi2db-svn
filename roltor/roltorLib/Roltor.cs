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
        IExecute masterThreadExecutor;

        /// <summary>
        /// Reference to ER Trading system
        /// </summary>
        ERCSClient oltApi = null;

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
            
            masterThreadExecutor = executor;

            oltApi = ERCSClient.GetInstance();

            // We will chose Instruments to subscribe to
            oltApi.EnableAutoSubscription = false;
            // Auto subscribe to instruments that we post orders on
            oltApi.EnablePriceSubscriptionToTEonStructuredEvent = true;

            // Events
            oltApi.RecvOrderMsgEvent += new ERCSClient.RecvOrderMsg(RecvOrderMsgEvent);
            oltApi.RecvPriceMsgEvent += new ERCSClient.RecvPriceMsg(RecvPriceMsgEvent);
            oltApi.RecvGeneralMsgEvent += new ERCSClient.RecvGeneralMsg(RecvGeneralMsgEvent);  
        }

        void RecvGeneralMsgEvent(object from, VDMERLib.EasyRouter.General.GeneralMsgEventArg args)
        {
            if (args.DataType == GeneralMsgEventArg.GeneralDataType.Login)
            {
                Logon LoginInfo = (Logon)args;

                if(LoginInfo.LoggedOn)
                    System.Diagnostics.Debug.WriteLine("Logged onto OLT!!!");
                if (LoginInfo.LoggedOff)
                    System.Diagnostics.Debug.WriteLine("Logged off OLT!!!!");
            }
        }

        void RecvPriceMsgEvent(object from, VDMERLib.EasyRouter.Prices.PricesEventArg args)
        {
            if (args.DataType == VDMERLib.EasyRouter.Prices.PricesEventArg.PriceDataType.TradeData)
            {
                TradeData PriceInfo = (TradeData)args;

                string strMessage = string.Format("OLT->PRICE : {0} : BID={1} : ASK={3}",
                    PriceInfo.Symbol,
                    PriceInfo.Bid,
                    PriceInfo.Ask);
                System.Diagnostics.Debug.WriteLine(strMessage);
            }
        }

        void RecvOrderMsgEvent(object from, VDMERLib.EasyRouter.Orders.OrderDataEventArg args)
        {
            if (args.DataType == VDMERLib.EasyRouter.Orders.OrderDataEventArg.OrderDataType.Order)
            {
                OrderInfo OrderInfo = (OrderInfo) args;                
            }
        }

        /// <summary>
        /// Initialise connections to RTD and OLT
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            rtdApi.ConnectRTDapi();
            rtdApi.Login();
            oltApi.Logon("qwood", "R1sk", "vdm44olt1", true);
            return true;
        }

        /// <summary>
        /// Request order updates from RTD and start processing orders
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            rtdApi.SendRequest();
            return true;
        }


        /// <summary>
        /// Stop order updates from RTD
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            rtdApi.StopRequest();
            return true;
        }

        /// <summary>
        /// Disconnect from RTD and OLT
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            //rtdApi.CloseRTDapi();
            oltApi.Logoff();
            return true;
        }



        /// <summary>
        /// Decion process for orders coming from RTD
        /// </summary>
        /// <param name="iRTDOrderId">RTS Order Id</param>
        /// <param name="order">Order Structure</param>
        public void IncomingRTDOrderEvent(int iRTDOrderId, OrderStruct order)
        {
            string strMessage = "";

            // Order is Active
            if(order.State == 1)
            {
                // Is this the first time we have seen this order?
                if (!dicOrders.ContainsKey(iRTDOrderId))
                {
                    // Foward the master thread the order add request
                    masterThreadExecutor.AddOrder(order);
                    strMessage = string.Format("RTD->ROLTOR : {0,-6} : {1} : {2,8} of {3,8} @ {4,8}",
                        "ADD",
                        iRTDOrderId,
                        order.Qty,
                        order.ContractID,
                        order.Price);
                    System.Diagnostics.Debug.WriteLine(strMessage);
                    System.Diagnostics.Debug.WriteLine("isBid -> " + order.IsBid);

                    // then add it to the dictionary
                    dicOrders[iRTDOrderId] = order;                   
                }
                else
                {
                    // Has order quantity increased? If so we will need to pull and re-add the order
                    if (order.QtyOpen > dicOrders[iRTDOrderId].QtyOpen)
                    {
                        // Foward the master thread the order pull request
                        masterThreadExecutor.PullOrder(dicOrders[iRTDOrderId]);
                        strMessage = string.Format("RTD->ROLTOR : {0,-6} : {1} : {2,8} of {3,8} @ {4,8}",
                            "PULL",
                            iRTDOrderId,
                            order.Qty,
                            order.ContractID,
                            order.Price);
                        System.Diagnostics.Debug.WriteLine(strMessage);
                        System.Diagnostics.Debug.WriteLine("isBid -> " + order.IsBid);                        

                        // Foward the master thread the order add request
                        masterThreadExecutor.AddOrder(order);
                        strMessage = string.Format("RTD->ROLTOR : {0,-6} : {1} : {2,8} of {3,8} @ {4,8}",
                            "ADD",
                            iRTDOrderId,
                            order.Qty,
                            order.ContractID,
                            order.Price);
                        System.Diagnostics.Debug.WriteLine(strMessage);
                        System.Diagnostics.Debug.WriteLine("isBid -> " + order.IsBid);

                        // now add the new order to the dictionary
                        dicOrders[iRTDOrderId] = order;
                    }                       
                    // must just be a fill, price change or quantity reduction
                    else if (order.QtyOpen != dicOrders[iRTDOrderId].QtyOpen
                        || order.Price != dicOrders[iRTDOrderId].Price)
                    {
                        // Foward the master thread the order change request
                        masterThreadExecutor.ChangeOrder(order);
                        strMessage = string.Format("RTD->ROLTOR : {0,-6} : {1} : {2,8} of {3,8} @ {4,8}",
                            "CHANGE",
                            iRTDOrderId,
                            order.Qty,
                            order.ContractID,
                            order.Price);
                        System.Diagnostics.Debug.WriteLine(strMessage);
                        System.Diagnostics.Debug.WriteLine("isBid -> " + order.IsBid);

                        // add the new order to the dictionary
                        dicOrders[iRTDOrderId] = order;
                    }

                    // nothing important changed so do nothing
                }
                
            }
            // Order is in any other state than active
            else 
            {
                // if we have an order in our dictionary then we should pull the order and remove from dictionary
                if (dicOrders.ContainsKey(iRTDOrderId))
                {
                    // Foward the master thread the order pull request
                    masterThreadExecutor.PullOrder(dicOrders[iRTDOrderId]);
                    strMessage = string.Format("RTD->ROLTOR : {0,-6} : {1} : {2,8} of {3,8} @ {4,8}",
                        "PULL",
                        iRTDOrderId,
                        order.Qty,
                        order.ContractID,
                        order.Price);
                    System.Diagnostics.Debug.WriteLine(strMessage);
                    System.Diagnostics.Debug.WriteLine("isBid -> " + order.IsBid);

                    // remove the order from the dictionary
                    dicOrders.Remove(iRTDOrderId);
                }
            }
        }


        public bool AddOrderOlt(OrderStruct order)
        {
            bool bResult = false;

            oltApi.SubscribeTE("8XLONS:VOD.L");

            /*
            OrderInfo OrderInfo = new OrderInfo("8XLONS:VOD.L");
            OrderInfo.Price = 0.1;
            OrderInfo.OrderQty = 1000000;
            OrderInfo.TimeInForce = VDMERLib.EasyRouter.TimeInForce.StandardOrders;
            OrderInfo.OrderType = VDMERLib.EasyRouter.OrderType.Limit;
            Hashtable table = new Hashtable();
            table.Add("10052", "214214214"); // 214214214 IS THE RTD ORDER ID AS A STRING          
            bResult = oltApi.SubmitNewOrder(OrderInfo, table);
            */

            string strMessage = string.Format("ROLTOR->OLT : {0,-6} : {1} : {2,8} of {3,8} @ {4,8}",
                "ADD",
                order.OrderID,
                order.Qty,
                order.ContractID,
                order.Price);
            System.Diagnostics.Debug.WriteLine(strMessage);

            return bResult;
        }

        public bool ChangeOrderOlt(OrderStruct order)
        {
            bool bResult = false;

            string strMessage = string.Format("ROLTOR->OLT : {0,-6} : {1} : {2,8} of {3,8} @ {4,8}",
                "CHANGE",
                order.OrderID,
                order.Qty,
                order.ContractID,
                order.Price);
            System.Diagnostics.Debug.WriteLine(strMessage);

            return bResult;
        }

        public bool PullOrderOlt(OrderStruct order)
        {
            bool bResult = false;

            string strMessage = string.Format("ROLTOR->OLT : {0,-6} : {1} : {2,8} of {3,8} @ {4,8}",
                "PULL",
                order.OrderID,
                order.Qty,
                order.ContractID,
                order.Price);
            System.Diagnostics.Debug.WriteLine(strMessage);

            return bResult;
        }
    }
}
