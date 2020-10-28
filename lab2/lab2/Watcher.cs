using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;

namespace lab2
{
    class Watcher
    {
        FileSystemWatcher watcher;        
        bool enabled = true;
        string WatchedFolder;
        Extractor extractor;
        public Watcher(string WatchedFolder,Extractor extractor)
        {
            this.extractor = extractor;
            this.WatchedFolder = WatchedFolder;
            watcher = new FileSystemWatcher(WatchedFolder);
            watcher.Deleted += Watcher_Deleted;
            watcher.Created += Watcher_Created;
           // watcher.Changed += Watcher_Changed;
            watcher.Renamed += Watcher_Renamed;
            watcher.IncludeSubdirectories = true;            
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
