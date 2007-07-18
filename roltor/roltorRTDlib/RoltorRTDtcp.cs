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

        public RoltorRTDtcp(Roltor myRoltor)
        {
            this.myRoltor = myRoltor;
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


        private void Start()
        {
            readThread = new Thread(new ThreadStart(this.Read));
            readThread.Name = "Read";
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
            string[] arrFIDs;
            

            string strRID = strMessage.Substring(0, strMessage.IndexOf('\x1F'));

            switch (strRID)
            {
                case "3":
                    strMessage = strMessage.Substring(strMessage.IndexOf('\x1F') + 1);
                    //arrFIDs = strMessage.Split('\x1F');
                    System.Diagnostics.Debug.WriteLine("Answer - " + strMessage);
                    break;

                case "45":
                    /* this is all we really need, but couldn't be arsed filtering it out just now.
                     * arrData[0] = fid_rtd_order_id (167)
                     * arrData[1] = fid_contract_id (17)
                     * arrData[2] = fid_exchange_id (47)
                     * arrData[3] = fid_account_id (1)
                     * arrData[4] = fid_state (202)
                     * arrData[5] = fid_desired_state (196)
                     * arrData[6] = fid_order_bid (150)
                     * arrData[7] = fid_total_qty (170)
                     * arrData[8] = fid_open_qty (149)
                     * arrData[9] = fid_match_qty (143)
                    */
                    

                    OrderStruct order = new OrderStruct();
                    order.arrOrderData = new string[401];

                    strMessage = strMessage.Substring(strMessage.IndexOf('\x1F') + 1);
                    arrFIDs = strMessage.Split('\x1F');
                                        
                    for (int y = 0; y < arrFIDs.Length; y++)
                        order.SetOrder(int.Parse(arrFIDs[y]), arrFIDs[++y]);

                    System.Diagnostics.Debug.WriteLine("Order " + order[167] + " found");
                    if(BadHackFilter(order))
                        myRoltor.UpdateOrder(int.Parse(order[167]), order);
                    
                    break;

                default:
                    System.Diagnostics.Debug.WriteLine(strMessage);
                    break;
            }
            
        }


        private bool BadHackFilter(OrderStruct order)
        {
            bool bIsGood = false;

            Dictionary<int, int> arrContractIDs = new Dictionary<int, int>(11);
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

            Dictionary<int, int> arrAccountIDs = new Dictionary<int, int>(10);
            arrAccountIDs[0] = 1847;
            arrAccountIDs[1] = 1881;
            arrAccountIDs[2] = 1914;
            arrAccountIDs[3] = 1803;
            arrAccountIDs[4] = 1848;
            arrAccountIDs[5] = 596;
            arrAccountIDs[6] = 1916;
            arrAccountIDs[7] = 1645;
            arrAccountIDs[8] = 971;
            arrAccountIDs[9] = 932;

            if (arrAccountIDs.ContainsValue(int.Parse(order[1])) && arrContractIDs.ContainsValue(int.Parse(order[17])))
                bIsGood = true;

            return bIsGood;
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
                    string sTemp = string.Format("89{0}188{0}svcroltor{0}194{0}SVCROLTOR{0}15{0}777", '\x1F');
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
                    string sTemp = string.Format("44{0}273{0}0{0}15{0}778{0}105{0}1", '\x1F');
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
                    string sTemp = string.Format("44{0}273{0}0{0}15{0}778{0}105{0}2", '\x1F');
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

        private string developer = "Benn";
        public string Developer { get { return developer; } }
    }
}
