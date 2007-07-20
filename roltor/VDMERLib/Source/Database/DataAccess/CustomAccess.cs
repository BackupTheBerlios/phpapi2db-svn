using System;
using System.Collections.Generic;
using System.Text;
using VDMERLib.Database.SQLServer;

namespace VDMERLib.Database.DataAccess
{
    /// <summary>
    /// APJ allows writing of Trade info into a db source
    /// </summary>
    public class CustomAccess : ABCStoredProcedure
    {
        /// <summary>
        /// Insert record
        /// </summary>
        string SP_InsertTick = "InsertTick";

        /// <summary>
        /// vanilla construtor
        /// </summary>
        /// <param name="sDSN"></param>
        public CustomAccess(string sDSN)
		{
            this.DSN = sDSN;
		}

        /// <summary>
        /// Insert tick data
        /// </summary>
        /// <param name="sUsername"></param>
        /// <param name="dtTime"></param>
        /// <param name="sDataName"></param>
        /// <param name="dTick"></param>
        /// <returns></returns>
        public bool InsertTick(string sUsername,DateTime dtTime,string sDataName,double dTick)
		{
            bool bRetVal = true;
            this.m_sStoredProcedureName = SP_InsertTick;
			
            this.Open(true);

            try
            {
                this.AddVarcharParameter("Username", sUsername, 50);
                this.AddDateTimeParameter("DataTime", dtTime);
                this.AddVarcharParameter("DataName", sDataName,155);
                this.AddDoubleParameter("DataValue", dTick);
                this.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());  
            }
            finally 
            {
                this.Close();
            }

			return bRetVal;
		}
    }
}
