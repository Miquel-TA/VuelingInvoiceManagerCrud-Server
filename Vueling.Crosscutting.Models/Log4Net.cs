using System;
using log4net;

namespace Vueling.Crosscutting.Models
{
    public static class Log4Net
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Log4Net));
        private static bool PendingSetup = true;

        public static void Debug(string message)
        {
            CheckIfLoggerIsConfigured();
            Logger.Debug(message);
        }
        public static void Info(string message)
        {
            CheckIfLoggerIsConfigured();
            Logger.Info(message);
        }
        public static void Warn(string message)
        {
            CheckIfLoggerIsConfigured();
            Logger.Warn(message);
        }
        public static void Error(string message)
        {
            CheckIfLoggerIsConfigured();
            Logger.Error(message);
        }
        public static void Fatal(string message)
        {
            CheckIfLoggerIsConfigured();
            Logger.Fatal(message);
        }

        private static void CheckIfLoggerIsConfigured()
        {
            if (PendingSetup)
            {
                Log4NetConfigs.Setup();
                PendingSetup = false;
            }
        }
    }

}
