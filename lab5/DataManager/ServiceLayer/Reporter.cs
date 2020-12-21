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
        public async void CreateNewReportAsync()
        {
            ApplicationInsights.Logger.RecordStatusAsync("Report process started...");
            FileTransfer fileTransfer = new FileTransfer();

            var task1 = Task.Run(async () =>
            {
                var sqlTable = await _dataAccess.ExecuteStoredProcedureAsync("[GetEmployeeDepartmentHistory]");
                var xmlContent = await _xmlGenerator.generateXMLAsync("EmployeeDepartmentHistory", "BusinessEntity", sqlTable);
                await fileTransfer.CreateTempFileAsync(xmlContent);
                await fileTransfer.TranferFileToFTPAsync(_sourceDirectory, "ReportOfEmployeeDepartmentHistory.xml");
            });

            var task2 = Task.Run(async () =>
            {
                var sqlTable = await _dataAccess.ExecuteStoredProcedureAsync("[GetEmployeeWithSpecificOrganisationLevel]", new SqlParameter("@MinOrganisationLevel", 1), new SqlParameter("@MaxOrganisationLevel", 2));
                var xmlContent = await _xmlGenerator.generateXMLAsync("EmployeeOrganisationLevel", "BusinessEntity", sqlTable);
                await fileTransfer.CreateTempFileAsync(xmlContent);
                await fileTransfer.TranferFileToFTPAsync(_sourceDirectory, "ReportOfEmployeeOrganisationLevel.xml");
            });
            
            var task3 = Task.Run(async () =>
            {                
                var sqlTable = await _dataAccess.ExecuteStoredProcedureAsync("[GetEmployeeWithSpecificPayment]", new SqlParameter("@paymentMoreThan", 75));                
                var xmlContent = await _xmlGenerator.generateXMLAsync("EmployeePayment", "BusinessEntity", sqlTable);                
                await fileTransfer.CreateTempFileAsync(xmlContent);
                await fileTransfer.TranferFileToFTPAsync(_sourceDirectory, "ReportOfEmployeePayments.xml");                
            });

            var task4 = Task.Run(async () =>
            {                
                var sqlTable = await _dataAccess.ExecuteStoredProcedureAsync("[GetInformationAboutPersons]");                
                var xmlContent = await _xmlGenerator.generateXMLAsync("InformationAboutCurrentlyWorkingPersons", "Person", sqlTable);
                await fileTransfer.CreateTempFileAsync(xmlContent);
                await fileTransfer.TranferFileToFTPAsync(_sourceDirectory, "ReportOfCurrentlyWorkingPersons.xml");                
            });
            

            await Task.WhenAll(new[] { task1, task2, task3, task4 });            

            ApplicationInsights.Logger.RecordStatusAsync("Report process ended...");
        }
    }
}
