/*
** DataReader.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** DataReader - in development 
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

namespace VDMERLib.Database
{
    public class DataReader
    {
        public DataReader()
        { 
        
        }


        public static bool GetInteger(IDataReader reader,int nColumn,out int val)
        {
            try
            {
                val = reader.GetInt32(nColumn);
                return true;
            }
            catch (Exception)
            {
                val = Int32.MinValue;
                return false;
            }
        }

        public static bool GetString(IDataReader reader, int nColumn, out string val)
        {
            try
            {
                val = reader.GetString(nColumn);
                return true;
            }
            catch (Exception)
            {
                val = string.Empty;
                return false;
            }
        }

        public static bool GetChar(IDataReader reader, int nColumn, out char val)
        {
            try
            {
                val = reader.GetString(nColumn)[0];
                return true;
            }
            catch (Exception)
            {
                val = ' ';
                return false;
            }
        }

        public static bool GetDate(IDataReader reader, int nColumn, out DateTime val)
        {
            try
            {
                val = reader.GetDateTime(nColumn);
                return true;
            }
            catch (Exception)
            {
                val = DateTime.MinValue;
                return false;
            }
        }

        public static bool GetDouble(IDataReader reader, int nColumn, out double val)
        {
            try
            {
                val = reader.GetDouble(nColumn);
                return true;
            }
            catch (Exception)
            {
                val = double.NaN; 
                return false;
            }



        }
    }
}
