using ApplicationInsights;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ConfigurationManager
{
    public class SystemConfiguration
    {
        private XmlParser _xmlParser = new XmlParser();
        private JsonParser _jsonParser = new JsonParser();
        private List<Type> _etlOptions = new List<Type>();
        private Utils _utils = new Utils();
        private string _configurationFilePath;
        private string GetConfigurationFilePathByName(string configFileName)
        {
            try
            {
                var baseDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                if (baseDirectory.GetFiles().Select(x => x.Name).Contains(configFileName))
                {
                    return Path.Combine(baseDirectory.FullName, configFileName);
                }
            }
            catch
            {
            }
            try
            {
                var nearExe = new FileInfo(configFileName);
                if (nearExe.Exists)
                {
                    return nearExe.FullName;
                }
            }
            catch
            {
            }
            try
            {
                var assemblyDirectory = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                if (assemblyDirectory.GetFiles().Select(x => x.Name).Contains(configFileName))
                {
                    return Path.Combine(assemblyDirectory.FullName, configFileName);
                }
            }
            catch
            {
            }


            return null;
        }
        [JsonFileName("appsettings.json")]
        public SystemConfiguration(string jsonFileName = null, string xmlFileName = null)
        {
            if (!_etlOptions.Equals(new List<Type>()))
            {
                _configurationFilePath = GetConfigurationFilePathByName(jsonFileName ?? _utils.GetAttribute<JsonFileNameAttribute>(this)?.Name);
                if (_configurationFilePath == null)
                {
                    _configurationFilePath = GetConfigurationFilePathByName(xmlFileName ?? _utils.GetAttribute<XmlFileNameAttribute>(this)?.Name);
                    if (_configurationFilePath == null)
                    {
                        throw new FileNotFoundException("Configuration file was not found");
                    }
                    else
                    {
                        _xmlParser.Parse(_configurationFilePath, _etlOptions);
                    }
                }
                else
                {
                   _jsonParser.Parse(_configurationFilePath, _etlOptions);
                }
                Logger.RecordStatusAsync("System configuration has been injected to the program...");
            }
        }
        public Type GetConfigurationClass<T>(T obj)
        {
            var objectName = _utils.GetAttribute<ConfigurationPseudonymAttribute>(obj)?.Pseudonym ?? obj.GetType().Name;
            var result = _etlOptions.Find(option => objectName == option.FullName);
            if (result == null)
            {
                throw new KeyNotFoundException(string.Format("Configuration \"{0}\" was not found in the configuration file", objectName));
            }
            return result;
        }
    }
}
