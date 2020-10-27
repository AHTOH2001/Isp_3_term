using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace lab2
{
    public partial class Service1 //: ServiceBase
    {
        Logger logger;
        Watcher watcher;
        //public Service1()
        //{
        //    InitializeComponent();
        //    this.CanStop = true;
        //    this.CanPauseAndContinue = true;
        //    this.AutoLog = true;
        //}

        public void OnStart()
        {
            logger = new Logger(@"D:\Ucheba\Labs\3 sem\Isp\lab2\TargetDirectory\log.txt");
            watcher = new Watcher(@"D:\Ucheba\Labs\3 sem\Isp\lab2\SourceDirectory");
            Thread loggerThread = new Thread(new ThreadStart(watcher.Start));
            loggerThread.Start();
        }

        //protected override void OnStop()
        //{
        //    logger.Stop();
        //    Thread.Sleep(1000);
        //}
    }
}
