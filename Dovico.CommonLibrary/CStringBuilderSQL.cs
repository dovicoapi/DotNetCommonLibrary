using System;

namespace Dovico.CommonLibrary
{
    public class CStringBuilderSQL : CStringBuilder
    {
        // Default constructor
        public CStringBuilderSQL() { }

        // Overloaded constructors allowing an instance of this object to be created pre-poulated with some SQL (1 line of code rather than 2)
        public CStringBuilderSQL(string sSQL) { base.Append(sSQL); }
        public CStringBuilderSQL(string sSQL, int iValue) { base.Append(sSQL, iValue); }
        public CStringBuilderSQL(string sSQL, long lValue) { base.Append(sSQL, lValue); }
        public CStringBuilderSQL(string sSQL, CDovicoID idValue) { base.Append(sSQL, idValue); }
        public CStringBuilderSQL(string sSQL, double dValue) { base.Append(sSQL, dValue); }
        public CStringBuilderSQL(string sSQL, bool bValue) { base.Append(sSQL, bValue); }
        public CStringBuilderSQL(string sSQL, Guid guidValue) { base.Append(sSQL, guidValue); }
        
        // Overloaded constructors that call the Append functions defined in this class (above they simply call the base class methods)
        public CStringBuilderSQL(string sSQL, string sValue) { this.Append(sSQL, sValue); }
        public CStringBuilderSQL(string sSQL, DateTime dtValue) { this.Append(sSQL, dtValue); }


        // Methods that can be used to add values to this object once an instance of this object has been created
        public void Append(string sString, string sValue)
        {
            base.Append(sString);
            base.Append(sValue.Replace("'", "''"));//Single quotes lead to SQL injection attacks. Escape all single quote characters entered.
        }

        public void Append(string sString, DateTime dtValue)
        {
            base.Append(sString);
            base.Append(dtValue.ToString(Constants.SQL_ISO_8601_DATE_FORMAT));
        }



        // Helper methods for when you build up a SET statement (the Column=Value pairs)
        // NOTE: do not include sqare brackets around the column name value (we do that for you :) )
        public void AppendColumnValue(string sColumnName, string sValue) { AppendColumnValue(sColumnName, sValue, true); }
        public void AppendColumnValue(string sColumnName, DateTime dtValue) { AppendColumnValue(sColumnName, dtValue.ToString(Constants.SQL_ISO_8601_DATE_FORMAT), true); }

        // Helper that handles the task of adding a column/value pair to the SQL
        protected void AppendColumnValue(string sColumnName, string sValue, bool bSurroundValueWithSingleQuote)
        {
            this.Append(("[" + sColumnName + "]=" + (bSurroundValueWithSingleQuote ? "'" : "")), sValue);
            if (bSurroundValueWithSingleQuote) { this.Append("'"); }
        }
    }
}
