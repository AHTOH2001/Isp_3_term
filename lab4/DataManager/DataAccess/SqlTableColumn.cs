using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager
{
    public class SqlTableColumn
    {
        public string columnName { get; }
        public  List<object> values { get;}

        public SqlTableColumn(string columnName)
        {
            this.columnName = columnName;
            this.values = new List<object>();
        }
    }
}
