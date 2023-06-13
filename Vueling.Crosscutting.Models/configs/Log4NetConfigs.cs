using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;

public static class Log4NetConfigs
{
    public static void Setup()
    {
        Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

        PatternLayout patternLayout = new PatternLayout();
        patternLayout.ConversionPattern = "%date [%thread] %-5level %logger - %message%newline";
        patternLayout.ActivateOptions();

        RollingFileAppender roller = new RollingFileAppender();
        roller.AppendToFile = true;
        roller.File = @"logs\\log_";
        roller.Layout = patternLayout;
        roller.MaxSizeRollBackups = 30;  // keep logs for the last 30 days
        roller.StaticLogFileName = false;  // dynamic Logger file based on date
        roller.DatePattern = "yyyy-MM-dd'.txt'";  // each day has its own Logger file
        roller.RollingStyle = RollingFileAppender.RollingMode.Date;  // roll by date
        roller.ActivateOptions();
        hierarchy.Root.AddAppender(roller);

        ConsoleAppender consoleAppender = new ConsoleAppender();
        consoleAppender.Layout = patternLayout;
        consoleAppender.ActivateOptions();
        hierarchy.Root.AddAppender(consoleAppender);

        hierarchy.Root.Level = log4net.Core.Level.All;
        hierarchy.Configured = true;
    }
}
