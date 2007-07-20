using System;
using System.Collections.Generic;
using System.Text;
using VDMERLib.EasyRouter.Structure;

namespace VDMERLib.EasyRouter.Orders
{
    class FIXOrderManagement : OrderManagement
    {
        public FIXOrderManagement(InstrumentManager instrumentManager)
            : base(instrumentManager)
        {
           
        }
    }
}
