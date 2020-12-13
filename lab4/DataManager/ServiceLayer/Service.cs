using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigurationManager;
using ApplicationInsights;
using System.IO;
using System.Threading;
using Models;

namespace DataManager
{
    public class Service
    {
        private SystemConfiguration _systemConfiguration;
        private bool _enabled = true;
        private string sourceDirectory;
        private string targetDirectory;
        public Service(SystemConfiguration systemConfiguration)
        {
            _systemConfiguration = systemConfiguration;
        }
        public void Start()//Should rename
        {
            Logger.RecordStatus("The service starts...");
            var serverOptions = _systemConfiguration.GetConfigurationClass(new ServerOptions());

            var connectionString = (string)serverOptions.GetField("connectionString").GetValue(null);
            DataAccess dataAccess = new DataAccess(connectionString);            

            var fileOptions = _systemConfiguration.GetConfigurationClass(new WatcherOptions());
            Logger.LogFilePath = fileOptions.GetOption<string>("LogFilePath");

            sourceDirectory = fileOptions.GetOption<string>("SourceDirectory");
            XMLGenerator xmlGenerator = new XMLGenerator();
            Reporter reporter = new Reporter(sourceDirectory, dataAccess, xmlGenerator);
            Logger.RecordStatus("The service started...");
            var reporterOptions = _systemConfiguration.GetConfigurationClass(new ReporterOptions());
            var hours = reporterOptions.GetOption<int>("FrequencyOfReportHours");
            var minutes = reporterOptions.GetOption<int>("FrequencyOfReportMinutes");
            var seconds = reporterOptions.GetOption<int>("FrequencyOfReportSeconds");
            while (_enabled)
            {                
                Thread.Sleep(new TimeSpan(hours, minutes, seconds));
                reporter.CreateNewReport();
            }
        }

        public void Stop()
        {
            _enabled = false;
            Logger.RecordStatus("The service stopped...");
        }

    }
}