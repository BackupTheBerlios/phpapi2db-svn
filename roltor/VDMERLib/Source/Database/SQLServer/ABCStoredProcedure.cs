/*
** ABCStoredProcedure.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** ABCStoredProcedure - in development 
**
** Changes
** -------
**
*/


using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient; //SQLServer connection
using System.Data;

using VDMERLib.Database; //IStoredProcedure

namespace VDMERLib.Database.SQLServer
{
    /// <summary>
    /// in development 
    /// </summary>
    public abstract class ABCStoredProcedure : IStoredProcedure
    {
        /// <summary>
        /// Connection string
        /// </summary>
        protected string m_sDSN = string.Empty;

        int m_nTimeOut = 1800;

        /// <summary>
        /// Database connection objects
        /// </summary>
        protected SqlConnection m_sqlConnection = null;
        /// <summary>
        /// Database Command object
        /// </summary>
        protected SqlCommand m_sqlCommand = null;

        /// <summary>
        /// name of procedure
        /// </summary>
        protected string m_sStoredProcedureName;

        /// <summary>
        /// Vanilla
        /// </summary>
        public ABCStoredProcedure()
        {
            //Empty Constructor
        }

        /// <summary>
        /// Vanilla
        /// </summary>
        ~ABCStoredProcedure()
        {
            Close();
        }

        /// <summary>
        /// Open connection
        /// </summary>
        /// <param name="bGlobal">Reuse connection</param>
        /// <returns></returns>
        public bool Open(bool bGlobal)
        {
            bool bRetVal = false;
            if (bGlobal)
            {
                if (m_sqlConnection == null)
                {
                    if (m_sDSN != string.Empty)
                    {
                        m_sqlConnection = new SqlConnection(m_sDSN);
                        m_sqlConnection.Open();
                        Init();
                        bRetVal = true;
                    }
                }
                else 
                {
                    if (m_sqlConnection.State != ConnectionState.Closed)
                        Init();
                    else
                    {
                        m_sqlConnection.Open();
                        Init();
                        bRetVal = true;
                    }
                }
            }
            else
                bRetVal = Open(); 
            return bRetVal;
        }

        /// <summary>
        /// open connection
        /// </summary>
        /// <returns></returns>
        public bool  Open()
        {
            bool bRetVal = false;
            if (m_sDSN != string.Empty)
            {
                m_sqlConnection = new SqlConnection(m_sDSN);
                m_sqlConnection.Open();
                Init();
                bRetVal = true;
            }
            return bRetVal;
        }

        /// <summary>
        /// intialise sql objects
        /// </summary>
        public void Init()
        {
            m_sqlCommand = new SqlCommand(m_sStoredProcedureName, m_sqlConnection);
            m_sqlCommand.CommandType = CommandType.StoredProcedure;
            m_sqlCommand.CommandTimeout = 1800;
        }

        /// <summary>
        /// Close sql connection
        /// </summary>
        public void Close()
        {
            if (m_sqlConnection != null)
                m_sqlConnection.Close();
            m_sqlConnection = null;
        }   

        /// <summary>
        /// Connection String
        /// </summary>
        public string DSN
        {
            get { return m_sDSN; }
            set { m_sDSN = value; }
        }

        /// <summary>
        /// Timeout
        /// </summary>
        public int TIMEOUT
        {
            get { return m_nTimeOut; }
            set { m_nTimeOut = value; }
        }

        /// <summary>
        /// Returns Data from Database
        /// </summary>
        /// <returns></returns>
        public IDataReader ExecuteReader()
        {
            SqlDataReader reader = null;
            reader = m_sqlCommand.ExecuteReader();
            return reader;
        }

        /// <summary>
        /// No Record Set Returned
        /// </summary>
        public void ExecuteNonQuery()
        {
            m_sqlCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// add char
        /// </summary>
        /// <param name="sParameterName"></param>
        /// <param name="sVal"></param>
        /// <param name="size"></param>
        public void AddVarcharParameter(string sParameterName, string sVal, int size)
        {
            m_sqlCommand.Parameters.Add(sParameterName, SqlDbType.VarChar, size);
            m_sqlCommand.Parameters[sParameterName].Value = sVal;
        }

        /// <summary>
        /// add text
        /// </summary>
        /// <param name="sParameterName"></param>
        /// <param name="sVal"></param>
        public void AddTextParameter(string sParameterName, string sVal)
        {
            m_sqlCommand.Parameters.Add(sParameterName, SqlDbType.Text);
            m_sqlCommand.Parameters[sParameterName].Value = sVal;
        }

        /// <summary>
        /// add int
        /// </summary>
        /// <param name="sParameterName"></param>
        /// <param name="nVal"></param>
        public void AddIntegerParameter(string sParameterName, int nVal)
        {
            m_sqlCommand.Parameters.Add(sParameterName, SqlDbType.Int);
            m_sqlCommand.Parameters[sParameterName].Value = nVal;
        }

        /// <summary>
        /// add tiny int
        /// </summary>
        /// <param name="sParameterName"></param>
        /// <param name="nVal"></param>
        public void AddTinyIntParameter(string sParameterName, int nVal)
        {
            m_sqlCommand.Parameters.Add(sParameterName, SqlDbType.TinyInt);
            m_sqlCommand.Parameters[sParameterName].Value = nVal;
        }

        /// <summary>
        /// add bit
        /// </summary>
        /// <param name="sParameterName"></param>
        /// <param name="nVal"></param>
        public void AddBitParameter(string sParameterName, int nVal)
        {
            m_sqlCommand.Parameters.Add(sParameterName, SqlDbType.Bit);
            m_sqlCommand.Parameters[sParameterName].Value = nVal;
        }

        /// <summary>
        /// add bit
        /// </summary>
        /// <param name="sParameterName"></param>
        /// <param name="bVal"></param>
        public void AddBitParameter(string sParameterName, bool bVal)
        {
            m_sqlCommand.Parameters.Add(sParameterName, SqlDbType.Bit);
            m_sqlCommand.Parameters[sParameterName].Value = bVal;
        }

        /// <summary>
        /// add double
        /// </summary>
        /// <param name="sParameterName"></param>
        /// <param name="dVal"></param>
        public void AddDoubleParameter(string sParameterName, double dVal)
        {
            m_sqlCommand.Parameters.Add(sParameterName, SqlDbType.Float);
            m_sqlCommand.Parameters[sParameterName].Value = dVal;
        }

        /// <summary>
        /// add decimal
        /// </summary>
        /// <param name="sParameterName"></param>
        /// <param name="dVal"></param>
        public void AddDecimalParameter(string sParameterName, decimal dVal)
        {
            m_sqlCommand.Parameters.Add(sParameterName, SqlDbType.Decimal);
            m_sqlCommand.Parameters[sParameterName].Value = dVal;
        }

        /// <summary>
        /// add datetime 
        /// </summary>
        /// <param name="sParameterName"></param>
        /// <param name="dtVal"></param>
        public void AddDateTimeParameter(string sParameterName, DateTime dtVal)
        {
            m_sqlCommand.Parameters.Add(sParameterName, SqlDbType.DateTime);
            m_sqlCommand.Parameters[sParameterName].Value = dtVal;
        }

        /// <summary>
        /// add var char
        /// </summary>
        /// <param name="sParameterName"></param>
        /// <param name="cVal"></param>
        public void AddCharParameter(string sParameterName, char cVal)
        {
            m_sqlCommand.Parameters.Add(sParameterName, SqlDbType.Char);
            m_sqlCommand.Parameters[sParameterName].Value = cVal;
        }

        /// <summary>
        /// clear param
        /// </summary>
        public void ClearParameters()
        {
            m_sqlCommand.Parameters.Clear();
        }
    }
}
