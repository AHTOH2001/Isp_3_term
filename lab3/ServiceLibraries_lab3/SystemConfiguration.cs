using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ServiceLibraries_lab3
{
    public class SystemConfiguration
    {
        private List<Type> _etlOptions = new List<Type>();
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
            _configurationFilePath = GetConfigurationFilePathByName(jsonFileName ?? Utils.GetAttribute<JsonFileNameAttribute>(this)?.Name);
            if (_configurationFilePath == null)
            {
                _configurationFilePath = GetConfigurationFilePathByName(xmlFileName ?? Utils.GetAttribute<XmlFileNameAttribute>(this)?.Name);
                if (_configurationFilePath == null)
                {
                    throw new FileNotFoundException("Configuration file was not found");
                }
                else
                {
                    XmlParser.Parse(_configurationFilePath, _etlOptions);                    
                }
            }
            else
            {
                JsonParser.Parse(_configurationFilePath, _etlOptions);
            }
        }
        public Type GetConfigurationClass<T>(T obj)
        {
            var objectName = Utils.GetAttribute<ConfigurationPseudonymAttribute>(obj)?.Pseudonym ?? obj.GetType().Name;
            var result = _etlOptions.Find(option => objectName == option.Name);
            if (result == null)
            {
                throw new KeyNotFoundException(string.Format("Configuration \"{0}\" was not found in the configuration file", objectName));
            }
            return result;
        }
    }
}
