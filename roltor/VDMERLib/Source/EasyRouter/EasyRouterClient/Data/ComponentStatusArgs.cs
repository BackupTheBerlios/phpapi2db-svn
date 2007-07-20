/*
** ComponentStatusArgs.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** ComponentStatusArgs
**
** Changes
** -------
**
*/

using System;
using System.Collections.Generic;
using System.Text;

using MESSAGEFIX3Lib;

namespace VDMERLib.EasyRouter.EasyRouterClient.Data
{
   
    
    /// <summary>
    /// Class ComponentStatusArgs
    /// </summary>
    public class ComponentStatusArgs : EventArgs
    {
        /// <summary>
        /// easyrouter component status enum
        /// </summary>
        public enum Status
        {
            /// <summary>
            /// unknown
            /// </summary>
            eUnknown = 0,
            /// <summary>
            /// active
            /// </summary>
            eActive = 1,    //GREEN
            /// <summary>
            /// waiting
            /// </summary>
            eWaiting = 2,    //AMBER
            /// <summary>
            /// waiting active
            /// </summary>
            eWaitingActive = 3,    //AMBER GREEN
            /// <summary>
            /// inactive
            /// </summary>
            eInactive = 4,    //RED
            /// <summary>
            /// inactive active
            /// </summary>
            eInactiveActive = 5,    //RED GREEN
            /// <summary>
            /// inactive waiting 
            /// </summary>
            eInactiveWaiting = 6,    //RED AMBER
            /// <summary>
            /// all
            /// </summary>
            eALL = 7     //RED AMBER GREEN -> CONVERTS TO RED AMBER
        }

        /// <summary>
        /// component type
        /// public const string esFIXComponentCategoryOrders = "O";
        /// public const string esFIXComponentCategoryPricesBest = "B";
        /// public const string esFIXComponentCategoryPricesDepth = "D";
        /// public const string esFIXComponentCategoryStructure = "S";
        /// </summary>
        string m_sType = MESSAGEFIX3Lib.FIXComponentCategoryConstants.esFIXComponentCategoryUnknown;

        /// <summary>
        /// Security Exchange code
        /// </summary>
        string m_sExchange = string.Empty;

        /// <summary>
        /// Security Exchange code
        /// </summary>
        public string Exchange
        { 
            get { return m_sExchange; }
        }

        /// <summary>
        /// Security Exchange description
        /// </summary>
        string m_sExchangeDesc = string.Empty;

        /// <summary>
        /// Security Exchange description
        /// </summary>
        public string ExchangeDesc
        {
            get { return m_sExchangeDesc; }
        }      
 
        /// <summary>
        /// component status Status
        /// </summary>
        Status m_eType = 0;

        /// <summary>
        /// component type
        /// public const string esFIXComponentCategoryOrders = "O";
        /// public const string esFIXComponentCategoryPricesBest = "B";
        /// public const string esFIXComponentCategoryPricesDepth = "D";
        /// public const string esFIXComponentCategoryStructure = "S";
        /// </summary>
        public string ComponentType
        {
            get { return m_sType; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sType"></param>
        /// <param name="sExchange"></param>
        /// <param name="sExchangeDesc"></param>
        public ComponentStatusArgs(string sType, string sExchange, string sExchangeDesc)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format("Type[{0}]Exchange[{1}]ExchangeDesc[{2}]",sType,sExchange,sExchangeDesc));  
#endif
            m_sType = sType;
            m_sExchangeDesc = sExchangeDesc;
            m_sExchange = sExchange;
        }

        /// <summary>
        /// Set the component status 
        /// </summary>
        /// <param name="nActive"></param>
        /// <param name="nWaiting"></param>
        /// <param name="nInactive"></param>
        /// <returns></returns>
        public Status SetStatus(int nActive,int nWaiting,int nInactive)
        {
            if( nActive > 0 )
                m_eType |= (Status)nActive;
            if (nWaiting > 0)
                m_eType |= (Status)nWaiting;
            if (nInactive > 0)
                m_eType |= (Status)nInactive;

#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format("Type[{0}]", m_eType));
#endif
            return m_eType;
        }

        /// <summary>
        /// Get the current status
        /// </summary>
        /// <returns></returns>
        public Status GetStatus()
        {
            return m_eType;
        }

        /// <summary>
        /// Get status as user friendly string
        /// </summary>
        /// <returns></returns>
        public string GetStatusAsString()
        { 
            switch(m_eType)
            {
                case Status.eActive :           return "Active";
                case Status.eWaiting:           return "Waiting";
                case Status.eWaitingActive:     return "WaitingActive";
                case Status.eInactive:          return "Inactive";
                case Status.eInactiveActive:    return "InactiveActive";
                case Status.eInactiveWaiting:   return "InactiveWaiting";
                case Status.eALL:               return "ALL";
                default: return "Unknown";
            }
        }

        /// <summary>
        /// Get component type as use friendly string
        /// </summary>
        /// <returns></returns>
        public string GetComponentAsString()
        {
            switch(m_sType)
            {
                case FIXComponentCategoryConstants.esFIXComponentCategoryOrders:        return "Order";
                case FIXComponentCategoryConstants.esFIXComponentCategoryPricesBest:    return "Prices Best";
                case FIXComponentCategoryConstants.esFIXComponentCategoryPricesDepth:   return "Prices Depth";
                case FIXComponentCategoryConstants.esFIXComponentCategoryStructure:     return "Structure";
                default:                                                                return "Unknown";
            }
        }
    }
}
