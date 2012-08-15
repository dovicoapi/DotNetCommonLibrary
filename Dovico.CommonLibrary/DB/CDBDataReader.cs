using System;
using System.Data.SqlClient;

namespace Dovico.CommonLibrary.DB
{
    // Helper class for dealing with database reader objects
    public class CDBDataReader
    {
        protected SqlDataReader m_drDataReader = null;

        public CDBDataReader(SqlDataReader drDataReader) { m_drDataReader = drDataReader; }
        
        public bool Read() { return m_drDataReader.Read(); }
        public void Close() { m_drDataReader.Close(); }


        public string GetString(string sFieldName) { return Get<string>(sFieldName).Trim(); }
        public DateTime GetDate(string sFieldName) { return Get<DateTime>(sFieldName); }
        public double GetDouble(string sFieldName) { return Get<double>(sFieldName); }
        public CDovicoID GetID(string sFieldName) { return Get<long>(sFieldName); }
        public int GetInt(string sFieldName) { return Get<int>(sFieldName); }
        public long GetLong(string sFieldName) { return Get<long>(sFieldName); }
        
        // A generic method for grabbing values from the data reader
        protected T Get<T>(string sFieldName)
        {
            // Will hold our return value
            T ReturnVal = default(T);

            // Grab the field value from the data reader
            object objValue = m_drDataReader[sFieldName];
            if (objValue != DBNull.Value) { ReturnVal = (T)objValue; }

            // Return the requested value to the caller
            return ReturnVal;
        }
    }
}