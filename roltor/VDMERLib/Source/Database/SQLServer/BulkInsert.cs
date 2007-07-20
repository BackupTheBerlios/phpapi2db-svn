/*
** BulkInsert.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** BulkInsert - in development 
**
** Changes
** -------
**
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace VDMERLib.Database.SQLServer
{
    class BulkInsert
    {
        public BulkInsert()
        { 
            
        }

        virtual public bool InsertData(string sql)
        {
            bool bReturn = true;
            SqlConnection connection = new SqlConnection(m_sDSN);
            SqlCommand command = new SqlCommand(sql, connection);
            try
            {
                connection.Open();
                command.CommandTimeout = 600;
                Console.WriteLine("START DB  " + DateTime.Now.ToString());
                command.ExecuteNonQuery();
                Console.WriteLine("END DB  " + DateTime.Now.ToString());

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString()); 
                bReturn = false;
            }
            finally
            {
                // Clean up....
                command = null;
                connection.Close();
                connection = null;
            }
            return bReturn;
        }

        string m_sDSN = "Data Source=JABY-HOPE;Initial Catalog=BOExchange;Integrated Security=True";
        /// <summary>
        /// Connection String
        /// </summary>
        public string DSN
        {
            get { return m_sDSN; }
            set { m_sDSN = value; }
        }
    }
}
