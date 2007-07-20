/*
** Logon.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** Logon 
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
    /// Logon
    /// </summary>
    public class Logon : GeneralMsgEventArg
    {
        // Logon error ids are defined for a few error conditions in the stored procedure dboEATDoUserLogin.prc. Each is equal to
        // vbObjectError + ER_ERROR_BO_DB_OFFSET + DBError - 66000. vbObjectError is -2147221504. ER_ERROR_BO_DB_OFFSET is 40000.
        // DBError is the error code specified in the stored procedure, and is of the form 660xx. NOTE THAT THE ERROR IDS ARE HARD-CODED
        // IN THE STORED PROCEDURE AND ARE SUBJECT TO CHANGE WITHOUT WARNING!
        const int vbObjectError = -2147221504;
        /// <summary>
        /// Possible Login Failure Codes
        /// </summary>
        
        public enum FailureCodes
        {
            TooManyBadLogons = vbObjectError + 40027,
            BadPassword = vbObjectError + 40022,
            AlreadyLoggedIn = vbObjectError + 40021
        }

        public enum ConnectionTypes
        {
            Unknown = 0,
            Dummy = 1,
            Training = 2,
            Live = 3
        }

        private ConnectionTypes m_eConnectionType;

        public ConnectionTypes ConnectionType
        {
            get { return m_eConnectionType; }
            set { m_eConnectionType = value; }
        }

        private string m_sConnectionDescription;

        public string ConnectionDescription
        {
            get { return m_sConnectionDescription; }
            set { m_sConnectionDescription = value; }
        }

        /// <summary>
        /// status of logon request
        /// </summary>
        bool m_bLoginSuccess = false;
        
        /// <summary>
        /// public property for logon status
        /// </summary>
        public bool LoggedOn
        {
            get { return m_bLoginSuccess; }
            set { m_bLoginSuccess = value; }
        }

        /// <summary>
        /// status of logoff request
        /// </summary>
        bool m_bLogoff = false;

        /// <summary>
        /// public property for logoff status
        /// </summary>
        public bool LoggedOff
        {
            get 
            {
                return m_bLogoff; 
            }
            set 
            { 
                m_bLogoff = value; 
            }
        }


        string m_sError = "Logged On";

        /// <summary>
        /// Get or Set Error String
        /// </summary>
        public string Error
        {
            get { return m_sError; }
            set { m_sError = value; }
        }

        int m_iErrorCode = 0;

        /// <summary>
        /// Login Error Codes
        /// </summary>
        public int ErrorCode
        {
            get { return m_iErrorCode; }
            set { m_iErrorCode = value; }
        }

        /// <summary>
        /// Vanilla
        /// </summary>
        public Logon()
            : base(GeneralDataType.Login)
        { 
            
        }

        /// <summary>
        /// To string 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return m_sError.ToString();     
        }
    }
}
