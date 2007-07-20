/*
** OrderDataEventArg.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** Base class for all order/ trade events
**
** Changes
** -------
**
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace VDMERLib.EasyRouter.Orders
{
    /// <summary>
    /// Class OrderDataEventArg
    /// </summary>
    public class OrderDataEventArg
    {
        /// <summary>
        /// OrderDataType
        /// </summary>
        public enum OrderDataType
        {
            /// <summary>
            /// unknown
            /// </summary>
            Unknown,
            /// <summary>
            /// trade
            /// </summary>
            Trade,
            /// <summary>
            /// order
            /// </summary>
            Order
        }

        /// <summary>
        /// Type of message
        /// </summary>
        protected OrderDataType m_OrderType = OrderDataType.Order;

        /// <summary>
        /// Get type
        /// </summary>
        public OrderDataType DataType
        {
            get { return m_OrderType; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public OrderDataEventArg(OrderDataType type)
        {
            m_OrderType = type;
        }

        /// <summary>
        /// Vanilla
        /// </summary>
        public OrderDataEventArg()
        { 
        
        }
        
    }
}
