/*
** StructureDataEventArg.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** StructureDataEventArg - base class for all instrument data classes
**
** Changes
** -------
**
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace VDMERLib.EasyRouter.Structure
{
    /// <summary>
    /// Class StructureDataType
    /// </summary>
    public enum StructureDataType
    { 
        /// <summary>
        /// unknown
        /// </summary>
        Unknown,
        /// <summary>
        /// Commodity
        /// </summary>
        Commodity,
        /// <summary>
        /// security exchange
        /// </summary>
        SecurityExchange,
        /// <summary>
        /// teinstrument
        /// </summary>
        TEInstrument,
        /// <summary>
        /// esexchange
        /// </summary>
        ESExchange,
        /// <summary>
        /// end of security responses
        /// </summary>
        EndOfResponses
    }
    
    /// <summary>
    /// StructureDataEventArg
    /// </summary>
    public class StructureDataEventArg
    {
        /// <summary>
        /// Data Type
        /// </summary>
        StructureDataType m_Datatype = StructureDataType.Unknown;

        /// <summary>
        /// Get Type
        /// </summary>
        public StructureDataType DataType
        {
            get { return m_Datatype; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public StructureDataEventArg(StructureDataType type)
        {
            m_Datatype = type;
        }
    }
}
