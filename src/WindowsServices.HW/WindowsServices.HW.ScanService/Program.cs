using System;
using WindowsServices.HW.Utils.Props;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace WindowsServices.HW.ScanService
{
    class Program
    {
        private static void Main(string[] args)
        {

            var logFactory = GetLogFactory(args, @"C:\winserv\scanner.log");

            var logger = HW.Logging.Logger.Current;
            logger.SetActualLogger(logFactory.GetLogger("HW.ScanService"));

            logger.LogInfo("Main");
            foreach (var arg in args)
            {
                logger.LogInfo(arg);
            }


            var props = BaseProperties.GetProperties(args);
            if (args.Length > 0 && args[0].Equals("console"))
            {
                var serv = new ScannerService(props);
                serv.StartScanning();

                Console.ReadKey();
                serv.StopScanning();
            }
            else //windows service
            {
                try
                {
                    ScannerService.Run(new ScannerService(props));
                }
                catch (Exception e)
                {

                    if (logger != null)
                    {
                        logger.LogError(e);
                    }
                }
            }
        }
        private static LogFactory GetLogFactory(string[] args, string defaultPath)
        {

            string logPath = LogBaseProperties.GetLogPath(args, defaultPath);
            var logConfig = new LoggingConfiguration();

            var target = new FileTarget()
            {
                Name = "Def",
                FileName = logPath,
                Layout = "${date} ${message} ${onexception:inner=${exception:format=toString}}"
            };

            logConfig.AddTarget(target);
            logConfig.AddRuleForAllLevels(target);
            var consoleTarget = new ConsoleTarget
            {
                Layout = "${date} ${message} ${onexception:inner=${exception:format=toString}}",
                Name = "console"
            };


            logConfig.AddTarget(consoleTarget);
            logConfig.AddRuleForAllLevels(consoleTarget);

            var logFactory = new LogFactory(logConfig);

            return logFactory;
        }
    }
}
