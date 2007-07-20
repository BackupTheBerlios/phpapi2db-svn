using System;
using System.Collections.Generic;
using System.Text;

namespace RoltorLib
{
    public interface IExecute
    {
       
        void AddOrder(OrderStruct order);

        void ChangeOrder(OrderStruct order);

        void PullOrder(OrderStruct order);
                
    }
}
