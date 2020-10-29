using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace lab2
{
    public class Watcher
    {
        FileSystemWatcher watcher;
        bool enabled = true;
        string WatchedFolder;
        Extractor extractor;
        //Begin_CONFIG
        const string LogFilePath = @"D:\Ucheba\Labs\3 sem\Isp\lab2\TargetDirectory\log.txt";
        const string sourceDirectory = @"D:\Ucheba\Labs\3 sem\Isp\lab2\SourceDirectory";
        const string targetDirectory = @"D:\Ucheba\Labs\3 sem\Isp\lab2\TargetDirectory";
        //Finish_CONFIG
        public Watcher()
        {
            Logger.LogFilePath = LogFilePath;
            Logger.RecordStatus("Сервис запускается...");
            var tempAes = Aes.Create();
            var aesKey = tempAes.Key;
            var aesIV = tempAes.IV;
            var extractor = new Extractor(sourceDirectory, targetDirectory, aesKey, aesIV);
            this.extractor = extractor;
            this.WatchedFolder = sourceDirectory;
            watcher = new FileSystemWatcher(WatchedFolder);
            watcher.Deleted += Watcher_Deleted;
            watcher.Created += Watcher_Created;
            // watcher.Changed += Watcher_Changed;
            watcher.Renamed += Watcher_Renamed;
            watcher.IncludeSubdirectories = true;
            Logger.RecordStatus("Сервис запущен...");
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

        // переименовывание файлов
        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            string fileEvent = "переименован в " + e.FullPath;
            string filePath = e.OldFullPath;
            Logger.RecordEntry(fileEvent, filePath);
        }
        // изменение файлов
        //private void Watcher_Changed(object sender, FileSystemEventArgs e)
        //{
        //    string fileEvent = "изменен";
        //    string filePath = e.FullPath;
        //    Logger.RecordEntry(fileEvent, filePath);
        //}

        private bool IsNotFolder(string fullPath)
        {
            FileInfo fl = new FileInfo(fullPath);
            if (fl.Extension != "") return true;
            else return false;
        }

        // создание файлов
        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "создан";
            string filePath = e.FullPath;
            Logger.RecordEntry(fileEvent, filePath);

            if (IsNotFolder(e.FullPath))
            {
                watcher.EnableRaisingEvents = false;
                extractor.Extract(e.FullPath);
                watcher.EnableRaisingEvents = true;
            }
        }

        // удаление файлов
        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "удален";
            string filePath = e.FullPath;
            Logger.RecordEntry(fileEvent, filePath);
        }

    }
}
