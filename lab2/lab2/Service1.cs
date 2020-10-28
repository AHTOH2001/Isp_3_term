using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Security.Cryptography;
using System.Linq;
using System.IO;

namespace lab2
{
    public partial class Service1 //: ServiceBase
    {
        Watcher watcher;
        //Begin_CONFIG
        static byte[] aesKey;
        static byte[] aesIV;
        //Finish_CONFIG
        public static void Encrypt(string filePath)
        {
            StreamReader streamReader = new StreamReader(filePath);
            byte[] encrypted = Encryptor.EncryptStringToBytes_Aes(streamReader.ReadToEnd(), aesKey, aesIV);
            streamReader.Close();
            StreamWriter streamWriter = new StreamWriter(filePath, false);
            streamWriter.Write(string.Concat(encrypted.Select(x=>(char)x).ToArray()));            
            streamWriter.Close();
            Logger.RecordEntry("зашифрован", filePath);
        }

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
            aesKey = tempAes.Key;
            aesIV = tempAes.IV;
            Logger.LogFilePath = @"D:\Ucheba\Labs\3 sem\Isp\lab2\TargetDirectory\log.txt";
            watcher = new Watcher(@"D:\Ucheba\Labs\3 sem\Isp\lab2\SourceDirectory");
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
