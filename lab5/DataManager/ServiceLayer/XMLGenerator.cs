using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager
{
    internal class XMLGenerator : IXMLGeneratorService
    {
        public async Task<string> generateXMLAsync(string tableName, string rowName, SqlTableColumn[] tableContent)
        {
            StringBuilder XMLcontent = new StringBuilder();
            await Task.Run(() => XMLcontent.AppendLine(@"<?xml version = ""1.0"" encoding = ""utf-8"" ?>"));
            await Task.Run(() => XMLcontent.AppendLine($"<{tableName}>"));
            int rowAmount = tableContent[0].values.Count;
            for (int i = 0; i < rowAmount; i++)
            {
                await Task.Run(() => XMLcontent.AppendLine($"\t<{rowName}>"));
                for (int j = 0; j < tableContent.Length; j++)
                {
                    await Task.Run(() => XMLcontent.Append($"\t\t<{tableContent[j].columnName}>"));
                    await Task.Run(() => XMLcontent.Append($"{tableContent[j].values[i]}"));
                    await Task.Run(() => XMLcontent.AppendLine($"</{tableContent[j].columnName}>"));
                }
                await Task.Run(() => XMLcontent.AppendLine($"\t</{rowName}>"));
            }
            await Task.Run(() => XMLcontent.AppendLine($"</{tableName}>"));
            return XMLcontent.ToString();
        }
    }
}
