using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;

namespace ServiceLibraries_lab3
{
    public class SystemConfiguration
    {
        private List<Type> _etlOptions = new List<Type>();
        //public static Options GetOptions<T>()
        //{
        //    Options opt;
        //}
        private string _configurationFilePath;        
        private string GetConfigurationFilePathByName(string configFileName)
        {
            try
            {
                DirectoryInfo baseDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                if (baseDirectory.GetFiles().Select(x => x.Name).Contains(configFileName))
                {
                    return Path.Combine(baseDirectory.FullName, configFileName);
                }
            }
            catch
            {}
            try
            {
                FileInfo nearExe = new FileInfo(configFileName);
                if (nearExe.Exists)
                {
                    return nearExe.FullName;
                }
            }
            catch
            {}
            try
            {
                DirectoryInfo assemblyDirectory = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                if (assemblyDirectory.GetFiles().Select(x => x.Name).Contains(configFileName))
                {
                    return Path.Combine(assemblyDirectory.FullName, configFileName);
                }
            }
            catch
            {}


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
                    xmlParser.Parse(_configurationFilePath, _etlOptions);
                }
            }
            else
            {
                JsonParser.Parse(_configurationFilePath, _etlOptions);
            }
        }        
        public Type GetConfigurationClass<T>(T obj)
        {
            string objectName = Utils.GetAttribute<ConfigurationPseudonymAttribute>(obj).Pseudonym;
            var result = _etlOptions.Find(option => objectName == option.Name);
            if (result == null)
            {
                throw new KeyNotFoundException(string.Format("Configuration \"{0}\" was not found in the configuration file", objectName));
            }
            return result;
        }


    }
}
