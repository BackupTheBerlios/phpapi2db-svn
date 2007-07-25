using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using VDMERLib.EasyRouter.EasyRouterClient;
using VDMERLib.EasyRouter.Orders;
using VDMERLib.EasyRouter.Prices;
using VDMERLib.EasyRouter.General;

namespace RoltorLib
{
    public struct OrderStruct
    {
        private int iPBOID;
        private int iOrderID;
        private int iAccountID;
        private int iContractID;
        private int iCurrencyID;
        private int iExchangeID;
        private int iState;
        private bool bIsBid;
        private int iQty;
        private int iQtyOpen;        
        private double dPrice;
        private string sText;
        private string sOrderAction;

        public int PBOID { get { return iPBOID; } set { iPBOID = value; } }
        public int OrderID { get { return iOrderID; } set { iOrderID = value; } }
        public int AccountID { get { return iAccountID; } set { iAccountID = value; } }
        public int ContractID { get { return iContractID; } set { iContractID = value; } }
        public int CurrencyID { get { return iCurrencyID; } set { iCurrencyID = value; } }
        public int ExchangeID { get { return iExchangeID; } set { iExchangeID = value; } }
        public int State { get { return iState; } set { iState = value; } }
        public bool IsBid { get { return bIsBid; } set { bIsBid = value; } }
        public int Qty { get { return iQty; } set { iQty = value; } }
        public int QtyOpen { get { return iQtyOpen; } set { iQtyOpen = value; } }
        public double Price { get { return dPrice; } set { dPrice = value; } }
        public string Text { get { return sText; } set { sText = value; } }
        public string Action { get { return sOrderAction; } set { sOrderAction = value; } }
    }

    public struct PriceStruct
    {
        private double dBestBid;
        private double dBestAsk;

        public double BestBid { get { return dBestBid; } set { dBestBid = value; } }
        public double BestAsk { get { return dBestAsk; } set { dBestAsk = value; } }

        public PriceStruct(double bid, double ask)
        {
            dBestBid = bid;
            dBestAsk = ask;
        }
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
        ERCSClient voltApi = null;

        /// <summary>
        /// The RTD api tcp object
        /// </summary>
        private RoltorRTDtcp rtdApi;

        /// <summary>
        /// The location of the csv containing the RTD Contract ID to OLT Chi-X TE Mneumonics
        /// </summary>
        private string sFileNameContracts = "C:\\Stuff\\Development\\Svn\\roltor\\contracts.csv";

        /// <summary>
        /// Dictionary containing the mapping of RTD ContractIDs to OLT Chi-X TE Mneumonics
        /// </summary>
        public Dictionary<int, string> dicContractMap = new Dictionary<int, string>();

        /// <summary>
        /// Dictionary containing all active orders
        /// </summary>
        private Dictionary<int,OrderStruct> dicOrders = new Dictionary<int,OrderStruct>();

        /// <summary>
        /// Dictionary containing all best prices
        /// </summary>
        private Dictionary<string, double> dicPricesBid = new Dictionary<string, double>();
        private Dictionary<string, double> dicPricesAsk = new Dictionary<string, double>();


        
        /// <summary>
        /// the Roltor engine
        /// </summary>
        /// <param name="executor">Reference to the master thread object from this class was called</param>
        public Roltor(IExecute executor)
        {
            this.rtdApi = new RoltorLib.RoltorRTDtcp(this);
            
            masterThreadExecutor = executor;

            voltApi = ERCSClient.GetInstance();

            // We will chose Instruments to subscribe to
            voltApi.EnableAutoSubscription = false;
            // Auto subscribe to instruments that we post orders on
            voltApi.EnablePriceSubscriptionToTEonStructuredEvent = true;

            // Events
            voltApi.RecvOrderMsgEvent += new ERCSClient.RecvOrderMsg(RecvOrderMsgEvent);
            voltApi.RecvPriceMsgEvent += new ERCSClient.RecvPriceMsg(RecvPriceMsgEvent);
            voltApi.RecvGeneralMsgEvent += new ERCSClient.RecvGeneralMsg(RecvGeneralMsgEvent);  
        }


        /// <summary>
        /// Initialise connections to RTD and OLT
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            if (!ReadContractsFromFile())
                return false;

            rtdApi.ConnectRTDapi();
            if (rtdApi.IsConnected)
                rtdApi.Login();
            else
                return false;

            return voltApi.Logon("svcRoltor", "svcRoltor", "vdm44olt1", true);
        }

        /// <summary>
        /// Request order updates from RTD and start processing orders
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            if (rtdApi.IsConnected)
            {
                rtdApi.SendRequest();
            }
            if (voltApi != null)
            {
                SubscribeTradeables();
            }
            return true;
        }


        /// <summary>
        /// Stop order updates from RTD
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            if (rtdApi.IsConnected)
            {
                rtdApi.StopRequest();
            }
            return true;
        }

        /// <summary>
        /// Disconnect from RTD and OLT
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            if (rtdApi.IsConnected)
            {
                rtdApi.CloseRTDapi();
            }
            voltApi.Logoff();
            return true;
        }


        private bool ReadContractsFromFile()
        {
            bool bIsProcessed = false;

            if (File.Exists(sFileNameContracts))
            {
                try
                {
                    string strLine;
                    string[] arrLine;

                    StreamReader stream = new StreamReader(sFileNameContracts);
                    while (!stream.EndOfStream)
                    {
                        strLine = stream.ReadLine();
                        arrLine = strLine.Split(',');
                        dicContractMap[int.Parse(arrLine[0])] = arrLine[1];
                    }
                    bIsProcessed = true;
                }
                catch (IOException)
                {
                    System.Diagnostics.Debug.WriteLine("Error reading from " + sFileNameContracts);
                    System.Diagnostics.Debug.WriteLine("CONTRACTS NOT LOADED!");
                }

            }

            return bIsProcessed;
        }



        /// <summary>
        /// Decision process for orders coming from RTD
        /// </summary>
        /// <param name="iRTDOrderId">RTS Order Id</param>
        /// <param name="order">Order Structure</param>
        public void IncomingRTDOrderEvent(int iRTDOrderId, OrderStruct order)
        {
            string strMessage = "";
            string strTESymbol = dicContractMap[order.ContractID];

            bool bIsPassive = (order.IsBid && order.Price < dicPricesAsk[strTESymbol])
                    || (!order.IsBid && order.Price > dicPricesBid[strTESymbol]);

            // Order is Active and not aggressive
            if(order.State == 1 && bIsPassive)                
            {
                // Is this the first time we have seen this order?
                if (!dicOrders.ContainsKey(iRTDOrderId))
                {
                    // DEBUG
                    strMessage = string.Format("RTD->ROLTOR : {0,-6} : {1} : {2,8} of {3,8} @ {4,8}",
                        "ADD",
                        iRTDOrderId,
                        order.Qty,
                        order.ContractID,
                        order.Price);
                    System.Diagnostics.Debug.WriteLine(strMessage);
                    System.Diagnostics.Debug.WriteLine("isBid -> " + order.IsBid);

                    // Foward the master thread the order add request
                    masterThreadExecutor.AddOrder(order);

                    // then add it to the dictionary
                    dicOrders[iRTDOrderId] = order;                   
                }
                else
                {
                    // Has order quantity increased? If so we will need to pull and re-add the order
                    if (order.QtyOpen > dicOrders[iRTDOrderId].QtyOpen)
                    {
                        // DEBUG
                        strMessage = string.Format("RTD->ROLTOR : {0,-6} : {1} : {2,8} of {3,8} @ {4,8}",
                            "PULL",
                            iRTDOrderId,
                            order.Qty,
                            order.ContractID,
                            order.Price);
                        System.Diagnostics.Debug.WriteLine(strMessage);
                        System.Diagnostics.Debug.WriteLine("isBid -> " + order.IsBid);

                        // Foward the master thread the order pull request
                        masterThreadExecutor.PullOrder(dicOrders[iRTDOrderId]);


                        // DEBUG
                        strMessage = string.Format("RTD->ROLTOR : {0,-6} : {1} : {2,8} of {3,8} @ {4,8}",
                            "ADD",
                            iRTDOrderId,
                            order.Qty,
                            order.ContractID,
                            order.Price);
                        System.Diagnostics.Debug.WriteLine(strMessage);
                        System.Diagnostics.Debug.WriteLine("isBid -> " + order.IsBid);

                        // Foward the master thread the order add request
                        masterThreadExecutor.AddOrder(order);

                        // now add the new order to the dictionary
                        dicOrders[iRTDOrderId] = order;
                    }                       
                    // must just be a fill, price change or quantity reduction
                    else if (order.QtyOpen != dicOrders[iRTDOrderId].QtyOpen
                        || order.Price != dicOrders[iRTDOrderId].Price)
                    {
                        // DEBUG
                        strMessage = string.Format("RTD->ROLTOR : {0,-6} : {1} : {2,8} of {3,8} @ {4,8}",
                            "CHANGE",
                            iRTDOrderId,
                            order.Qty,
                            order.ContractID,
                            order.Price);
                        System.Diagnostics.Debug.WriteLine(strMessage);
                        System.Diagnostics.Debug.WriteLine("isBid -> " + order.IsBid);

                        // Foward the master thread the order change request
                        masterThreadExecutor.ChangeOrder(order);

                        // add the new order to the dictionary
                        dicOrders[iRTDOrderId] = order;
                    }

                    // nothing important changed so do nothing
                }

            }
            // Order is in any other state than active or it is an aggressive order
            else 
            {
                // if we have an order in our dictionary then we should pull the order and remove from dictionary
                if (dicOrders.ContainsKey(iRTDOrderId))
                {
                    //DEBUG
                    strMessage = string.Format("RTD->ROLTOR : {0,-6} : {1} : {2,8} of {3,8} @ {4,8}",
                        "PULL",
                        iRTDOrderId,
                        order.Qty,
                        order.ContractID,
                        order.Price);
                    System.Diagnostics.Debug.WriteLine(strMessage);
                    System.Diagnostics.Debug.WriteLine("isBid -> " + order.IsBid);
                    
                    // Foward the master thread the order pull request
                    masterThreadExecutor.PullOrder(dicOrders[iRTDOrderId]);

                    // remove the order from the dictionary
                    dicOrders.Remove(iRTDOrderId);
                }
            }
        }


        private void SubscribeTradeables()
        {
            foreach (int iKey in dicContractMap.Keys)
            {
                voltApi.SubscribeTE(dicContractMap[iKey]);
            }
        }



        public bool AddOrderOlt(OrderStruct order)
        {
            bool bResult = false;
            string strTESymbol = dicContractMap[order.ContractID];

            OrderInfo OrderInfo = new OrderInfo(strTESymbol);
            OrderInfo.Price = order.Price;
            OrderInfo.OrderQty = order.QtyOpen;
            OrderInfo.TimeInForce = VDMERLib.EasyRouter.TimeInForce.StandardOrders;
            OrderInfo.OrderType = VDMERLib.EasyRouter.OrderType.Limit;
            //Hashtable table = new Hashtable();
            //table.Add("10052", "214214214"); // 214214214 IS THE RTD ORDER ID AS A STRING          
            //bResult = voltApi.SubmitNewOrder(OrderInfo, table);

            //DEBUG
            string strMessage = string.Format("ROLTOR->OLT : {0,-6} : {1} : {2,8} of {3,8} @ {4,8}",
                "ADD",
                order.OrderID,
                order.Qty,
                order.ContractID,
                order.Price);


            return bResult;
        }

        public bool ChangeOrderOlt(OrderStruct order)
        {
            bool bResult = false;
            string strTESymbol = dicContractMap[order.ContractID];
            int iPBOID = order.PBOID;

            if (iPBOID != 0)
            {
                OrderInfo OrderInfo = voltApi.OrderManagement.GetCurrentOrder(iPBOID);

                //bResult = voltApi.EditOrder(OrderInfo, order.Price, order.QtyOpen)

                //DEBUG
                string strMessage = string.Format("ROLTOR->OLT : {0,-6} : {1} : {2,8} of {3,8} @ {4,8}",
                    "CHANGE",
                    order.OrderID,
                    order.Qty,
                    order.ContractID,
                    order.Price);
                System.Diagnostics.Debug.WriteLine(strMessage);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("FUCK!!! BOID not set........ CANT CHANGE ORDER - KILLING MYSELF");
            }

            return bResult;
        }

        public bool PullOrderOlt(OrderStruct order)
        {
            bool bResult = false;
            string strTESymbol = dicContractMap[order.ContractID];
            int iPBOID = order.PBOID;

            if (iPBOID != 0)
            {
                bResult = voltApi.PullOrder(iPBOID);

                //DEBUG
                string strMessage = string.Format("ROLTOR->OLT : {0,-6} : {1} : {2,8} of {3,8} @ {4,8}",
                    "CHANGE",
                    order.OrderID,
                    order.Qty,
                    order.ContractID,
                    order.Price);
                System.Diagnostics.Debug.WriteLine(strMessage);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("FUCK!!! BOID not set........ CANT PULL ORDER - KILLING MYSELF");
            }

            return bResult;
        }



        void RecvGeneralMsgEvent(object from, VDMERLib.EasyRouter.General.GeneralMsgEventArg args)
        {
            if (args.DataType == GeneralMsgEventArg.GeneralDataType.Login)
            {
                Logon LoginInfo = (Logon)args;

                if (LoginInfo.LoggedOn)
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

                if (PriceInfo.Ask != null && PriceInfo.Ask.HasChanged)
                {
                    dicPricesAsk[PriceInfo.Symbol] = PriceInfo.Ask.Price.Value;

                    //DEBUG
                    string strMessage = string.Format("OLT->PRICE : {0} : ASK={1}",
                        PriceInfo.Symbol,
                        PriceInfo.Ask.Price);
                    //System.Diagnostics.Debug.WriteLine(strMessage);
                }

                if (PriceInfo.Bid != null && PriceInfo.Bid.HasChanged)
                {
                    dicPricesBid[PriceInfo.Symbol] = PriceInfo.Bid.Price.Value;

                    // DEBUG
                    string strMessage = string.Format("OLT->PRICE : {0} : BID={1}",
                        PriceInfo.Symbol,
                        PriceInfo.Bid.Price);
                    //System.Diagnostics.Debug.WriteLine(strMessage);
                }                
            }
        }

        void RecvOrderMsgEvent(object from, VDMERLib.EasyRouter.Orders.OrderDataEventArg args)
        {
            if (args.DataType == VDMERLib.EasyRouter.Orders.OrderDataEventArg.OrderDataType.Order)
            {
                OrderInfo OrderInfo = (OrderInfo)args;

                string strMessage = string.Format("OLT->ORDER : {0} : {1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                    OrderInfo.PrimaryBOID,
                    OrderInfo.ExecutionReportStatus,
                    OrderInfo.AccountID,
                    OrderInfo.Instrument.ESTickerMnemonic,
                    OrderInfo.Instrument.ESTickerDescription,
                    OrderInfo.OrderQty,
                    OrderInfo.CumQty,
                    OrderInfo.LeavesQty,
                    OrderInfo.IsBuy,
                    OrderInfo.IsSell,
                    OrderInfo.Price);
                //System.Diagnostics.Debug.WriteLine(strMessage);

            }
        }
    }
}
