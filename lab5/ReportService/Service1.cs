using DataManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReportService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
        }
        static DataManagerClass dataManager;
        protected override void OnStart(string[] args)
        {
            dataManager = new DataManagerClass();
            Thread dataManagerThread = new Thread(new ThreadStart(dataManager.Start));
            dataManagerThread.Start();
        }

        protected override void OnStop()
        {
            dataManager.Stop();
            Thread.Sleep(1000);
        }
    }
}
