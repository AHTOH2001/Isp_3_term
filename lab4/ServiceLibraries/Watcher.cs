using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace lab2
{
    public class Watcher
    {
        private FileSystemWatcher _watcher;
        private bool _enabled = true;
        private string _watchedFolder;
        private Extractor _extractor;
        //Begin_CONFIG
        private const string logFilePath = @"D:\Ucheba\Labs\3 sem\Isp\lab2\TargetDirectory\log.txt";
        private const string sourceDirectory = @"D:\Ucheba\Labs\3 sem\Isp\lab2\SourceDirectory";
        private const string targetDirectory = @"D:\Ucheba\Labs\3 sem\Isp\lab2\TargetDirectory";
        //Finish_CONFIG

        public Watcher()
        {
            Logger.LogFilePath = logFilePath;
            Logger.RecordStatus("The service starts...");
            var tempAes = Aes.Create();
            var aesKey = tempAes.Key;
            var aesIV = tempAes.IV;
            var extractor = new Extractor(targetDirectory, aesKey, aesIV);
            this._extractor = extractor;
            this._watchedFolder = sourceDirectory;
            _watcher = new FileSystemWatcher(_watchedFolder);
            _watcher.Deleted += WatcherDeleted;
            _watcher.Created += WatcherCreated;
            _watcher.Renamed += WatcherRenamed;
            _watcher.IncludeSubdirectories = true;
            Logger.RecordStatus("The service started...");
        }

        public void Start()
        {
            _watcher.EnableRaisingEvents = true;
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
            Logger.RecordEntry(fileEvent, filePath);
        }

        private void WatcherCreated(object sender, FileSystemEventArgs e)
        {
            try
            {
                string fileEvent = "created";
                string filePath = e.FullPath;
                Logger.RecordEntry(fileEvent, filePath);
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
            Logger.RecordEntry(fileEvent, filePath);
        }

    }
}
