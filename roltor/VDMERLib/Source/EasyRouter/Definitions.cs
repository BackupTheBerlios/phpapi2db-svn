using System;
using System.Collections.Generic;
using System.Text;

namespace VDMERLib.EasyRouter
{
    public enum TimeInForce
    {
        ImmediateOrCancel = 0x0002,
        GoodTillCancelled = 0x0004,
        FillOrKill = 0x0008,
        StandardOrders = 0x0010,
        GoodTillDate = 0x0020,
        GoodTillCross = 0x0040,
        MarketOnOpen = 0x0080,
        MarketOnClose = 0x0100,
        GoodInSession = 0x0200
    }

    public enum OrderType
    {
        Limit = 0x0002,
        Market = 0x0004,
        HostStop = 0x0008,
        StopLimit = 0x0010,
        MarketIfTouched = 0x0020,
        MarketOnOpen = 0x0040,
        OnClose = 0x0100,
        PriceOpen = 0x0200,
        EasyStop = 0x0400,
        EasyStopLimit = 0x0800,
        MarketToLimit = 0x4000
    }
    class Definitions
    {
    }
}
