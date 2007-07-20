/*
** GeneralMsgEventArg.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** GeneralMsgEventArg - base clas for generic messages  
**
** Changes
** -------
**
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace VDMERLib.EasyRouter.General
{
        /// <summary>
        /// Class GeneralMsgEventArg
        /// </summary>
        public class GeneralMsgEventArg
        {
            /// <summary>
            /// GeneralDataType
            /// </summary>
            public enum GeneralDataType
            {
                /// <summary>
                /// unknown
                /// </summary>
                Unknown,
                /// <summary>
                /// login
                /// </summary>
                Login
            }

            /// <summary>
            /// get type
            /// </summary>
            GeneralDataType m_Datatype = GeneralDataType.Unknown;

            /// <summary>
            /// get type
            /// </summary>
            public GeneralDataType DataType
            {
                get { return m_Datatype; }
            }

            /// <summary>
            /// vanilla
            /// </summary>
            /// <param name="type"></param>
            public GeneralMsgEventArg(GeneralDataType type)
            {
                m_Datatype = type;
            }
        }

}
