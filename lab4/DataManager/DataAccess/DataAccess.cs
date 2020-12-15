using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public SqlTableColumn[] ExecuteSqlCommand(string commandText)
        {
            SqlCommand command = new SqlCommand(commandText, connection);
            return GetSqlTable(command);
        }
        public SqlTableColumn[] ExecuteStoredProcedure(string procedureName, params SqlParameter[] sqlParameters)
        {
            SqlCommand command = new SqlCommand(procedureName, connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddRange(sqlParameters);
            return GetSqlTable(command);
        }

        private SqlTableColumn[] GetSqlTable(SqlCommand command)
        {
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
    }
}
