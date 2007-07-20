/*
** IStoredProcedure.cs
** Copyright (c) 2007 JABYSoft
**
** Developer
** Ying Kiu Chan
**
** Description
** -----------
** IStoredProcedure - in development 
**
** Changes
** -------
**
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace VDMERLib.Database
{
    interface IStoredProcedure
    {
        string DSN
        {
            get;
            set;
        }

        int TIMEOUT
        {
            get;
            set;
        }

        bool Open();
        void Init();
        void Close();

        IDataReader ExecuteReader();
        void ExecuteNonQuery();

        void AddVarcharParameter(string sParameterName, string sVal, int size);
        void AddTextParameter(string sParameterName, string sVal);
        void AddIntegerParameter(string sParameterName, int nVal);
        void AddTinyIntParameter(string sParameterName, int nVal);
        void AddBitParameter(string sParameterName, int nVal);
        void AddBitParameter(string sParameterName, bool bVal);
        void AddDoubleParameter(string sParameterName, double dVal);
        void AddDecimalParameter(string sParameterName, decimal dVal);
        void AddDateTimeParameter(string sParameterName, DateTime dtVal);
        void AddCharParameter(string sParameterName, char cVal);
    }
}
