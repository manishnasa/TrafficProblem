using System;
using Common.Logger.Interfaces;

namespace Common.Logger
{
    public class DumbConsoleLogger: ILogger
    {
        DumbConsoleLogger() { }

        static DumbConsoleLogger Instance;
        public static DumbConsoleLogger GetInstance()
        {
            if (Instance == null)
                Instance = new DumbConsoleLogger();

            return Instance;
        }

        public void Info(string message)
        {
            Console.WriteLine("INFO: " + message);
        }

        public void Error(string message)
        {
            Console.WriteLine("ERROR: " + message);
        }
    }
}
