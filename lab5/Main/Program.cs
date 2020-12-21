using System;
using DataManager;

namespace Main
{
    class Program
    {
        //Main for test but can be used win service
        static void Main(string[] args)
        {
            DataManagerClass dataManager = new DataManagerClass();
            dataManager.Start();
        }
    }
}
