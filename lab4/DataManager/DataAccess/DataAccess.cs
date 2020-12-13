using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigurationManager;

namespace DataManager
{
    public class DataAccess
    {                
        private SqlConnection connection;
        public DataAccess(string connectionString)
        {            
            connection = new SqlConnection(connectionString);
            ConnectToServer();
        }
        private void ConnectToServer()
        {
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("Unable to connect to data base");
                }
            }

        }
        public string proc()
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT TOP (1000) [AddressID]" +
          ",[AddressLine1]" +
          ",[AddressLine2]" +
          ",[City]" +
          ",[StateProvinceID]" +
          ",[PostalCode]" +
          ",[SpatialLocation]" +
          ",[rowguid]" +
          ",[ModifiedDate]" +
           " FROM[AdventureWorks2012].[Person].[Address]";
            command.Connection = connection;
            var res = command.ExecuteReader();
            if (res.Read())
                return (res.GetString(res.GetOrdinal("AddressLine1")));
            else
                throw new Exception("Unable to read");

        }
        public SqlTableColumn[] ExecuteStoredProcedure(string procedureName, params SqlParameter[] sqlParameters)
        {
            SqlCommand command = new SqlCommand(procedureName, connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddRange(sqlParameters);            
            var reader = command.ExecuteReader();
            var fieldCount = reader.FieldCount;
            reader.Close();
            SqlTableColumn[] sqlTable = new SqlTableColumn[fieldCount];
            for (int i = 0; i < fieldCount; i++)
            {                
                reader = command.ExecuteReader();
                sqlTable[i] = new SqlTableColumn(reader.GetName(i));
                while (reader.Read())
                {
                    sqlTable[i].values.Add(reader.GetValue(i));
                }
                reader.Close();
            }

            return sqlTable;
        }
        public string procWithStoredProcedure2(string procName)
        {
            SqlCommand command = new SqlCommand(procName, connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                Console.WriteLine("{0}\t\t{1}\t\t{2}\t\t{3}", reader.GetName(0), reader.GetName(1), reader.GetName(2), reader.GetName(3));

                while (reader.Read())
                {
                    int id = reader.GetInt16(0);
                    string name = reader.GetString(1);
                    string groupName = reader.GetString(2);
                    var modifiedDate = reader.GetDateTime(3);
                    Console.WriteLine("{0}\t\t{1}\t\t{2}\t\t{3}", id, name, groupName, modifiedDate.ToString("dd.MM.yyyy"));
                }
            }
            reader.Close();
            return "";
        }
    }
}
