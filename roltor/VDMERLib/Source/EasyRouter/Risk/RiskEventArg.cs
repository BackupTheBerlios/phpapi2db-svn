using System;
using System.Collections.Generic;
using System.Text;

namespace VDMERLib.EasyRouter.Risk
{
    
    /// <summary>
    /// PricesEventArg
    /// </summary>
    public class RiskEventArg : Base
    {
        /// <summary>
        /// PriceDataType
        /// </summary>
        public enum RiskDataType
        {
            /// <summary>
            /// account level
            /// </summary>
            Account,
            /// <summary>
            ///position level
            /// </summary>
            Position
        }
        /// <summary>
        /// Risk type
        /// </summary>
        protected RiskDataType m_RiskType = RiskDataType.Position;

        /// <summary>
        /// Data type
        /// </summary>
        public RiskDataType DataType
        {
            get { return m_RiskType; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public RiskEventArg(RiskDataType type)
        {
            m_RiskType = type;
        }

        /// <summary>
        /// Vanilla
        /// </summary>
        public RiskEventArg()
        {

        }
    }    
}
