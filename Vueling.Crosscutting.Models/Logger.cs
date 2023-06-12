using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vueling.Crosscutting.Models
{
    public static class Logger
    {
        private static readonly string logDirectory = "logs";

        public enum Severity
        {
            Info,
            Warning,
            Error,
            Critical
        }

        public static void Log(string message, Severity severity)
        {
            string logFileName = $"{DateTime.Today:yyyy-MM-dd}.log";
            string logFilePath = Path.Combine(logDirectory, logFileName);

            Directory.CreateDirectory(logDirectory);
            File.AppendAllText(logFilePath, $"{DateTime.Now}: [{severity}] {message}\n");
            Console.WriteLine($"{DateTime.Now}: [{severity}] {message}");
        }
    }
}
