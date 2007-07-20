using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace RoltorLib
{
    public class RoltorRTDtcp
    {
        protected NetworkStream networkStream;
        protected StreamWriter streamOut;
        protected StreamReader streamIn;

        private Roltor myRoltor;

        private TcpClient rtdConn = null;
        private Thread readThread;
        private bool bContinue = false;

        private Dictionary<int, int> arrContractIDs = new Dictionary<int, int>(11);
        private Dictionary<int, string> arrAccountTexts = new Dictionary<int, string>(4);
        private Dictionary<int, int> arrExchangeIDs = new Dictionary<int, int>();
            

        public RoltorRTDtcp(Roltor myRoltor)
        {
            this.myRoltor = myRoltor;
            
            arrContractIDs[0] = 1146465;
            arrContractIDs[1] = 23049;
            arrContractIDs[2] = 773634;
            arrContractIDs[3] = 5340;
            arrContractIDs[4] = 5282;
            arrContractIDs[5] = 35;
            arrContractIDs[6] = 317271;
            arrContractIDs[7] = 145566;
            arrContractIDs[8] = 57;
            arrContractIDs[9] = 418241;
            arrContractIDs[10] = 1021863;

            arrAccountTexts[0] = "VME04";
            arrAccountTexts[1] = "VME08";
            arrAccountTexts[2] = "VME11";
            arrAccountTexts[3] = "VME13";
            arrAccountTexts[4] = "VME17";
            arrAccountTexts[5] = "VME20";

            arrExchangeIDs[0] = 570;
        }

        public void ConnectRTDapi()
        {
            if (rtdConn == null || rtdConn.Connected == false)
            {
                try
                {
                    rtdConn = new TcpClient();
                    rtdConn.Connect("10.26.29.1", 1290);
                    if (rtdConn.Connected)
                    {
                        System.Diagnostics.Debug.WriteLine("Connected to 10.26.29.1");
                        networkStream = rtdConn.GetStream();

                        streamOut = new StreamWriter(networkStream, System.Text.Encoding.ASCII);
                        streamIn = new StreamReader(networkStream);
                        Start();
                    }
                }
                catch (Exception err)
                {
                    rtdConn = null;
                    System.Diagnostics.Debug.WriteLine("Error " + err.Message);
                }
            }
        }


        public void CloseRTDapi()
        {
            if (rtdConn != null && rtdConn.Connected)
            {
                streamOut.Close();
                streamIn.Close();
                networkStream.Close();
                rtdConn.Close();
                rtdConn = null;
            }
            if (readThread != null)
            {
                if (bContinue == true)
                {
                    readThread.Abort();
                    readThread.Join();
                }
            }
        }

        public void Login()
        {
            if (rtdConn.Connected)
            {
                try
                {
                    // RID: LOGIN REQ
                    string sTemp = string.Format("89{0}188{0}svcroltor{0}194{0}SVCROLTOR{0}15{0}001", '\x1F');
                    System.Diagnostics.Debug.WriteLine(sTemp);
                    streamOut.WriteLine(sTemp);
                    streamOut.Flush();
                }
                catch (Exception err)
                {
                    System.Diagnostics.Debug.WriteLine("Error " + err.Message);
                }
            }
        }

        public void SendRequest()
        {
            if (rtdConn.Connected)
            {
                try
                {
                    // RID: ORDER REQ LOAD ALL
                    string sTemp = string.Format("44{0}273{0}0{0}15{0}002{0}105{0}1", '\x1F');
                    System.Diagnostics.Debug.WriteLine(sTemp);
                    streamOut.WriteLine(sTemp);
                    // RID: EXCHANGE REQ LOAD ALL
                    //string sTemp = string.Format("10{0}15{0}0{0}105{0}1", '\x1F');
                    streamOut.Flush();
                }
                catch (Exception err)
                {
                    System.Diagnostics.Debug.WriteLine("Error " + err.Message);
                }
            }
        }


        public void StopRequest()
        {
            if (rtdConn.Connected)
            {
                try
                {
                    // RID: ORDER REQ LOAD ALL - Unsubscribe to updates
                    string sTemp = string.Format("44{0}273{0}0{0}15{0}003{0}105{0}2", '\x1F');
                    System.Diagnostics.Debug.WriteLine(sTemp);
                    streamOut.WriteLine(sTemp);
                    streamOut.Flush();
                }
                catch (Exception err)
                {
                    System.Diagnostics.Debug.WriteLine("Error " + err.Message);
                }
            }
        }

        /*
         * Send RTD and order change
         */
        public void ChangeOrder(int iRTDOrderId, float fPrice, int iQtyTotal, int iQtyOpen)
        {
            if (rtdConn.Connected)
            {
                try
                {
                    // RID: ORDER REQ CHANGE - Order request change
                    string sTemp = string.Format("42{0}167{0}{1}{0}76{0}{2}{0}170{0}{3}{0}149{0}{4}{0}15{0}004", 
                        '\x1F', 
                        iRTDOrderId, 
                        fPrice,
                        iQtyTotal,
                        iQtyOpen);
                    System.Diagnostics.Debug.WriteLine("CHANGING RTD ORDER WITH -> " + sTemp);
                    //streamOut.WriteLine(sTemp);
                    //streamOut.Flush();
                }
                catch (Exception err)
                {
                    System.Diagnostics.Debug.WriteLine("Error " + err.Message);
                }
            }
        }

        /*
         * Send RTD and trade fill
         */
        public void AddTradeFill(OrderStruct order)
        {
            if (rtdConn.Connected)
            {
                try
                {
                    /*
RID: TRADE REQ ADD
# Mandatory Parameters:
account id, context, contract id, contracts long, contracts short, counterparty id, currency id, date,
exchange id, price, time, value date cash, value date contract (for non-cash trades)
# Optional Parameters:
basket id(0), bof sequence no(0), booking price(0), broker id(0), clearing account(""),
clearing text(""), courtage(0), cpty clr id(0), cpty id(0), currency booking id(0),
currency courtage id(0), currency fees id(0), currency provision id(0), exchange tran code(0),
external order no(""), fees(0), match id(""), member clr id(0), member id(0), order number(""),
posting code(0), provision(0), price type(0), rtd order id(""), trade flags(0), trader id(0),
trade number(""), trade text(""), underlying contract id(0), user id(0), value(0),
value date contract(0), value date fees(0)
If booking price, fees, provision, courtage are not 0 the corresponding currency ids must be specified
(currency booking id etc.)
                     */
                    // RID: TRADE REQ ADD - Order request change
                    string sTemp = string.Format("61{0}167{0}{1}{0}76{0}{2}{0}1{0}{3}{0}17{0}{4}{0}20{0}{5}"+
                        "{0}21{0}0{0}24{0}203{0}30{0}{6}{0}47{0}{7}{0}33{0}{8}{0}96{0}{9}{0}15{0}005",
                        '\x1F',
                        order.OrderID,
                        order.Price,
                        order.AccountID,
                        order.ContractID,
                        order.QtyOpen,
                        order.CurrencyID,
                        order.ExchangeID,
                        DateTime.Now.ToString("yyyyMMdd"),
                        DateTime.Now.ToString("HHmmss"));
                    System.Diagnostics.Debug.WriteLine("ADD RTD TRADE FILL WITH -> " + sTemp);
                    //streamOut.WriteLine(sTemp);
                    //streamOut.Flush();
                }
                catch (Exception err)
                {
                    System.Diagnostics.Debug.WriteLine("Error " + err.Message);
                }
            }
        }


        private void Start()
        {
            readThread = new Thread(new ThreadStart(this.Read));
            readThread.Name = "RoltorRTDtcpRead";
            readThread.Start();
        }


        private void Read()
        {
            bContinue = true;
            String strMessages = "";

            while (bContinue)
            {
                try
                {
                    char[] array = new char[1024];
                    int nLength = streamIn.Read(array, 0, 1024);
                    if (nLength > 0)
                    {
                        strMessages += new string(array, 0, nLength);
                        string[] arrMessages = strMessages.Split('\xA');

                        for (int x = 0; x < arrMessages.Length - 1; x++)
                        {
                            DecodeMessage(arrMessages[x]);
                        }

                        strMessages = arrMessages[arrMessages.Length - 1];
                    }
                    else
                    {
                        if (!rtdConn.Connected)
                        {
                            bContinue = false;
                        }
                    }
                }
                catch (Exception err)
                {
                    System.Diagnostics.Debug.WriteLine("Error " + err.Message);
                    bContinue = false;
                }
            }
        }


        private void DecodeMessage(string strMessage)
        {
            string[] arrMessage;


            string strRID = strMessage.Substring(0, strMessage.IndexOf('\x1F'));

            switch (strRID)
            {
                case "3":
                    strMessage = strMessage.Substring(strMessage.IndexOf('\x1F') + 1);
                    //arrFIDs = strMessage.Split('\x1F');
                    System.Diagnostics.Debug.WriteLine("Answer - " + strMessage);
                    break;

                case "45":   
                    OrderStruct order = new OrderStruct();

                    strMessage = strMessage.Substring(strMessage.IndexOf('\x1F') + 1);
                    arrMessage = strMessage.Split('\x1F');

                    // Lets loop over a bunch of time to get the shit we need

                    // fid_order_id
                    for (int y = 0; y < arrMessage.Length; y++)
                        if (arrMessage[y] == "167")
                            order.OrderID = int.Parse(arrMessage[++y]);
                    // fid_contract_id
                    for (int y = 0; y < arrMessage.Length; y++)
                        if (arrMessage[y] == "17")
                            order.ContractID = int.Parse(arrMessage[++y]);
                    // fid_currency_id
                    for (int y = 0; y < arrMessage.Length; y++)
                        if (arrMessage[y] == "30")
                            order.Text = arrMessage[++y];
                    // fid_exchange_id
                    for (int y = 0; y < arrMessage.Length; y++)
                        if (arrMessage[y] == "47")
                            order.ExchangeID = int.Parse(arrMessage[++y]);
                    // fid_state
                    for (int y = 0; y < arrMessage.Length; y++)
                        if (arrMessage[y] == "202")
                            order.State = int.Parse(arrMessage[++y]);
                    // fid_order_bid
                    for (int y = 0; y < arrMessage.Length; y++)
                        if (arrMessage[y] == "150")
                            order.IsBid = arrMessage[++y] == "1" ? true : false;
                    // fid_total_qty
                    for (int y = 0; y < arrMessage.Length; y++)
                        if (arrMessage[y] == "170")
                            order.Qty = int.Parse(arrMessage[++y]);
                    // fid_open_qty
                    for (int y = 0; y < arrMessage.Length; y++)
                        if (arrMessage[y] == "149")
                            order.QtyOpen = int.Parse(arrMessage[++y]);
                    // fid_price
                    for (int y = 0; y < arrMessage.Length; y++)
                        if (arrMessage[y] == "76")
                            order.Price = float.Parse(arrMessage[++y]);
                    // fid_text
                    for (int y = 0; y < arrMessage.Length; y++)
                        if (arrMessage[y] == "203")
                            order.Text = arrMessage[++y];

                    if (BadHackFilter(order))
                    {
                        //System.Diagnostics.Debug.WriteLine("Order " + order.OrderID + " found,ACC=" + order.Text + ",CTR=" + order.ContractID);
                        myRoltor.UpdateOrder(order.OrderID, order);
                    }

                    break;

                default:
                    System.Diagnostics.Debug.WriteLine(strMessage);
                    break;
            }

        }


        private bool BadHackFilter(OrderStruct order)
        {
            bool bIsGood = false;

            if (arrAccountTexts.ContainsValue(order.Text) 
                && arrContractIDs.ContainsValue(order.ContractID)
                && arrExchangeIDs.ContainsValue(order.ExchangeID))
                bIsGood = true;

            return bIsGood;
        }

        

    }
}
