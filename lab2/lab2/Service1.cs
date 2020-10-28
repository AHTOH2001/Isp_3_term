using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Security.Cryptography;
using System.Linq;
using System.IO;
using System.IO.Compression;

namespace lab2
{
    public partial class Service1 //: ServiceBase
    {
        Watcher watcher;                       
        //Begin_CONFIG
        const string LogFilePath = @"D:\Ucheba\Labs\3 sem\Isp\lab2\TargetDirectory\log.txt";
        const string sourceDirectory = @"D:\Ucheba\Labs\3 sem\Isp\lab2\SourceDirectory";
        const string targetDirectory = @"D:\Ucheba\Labs\3 sem\Isp\lab2\TargetDirectory";
        //Finish_CONFIG



        //public Service1()
        //{
        //    InitializeComponent();
        //    this.CanStop = true;
        //    this.CanPauseAndContinue = true;
        //    this.AutoLog = true;
        //}

        public void OnStart()
        {
            var tempAes = Aes.Create();
            var aesKey = tempAes.Key;
            var aesIV = tempAes.IV;
            var extractor = new Extractor(sourceDirectory,targetDirectory,aesKey,aesIV); 
            Logger.LogFilePath = LogFilePath;
            watcher = new Watcher(sourceDirectory, extractor);
            Thread watcherThread = new Thread(new ThreadStart(watcher.Start));
            watcherThread.Start();
        }

        //protected override void OnStop()
        //{
        //    logger.Stop();
        //    Thread.Sleep(1000);
        //}
    }
}
