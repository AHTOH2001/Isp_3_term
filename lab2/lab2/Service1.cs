using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Security.Cryptography;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.ServiceProcess;

namespace lab2
{
    public partial class Service1 : ServiceBase
    {
        Watcher watcher;        

        public Service1()
        {
            InitializeComponent();
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {                   
            watcher = new Watcher();
            Thread watcherThread = new Thread(new ThreadStart(watcher.Start));
            watcherThread.Start();            
        }

        protected override void OnStop()
        {
            Logger.RecordStatus("Сервис останавливается...");
            watcher.Stop();
            Thread.Sleep(1000);
            Logger.RecordStatus("Сервис остановлен...");
        }
    }
}
