using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using ServiceLibraries_lab3;

namespace lab2
{
    public class Watcher
    {
        static public SystemConfiguration SystemConfiguration;
        private FileSystemWatcher _watcher;
        private bool _enabled = true;
        private bool _isNeedToLogWatcher;
        private string _watchedFolder;
        private Extractor _extractor;
        private Type _watcherOptions;

        public Watcher()
        {
        }

        public void Start()
        {
            SystemConfiguration = new SystemConfiguration(xmlFileName: "config.xml");
            _watcherOptions = SystemConfiguration.GetConfigurationClass(new WatcherOptions());
            Logger.LogFilePath = _watcherOptions.GetOption<string>("LogFilePath");
            try
            {
                _isNeedToLogWatcher = _watcherOptions.GetOption<bool>("NeedToLog");
            }
            catch
            {
                Logger.RecordStatus("Warning, watcher need to log option was not found in the config");
                _isNeedToLogWatcher = true;
            }            
            if (_isNeedToLogWatcher)
            {
                Logger.RecordStatus("System configuration has been injected to the program...");
                Logger.RecordStatus("The service starts...");
            }
            var tempAes = Aes.Create();
            var aesKey = tempAes.Key;
            var aesIV = tempAes.IV;
            var extractor = new Extractor(_watcherOptions.GetOption<string>("TargetDirectory"), aesKey, aesIV);
            this._extractor = extractor;
            this._watchedFolder = _watcherOptions.GetOption<string>("SourceDirectory");
            _watcher = new FileSystemWatcher(_watchedFolder);
            _watcher.Deleted += WatcherDeleted;
            _watcher.Created += WatcherCreated;
            _watcher.Renamed += WatcherRenamed;
            _watcher.IncludeSubdirectories = true;
            _watcher.EnableRaisingEvents = true;
            if (_isNeedToLogWatcher)
            {
                Logger.RecordStatus("The service started...");
            }
            _enabled = true;
            while (_enabled)
            {
                Thread.Sleep(1000);
            }

        }
        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;
            _enabled = false;
        }

        private void WatcherRenamed(object sender, RenamedEventArgs e)
        {
            string fileEvent = "renamed to" + e.FullPath;
            string filePath = e.OldFullPath;
            if (_isNeedToLogWatcher)
            {
                Logger.RecordEntry(fileEvent, filePath);
            }
        }

        private void WatcherCreated(object sender, FileSystemEventArgs e)
        {
            try
            {
                string fileEvent = "created";
                string filePath = e.FullPath;
                if (_isNeedToLogWatcher)
                {
                    Logger.RecordEntry(fileEvent, filePath);
                }
                //Folders shouldn't be extracted
                if (Path.HasExtension(filePath))
                {
                    _watcher.EnableRaisingEvents = false;
                    _extractor.Extract(e.FullPath);
                    _watcher.EnableRaisingEvents = true;
                }
            }
            catch (Exception exception)
            {
                Logger.RecordStatus(exception.Message);
                Stop();
            }
        }

        private void WatcherDeleted(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "deleted";
            string filePath = e.FullPath;
            if (_isNeedToLogWatcher)
            {
                Logger.RecordEntry(fileEvent, filePath);
            }
        }

    }
}
