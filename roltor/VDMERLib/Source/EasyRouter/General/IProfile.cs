using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace VDMERLib.EasyRouter.General
{
    public enum ScreenIDs
    {
        MarketTicker = 1,
        Watch = 2,
        OrderBook = 3,
        Risk = 32,
        Depth = 256,
        OrderTicket = 268435468,
        Allocation = 300000000
    }

    public interface IProfile
    {
        ScreenIDs ScreenID
        {
            get;
        }

        string InstanceID
        {
            get;
            set;
        }

        string FormName
        {
            get;
            set;
        }

        bool ReadProperties(XmlReader AttributeWriter);
        

        void WriteProperties(XmlWriter AttributeWriter);
        
    }
}
