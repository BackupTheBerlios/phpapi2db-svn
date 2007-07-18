using System;
using System.Collections.Generic;
using System.Text;

namespace RoltorLib
{
    public struct OrderStruct
    {
        public string[] arrOrderData;
        public string this[int iFID]
        {
            get
            {
                return arrOrderData[iFID];
            }
            set
            {
                arrOrderData[iFID] = value;
            }
        }
        public void SetOrder(int iFID, string strData)
        {
            arrOrderData[iFID] = strData;
        } 

    }

    public class Roltor
    {
        Dictionary<int,OrderStruct> dicOrders = new Dictionary<int,OrderStruct>(8000);

        private RoltorRTDtcp rtdApi;

        public Roltor()
        {
            this.rtdApi = new RoltorLib.RoltorRTDtcp(this);
        }

        public bool Connect()
        {
            rtdApi.ConnectRTDapi();
            rtdApi.Login();
            return true;
        }

        public bool Start()
        {
            rtdApi.SendRequest();
            return true;
        }

        public bool Stop()
        {
            rtdApi.StopRequest();
            return true;
        }

        public bool Close()
        {
            rtdApi.CloseRTDapi();
            return true;
        }

        public void UpdateOrder(int iRTDOrderId, OrderStruct order)
        {
            dicOrders[iRTDOrderId] = order;
            string strMessage = "OrderState=" + order[202];
            strMessage += " DesiredState=" + order[196];
            strMessage += " Price=" + order[76];
            strMessage += " Qty=" + order[170];
            strMessage += " FillQty=" + order[143];
            strMessage += " OpenQty=" + order[149];
            strMessage += " Contract=" + order[17];
            strMessage += " Account=" + order[1];            

            System.Diagnostics.Debug.WriteLine(strMessage);
        }
    }
}
