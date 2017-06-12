using System;
using WindowsServices.HW.ImgScanner.Interfaces;
using WindowsServices.HW.ImgScanner.Services;
using WindowsServices.HW.ImgScanner.Utils;
using WindowsServices.HW.Logging.DynamicProxy;
using WindowsServices.HW.Utils.Files;
using WindowsServices.HW.Utils.Interfaces;
using WindowsServices.HW.Utils.Props;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
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
            var props = BaseProperties.GetProperties(args);
            var logProps = new LogBaseProperties(props);

            logger.SetActualLogger(logFactory.GetLogger("HW.ScanService"), logProps.UseCodeRewritingLogs);


            logger.LogInfo("Main");
            foreach (var arg in args)
            {
                logger.LogInfo(arg);
            }

            var scanProperties = new ScanProperties(props);
            var container = GetContainer(scanProperties, logProps);
            
            var serv = new ScannerService(container);

            if (args.Length > 0 && args[0].Equals("console"))
            {
                serv.StartScanning();

                Console.ReadKey();
                serv.StopScanning();
            }
            else //windows service
            {
                try
                {
                    ScannerService.Run(serv);
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

        private static IContainer GetContainer(ScanProperties parameters, LogBaseProperties logBaseProperties)
        {
            var builder = new ContainerBuilder();

            builder.Register(c => new LoggerInterceptor(logBaseProperties.UseDynamicProxyLogs)).Named<IInterceptor>("logger-interceptor");


            builder.RegisterType<ScannerWorker>().As<IScannerWorker>().EnableInterfaceInterceptors().InterceptedBy("logger-interceptor");
            builder.RegisterType<Scanner>().As<IScanner>().EnableInterfaceInterceptors().InterceptedBy("logger-interceptor");
            builder.RegisterType<FileSystemHelper>().As<IFileSystemHelper>().EnableInterfaceInterceptors().InterceptedBy("logger-interceptor");
            builder.RegisterType<PdfAggregatorFilesHandler>().As<IPdfAggregatorFilesHandler>().EnableInterfaceInterceptors().InterceptedBy("logger-interceptor");
            builder.RegisterType<LocalFolderStorage>().As<IStorageService>().EnableInterfaceInterceptors().InterceptedBy("logger-interceptor");

            builder.RegisterInstance(HW.Logging.Logger.Current).As<WindowsServices.HW.Logging.ILogger>();
            builder.RegisterInstance(parameters).As<ScanProperties>();
            builder.RegisterInstance(parameters.ServiceProperties).As<ServiceProperties>();
            builder.RegisterInstance(logBaseProperties).As<LogBaseProperties>();


            var container = builder.Build();
            return container;
        }
    }
}
