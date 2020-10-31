using System.ServiceProcess;
using System.Threading;

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
            Logger.RecordStatus("The service stops...");
            watcher.Stop();
            Thread.Sleep(1000);
            Logger.RecordStatus("The service stopped...");
        }
    }
}
