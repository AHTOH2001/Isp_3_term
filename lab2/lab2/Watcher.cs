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
        Dictionary<string, FileSystemWatcher> watchers = new Dictionary<string, FileSystemWatcher>();
        bool pause = false;
        bool enabled = true;
        string WatchedFolder;
        public Watcher(string WatchedFolder)
        {
            this.WatchedFolder = WatchedFolder;
            CreateNewWatcher(WatchedFolder);            
        }

        public void Start()
        {
            foreach (var watcher in watchers)
            {
                watcher.Value.EnableRaisingEvents = true;
            }
            enabled = true;
            while (enabled)
            {
                Thread.Sleep(1000);
            }
        }
        public void Stop()
        {
            foreach (var watcher in watchers)
            {
                watcher.Value.EnableRaisingEvents = false;
            }            
            enabled = false;
        }

        // переименовывание файлов
        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (pause)
                return;
            if (IsNotFolder(e.FullPath))
            {

            }
            else
            {                
                watchers.Add(e.FullPath, watchers[e.OldFullPath]);
                watchers.Remove(e.OldFullPath);
            }

            string fileEvent = "переименован в " + e.FullPath;
            string filePath = e.OldFullPath;
            Logger.RecordEntry(fileEvent, filePath);
        }
        // изменение файлов
        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (pause)
                return;
            if (IsNotFolder(e.FullPath))
            {
                string fileEvent = "изменен";
                string filePath = e.FullPath;
                Logger.RecordEntry(fileEvent, filePath);
            }
        }

        private bool IsNotFolder(string fullPath)
        {
            FileInfo fl = new FileInfo(fullPath);
            if (fl.Extension != "") return true;
            else return false;
        }

        // создание файлов
        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            if (pause)
                return;
            if (IsNotFolder(e.FullPath))
            {
                
            }
            else
            {
                
                CreateNewWatcher(e.FullPath);                
            }
            string fileEvent = "создан";
            string filePath = e.FullPath;
            Logger.RecordEntry(fileEvent, filePath);
        }

        // удаление файлов
        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (pause)
                return;

            if (!IsNotFolder(e.FullPath))
            {
                foreach (var watcher in watchers)
                {
                    if (watcher.Value.Path.StartsWith(e.FullPath))
                    {
                        watcher.Value.EnableRaisingEvents = false;
                        watchers.Remove(watcher.Key);
                    }
                    
                }
            }

            string fileEvent = "удален";
            string filePath = e.FullPath;
            Logger.RecordEntry(fileEvent, filePath);
        }

        private void CreateNewWatcher(string fullPath)
        {
            watchers.Add(fullPath, new FileSystemWatcher(fullPath));
            watchers[fullPath].Deleted += Watcher_Deleted;
            watchers[fullPath].Created += Watcher_Created;
            watchers[fullPath].Changed += Watcher_Changed;
            watchers[fullPath].Renamed += Watcher_Renamed;
            watchers[fullPath].EnableRaisingEvents = true;            
        }        
    }
}
