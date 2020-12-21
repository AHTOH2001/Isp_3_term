using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager
{
    interface IXMLGeneratorService
    {
        Task<string> generateXMLAsync(string tableName, string rowName, SqlTableColumn[] tableContent);
    }
}
