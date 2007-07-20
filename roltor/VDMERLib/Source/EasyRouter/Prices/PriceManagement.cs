using System;
using System.Collections.Generic;
using System.Text;

namespace VDMERLib.EasyRouter.Prices
{
    public class PriceManagement : SortedDictionary<string, TradeData>
    {
        internal TradeData ProcessPrice(EASYROUTERCOMCLIENTLib.IFIXMessage FIXMsg, bool bSnapFull)
        {
            TradeData data = new TradeData();
            if (!data.DecodeFIX(FIXMsg, bSnapFull))
               return  null;

            if (this.ContainsKey(data.Symbol))
                this[data.Symbol].Update(data);
            else
                this.Add(data.Symbol, data);
            return data;
        }
    }
}
