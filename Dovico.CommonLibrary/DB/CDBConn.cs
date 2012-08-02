using System;
using System.Data.SqlClient;

namespace Dovico.CommonLibrary.DB
{
    // Helper class for dealing with database connections
    public class CDBConn
    {
        protected SqlConnection m_scDBConn = null;
        protected SqlTransaction m_stTransaction = null;

        // Overloaded Constructors
        public CDBConn(string sConnectionString) { m_scDBConn = new SqlConnection(sConnectionString); }
        public CDBConn(string sAppName, string sDBServer, string sDBName, string sDBUser, string sDBPassword)
            : this(GetDBConnectionString(sAppName, sDBServer, sDBName, sDBUser, sDBPassword)) { }        

        // Returns the database connection string needed for our local database (used by the default constructor)
        public static string GetDBConnectionString(string sAppName, string sDBServer, string sDBName, string sDBUser, string sDBPassword)
        {
            return "Persist Security Info=false;Connection Timeout=120;Data Source=" + sDBServer +
                ";Application Name="+ sAppName +
                ";User ID=" + sDBUser +
                ";Password=" + sDBPassword +
                ";Initial Catalog=" + sDBName +
                ";MultipleActiveResultSets=true;";
        }


        public void Open() { this.Open(false); }
        public void Open(bool bBeginTransaction) 
        { 
            m_scDBConn.Open();
            if (bBeginTransaction) { BeginTransaction(); }
        }

        public void Close() { m_scDBConn.Close(); }

        // Only Begin a Transaction if we don't already have one!
        public void BeginTransaction() { if (m_stTransaction == null) { m_stTransaction = m_scDBConn.BeginTransaction(); } }
        public void CommitTransaction() { if (m_stTransaction != null) { m_stTransaction.Commit(); m_stTransaction = null; } }
        public void RollbackTransaction() { if (m_stTransaction != null) { m_stTransaction.Rollback(); m_stTransaction = null; } }



        // Helper to return an SqlCommand object when needed by the various functions in this class
        protected SqlCommand GetCommandObject(string sSQL) { return new SqlCommand(sSQL, m_scDBConn, m_stTransaction); }



        // Executes the SQL specified and returns the number of rows affected
        public int ExecuteNonQuery(string sSQL) { return GetCommandObject(sSQL).ExecuteNonQuery();  }

        // Returns a DataReader object for stepping through the returned obtained by the SQL specified
        public CDBDataReader ExecuteReader(string sSQL) { return new CDBDataReader(GetCommandObject(sSQL).ExecuteReader()); }




        public int ExecuteScalarInt(string sSQL) { return ExecuteScalar<int>(sSQL); }
        public CDovicoID ExecuteScalarID(string sSQL) { return ExecuteScalar<long>(sSQL); }

        // A generic method for the ExecuteScalar methods (T is the return type, Y is the default value if we were not able to grab the requested
        // value)
        protected T ExecuteScalar<T>(string sSQL)
        {
            // Will hold our return value
            T ReturnVal = default(T);
            
            // Call ExecuteScalar. If we have a value returned then cast it to our return type.
            object objValue = GetCommandObject(sSQL).ExecuteScalar();
            if ((objValue != DBNull.Value) && (objValue != null)) { ReturnVal = (T)objValue; }

            // Return the requested value to the caller
            return ReturnVal;
        }
        
    }
}