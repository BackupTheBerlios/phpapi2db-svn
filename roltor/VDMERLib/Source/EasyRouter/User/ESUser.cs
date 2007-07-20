/*
** ESUser.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** 
**
** Changes
** -------
**
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using MESSAGEFIX3Lib;
using EASYROUTERCOMCLIENTLib;

namespace VDMERLib.EasyRouter.User
{
    /// <summary>
    /// Class ESUser
    /// </summary>
    public class ESUser
    {
        /// <summary>
        /// ER client username
        /// </summary>
        private string m_sUserName  = "z";
        /// <summary>
        /// ER cleint Pwd
        /// </summary>
        private string m_sPassword  = "z";

        /// <summary>
        /// Vanilla
        /// </summary>
        public ESUser()
        { 
        
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sPassword"></param>
        /// <param name="sUserName"></param>
        public ESUser(string sPassword,string sUserName)
        {
            m_sUserName = sUserName;
            m_sPassword = sPassword;
        }

        /// <summary>
        /// Username
        /// </summary>
        public string Username
        {
            get { return m_sUserName; }
            set { m_sUserName = value; }
        }

        /// <summary>
        /// Password
        /// </summary>
        public string Password
        {
            get { return m_sPassword; }
            set { m_sPassword = value; }
        }

    

     
       
    }
}
