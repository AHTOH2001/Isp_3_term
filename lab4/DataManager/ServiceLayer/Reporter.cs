using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager
{
    internal class Reporter
    {
        private string _sourceDirectory;
        private DataAccess _dataAccess;
        private XMLGenerator _xmlGenerator;
        public Reporter(string sourceDirectory, DataAccess dataAccess, XMLGenerator xmlGenerator)
        {
            _sourceDirectory = sourceDirectory;
            _dataAccess = dataAccess;
            _xmlGenerator = xmlGenerator;
        }
        public void CreateNewReport()
        {
            FileTransfer fileTransfer = new FileTransfer();
            var sqlTable = _dataAccess.ExecuteStoredProcedure("[GetEmployeeDepartmentHistory]");
            var xmlContent = _xmlGenerator.generateXML("EmployeeDepartmentHistory", "BusinessEntity", sqlTable);
            fileTransfer.CreateTempFile(xmlContent);            
            fileTransfer.TranferFileToFTP(_sourceDirectory, "report.xml");            
        }
    }
}
