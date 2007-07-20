/*
** OrderHistory.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** Stores Trade Leg Information for an order and Order Status
**
** Changes
** -------
**
*/

using System;
using System.Collections.Generic;
using System.Text;

using System.Collections;

using VDMFIXExecutionReportConstants = MESSAGEFIX3Lib.FIXExecutionReportConstants;

namespace VDMERLib.EasyRouter.Orders
{
    /// <summary>
    /// Class OrderHistory
    /// </summary>
    public class OrderHistory
    {
        /// <summary>
        /// Order History - stores a list of Trade and Order information
        /// </summary>
        ArrayList m_OrderHistory = new ArrayList();

        public ArrayList Events
        {
            get { return m_OrderHistory; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="firstOrder"></param>
        public OrderHistory(OrderInfo firstOrder)
        {
            if (firstOrder != null)
            {
                m_OrderHistory.Add(firstOrder);
                m_CurrentOrder = new OrderInfo(firstOrder);  
            }
        }

        /// <summary>
        /// This Order  object stores the current order state from the updated order and trade messages 
        /// </summary>
        OrderInfo m_CurrentOrder = null;

        /// <summary>
        /// Pbulic property to get the Order object that stores the current order state from the updated order and trade messages 
        /// </summary>
        public OrderInfo CurrentOrder
        {
            get { return m_CurrentOrder; }
        }

        /// <summary>
        /// Get the priamry boid - identifier in ER system
        /// </summary>
        public int Key
        {
            get 
            {
                if (m_CurrentOrder == null)
                {
                    return int.MinValue;  
                }
                return m_CurrentOrder.PrimaryBOID;
            }
        }
        
        /// <summary>
        /// Update the current order with the FIX message trade or order update
        /// </summary>
        /// <param name="order"></param>
        public void UpdateOrder(OrderInfo order)
        {
            if (m_CurrentOrder != null)
            {
                if ((order.ExecutionReportStatus != Status.PendingCancel) &&
                   (order.ExecutionReportStatus != Status.PendingReplace))
                {
                    m_CurrentOrder.ApplyChanges(order);
                }
                else
                {
                    if(order.TransactTime != null) 
                        m_CurrentOrder.TransactTime = order.TransactTime; 
                }
                m_OrderHistory.Add(order); 
            }
        }

    }
}
