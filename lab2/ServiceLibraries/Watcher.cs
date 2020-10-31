using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace lab2
{
    public class Watcher
    {
        FileSystemWatcher watcher;
        bool enabled = true;
        string watchedFolder;
        Extractor extractor;
        //Begin_CONFIG
        const string logFilePath = @"D:\Ucheba\Labs\3 sem\Isp\lab2\TargetDirectory\log.txt";
        const string sourceDirectory = @"D:\Ucheba\Labs\3 sem\Isp\lab2\SourceDirectory";
        const string targetDirectory = @"D:\Ucheba\Labs\3 sem\Isp\lab2\TargetDirectory";
        //Finish_CONFIG
        public Watcher()
        {
            Logger.logFilePath = logFilePath;
            Logger.RecordStatus("The service starts...");
            var tempAes = Aes.Create();
            var aesKey = tempAes.Key;
            var aesIV = tempAes.IV;
            var extractor = new Extractor(targetDirectory, aesKey, aesIV);
            this.extractor = extractor;
            this.watchedFolder = sourceDirectory;
            watcher = new FileSystemWatcher(watchedFolder);
            watcher.Deleted += Watcher_Deleted;
            watcher.Created += Watcher_Created;
            watcher.Renamed += Watcher_Renamed;
            watcher.IncludeSubdirectories = true;
            Logger.RecordStatus("The service started...");
        }

        public void Start()
        {
            watcher.EnableRaisingEvents = true;
            enabled = true;
            while (enabled)
            {
                Thread.Sleep(1000);
            }
        }
        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
            enabled = false;
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            string fileEvent = "renamed to " + e.FullPath;
            string filePath = e.OldFullPath;
            Logger.RecordEntry(fileEvent, filePath);
        }

        private bool IsNotFolder(string fullPath)
        {
            var fl = new FileInfo(fullPath);
            if (fl.Extension != "") return true;
            else return false;
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "created";
            string filePath = e.FullPath;
            Logger.RecordEntry(fileEvent, filePath);

            if (IsNotFolder(e.FullPath))
            {
                watcher.EnableRaisingEvents = false;
                extractor.Extract(e.FullPath);
                watcher.EnableRaisingEvents = true;
            }
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "deleted";
            string filePath = e.FullPath;
            Logger.RecordEntry(fileEvent, filePath);
        }

    }
}
