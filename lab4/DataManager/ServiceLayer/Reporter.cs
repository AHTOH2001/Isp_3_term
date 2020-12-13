using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            fileTransfer.TranferFileToFTP(_sourceDirectory, "ReportOfEmployeeDepartmentHistory.xml");

            sqlTable = _dataAccess.ExecuteStoredProcedure("[GetEmployeeWithSpecificOrganisationLevel]", new SqlParameter("@MinOrganisationLevel", 1), new SqlParameter("@MaxOrganisationLevel", 2));
            xmlContent = _xmlGenerator.generateXML("EmployeeOrganisationLevel", "BusinessEntity", sqlTable);
            fileTransfer.CreateTempFile(xmlContent);
            fileTransfer.TranferFileToFTP(_sourceDirectory, "ReportOfEmployeeOrganisationLevel.xml");

            sqlTable = _dataAccess.ExecuteStoredProcedure("[GetEmployeeWithSpecificPayment]", new SqlParameter("@paymentMoreThan", 75));
            xmlContent = _xmlGenerator.generateXML("EmployeePayment", "BusinessEntity", sqlTable);
            fileTransfer.CreateTempFile(xmlContent);
            fileTransfer.TranferFileToFTP(_sourceDirectory, "ReportOfEmployeePayments.xml");

            sqlTable = _dataAccess.ExecuteStoredProcedure("[GetInformationAboutPersons]");
            xmlContent = _xmlGenerator.generateXML("InformationAboutCurrentlyWorkingPersons", "Person", sqlTable);
            fileTransfer.CreateTempFile(xmlContent);
            fileTransfer.TranferFileToFTP(_sourceDirectory, "ReportOfCurrentlyWorkingPersons.xml");
        }
    }
}
