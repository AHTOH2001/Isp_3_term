using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager
{
    internal class XMLGenerator : IXMLGeneratorService
    {
        public string generateXML(string tableName, string rowName, SqlTableColumn[] tableContent)
        {
            StringBuilder XMLcontent = new StringBuilder();
            XMLcontent.AppendLine(@"<?xml version = ""1.0"" encoding = ""utf-8"" ?>");
            XMLcontent.AppendLine($"<{tableName}>");
            int rowAmount = tableContent[0].values.Count;
            for (int i = 0; i < rowAmount; i++)
            {
                XMLcontent.AppendLine($"\t<{rowName}>");
                for (int j = 0; j < tableContent.Length; j++)
                {
                    XMLcontent.Append($"\t\t<{tableContent[j].columnName}>");
                    XMLcontent.Append($"{tableContent[j].values[i]}");
                    XMLcontent.AppendLine($"</{tableContent[j].columnName}>");
                }
                XMLcontent.AppendLine($"\t</{rowName}>");
            }
            XMLcontent.AppendLine($"</{tableName}>");
            return XMLcontent.ToString();
        }
    }
}
