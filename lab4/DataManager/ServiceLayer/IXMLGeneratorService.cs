using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager
{
    interface IXMLGeneratorService
    {
        string generateXML(string tableName, string rowName, SqlTableColumn[] tableContent);
    }
}
