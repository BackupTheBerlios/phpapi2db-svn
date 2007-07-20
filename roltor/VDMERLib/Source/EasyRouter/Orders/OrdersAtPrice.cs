using System;
using System.Collections.Generic;
using System.Text;

namespace VDMERLib.EasyRouter.Orders 
{
    class OrdersAtPrice : Dictionary<double, Dictionary<int, OrderInfo>>
    {
        public void Add(double price, OrderInfo order)
        {
            Dictionary<int, OrderInfo> OrderList;
            if (this.ContainsKey(price))
                OrderList = this[price];
            else
            {
                OrderList = new Dictionary<int, OrderInfo>();
                this.Add(price, OrderList);
            }

            if (OrderList.ContainsKey(order.PrimaryBOID))
                OrderList.Remove(order.PrimaryBOID);

            OrderList.Add(order.PrimaryBOID, order);
        }
        public void Remove(double price, OrderInfo order)
        {
            Dictionary<int, OrderInfo> OrderList;
            if (this.ContainsKey(price))
                OrderList = this[price];
            else
#warning "add error text";
                return;
            
            if (OrderList.ContainsKey(order.PrimaryBOID))
                OrderList.Remove(order.PrimaryBOID);

            if (OrderList.Count == 0)
                this.Remove(price);
        }
    }
}
