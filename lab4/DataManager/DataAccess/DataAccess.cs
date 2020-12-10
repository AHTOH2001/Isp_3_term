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
        private SystemConfiguration _systemConfiguration;
        private SqlConnection connection;
        public DataAccess(SystemConfiguration systemConfiguration)
        {
            _systemConfiguration = systemConfiguration;            
            var conf = systemConfiguration.GetConfigurationClass(new ServerOptions());
            var connectionString = (string)conf.GetField("connectionString").GetValue(null);
            connection = new SqlConnection(connectionString);
            connection.Open();
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
        public string procWithStoredProcedure1(string procName)
        {
            SqlCommand command = new SqlCommand(procName, connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var values = new object[reader.FieldCount];
                reader.GetValues(values);
                foreach (var val in values)
                {                    
                    Console.WriteLine(val);
                }
            }
            return "";
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
