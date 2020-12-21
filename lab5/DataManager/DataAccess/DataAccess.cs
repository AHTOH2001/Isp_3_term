using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataManager
{
    public class DataAccess
    {        
        private string connectionString;
        public DataAccess(string connectionString)
        {            
            this.connectionString = connectionString;
        }        
        public async Task<SqlTableColumn[]> ExecuteSqlCommandAsync(string commandText)
        {
            SqlConnection connection = new SqlConnection();
            await connection.OpenAsync();            
            SqlCommand command = new SqlCommand(commandText, connection);
            var res = await GetSqlTableAsync(command);            
            connection.Close();
            return res;
        }
        public async Task<SqlTableColumn[]> ExecuteStoredProcedureAsync(string procedureName, params SqlParameter[] sqlParameters)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();            
//            Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + ": " + procedureName);            
            SqlCommand command = new SqlCommand(procedureName, connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddRange(sqlParameters);
            var res = await GetSqlTableAsync(command);
            connection.Close();
            return res;
        }

        public async Task ExecuteStoredSetProcedureAsync(string procedureName, params SqlParameter[] sqlParameters)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();            
            SqlCommand command = new SqlCommand(procedureName, connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddRange(sqlParameters);
            await command.ExecuteNonQueryAsync();
            connection.Close();            
        }

        private async Task<SqlTableColumn[]> GetSqlTableAsync(SqlCommand command)
        {                    
            var reader = await command.ExecuteReaderAsync();
            var fieldCount = reader.FieldCount;
            reader.Close();
            SqlTableColumn[] sqlTable = new SqlTableColumn[fieldCount];
            for (int i = 0; i < fieldCount; i++)
            {
                reader = await command.ExecuteReaderAsync();
                sqlTable[i] = new SqlTableColumn(reader.GetName(i));
                while (await reader.ReadAsync())
                {
                    sqlTable[i].values.Add(reader.GetValue(i));
                }
                reader.Close();
            }

            return sqlTable;
        }
    }
}
