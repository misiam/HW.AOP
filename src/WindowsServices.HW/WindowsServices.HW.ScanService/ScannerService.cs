using System;
using System.ServiceProcess;
using WindowsServices.HW.ImgScanner.Services;
using WindowsServices.HW.Logging.CodeRewriting;
using WindowsServices.HW.Utils.Props;
using ILogger = WindowsServices.HW.Logging.ILogger;

namespace WindowsServices.HW.ScanService
{
    public class ScannerService : ServiceBase
    {
        private readonly Scanner _scanner;
        private readonly ILogger _logger;

        [LoggerAspect]
        public ScannerService(BaseProperties props)
        {
            _logger = HW.Logging.Logger.Current;

            var logger = HW.Logging.Logger.Current;
            logger.LogInfo("PropsArgs:");
            logger.LogInfo(props.PropsArgs);

            foreach (var property in props.Properties)
            {
                logger.LogInfo(property.Key + "|" + property.Value);
            }

            logger.LogInfo("scanInterval");

            var scanProperties = new ScanProperties(props);
            _scanner = new Scanner(scanProperties);

        }

        [LoggerAspect]
        protected override void OnStart(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            try
            {
                StartScanning();
            }
            catch (Exception e)
            {
                _logger?.LogError(e);
            }
        }

        [LoggerAspect]
        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (_logger != null)
            {
                string senderPart = (sender ?? "[null]").ToString();
                string exceptionPart = e.ToString();
                _logger.LogError("sender: " + senderPart + " | exception: " + exceptionPart);
            }
        }

        [LoggerAspect]
        protected override void OnStop()
        {
            StopScanning();
        }

        [LoggerAspect]
        public void StartScanning()
        {
            _scanner.StartScan();
        }

        [LoggerAspect]
        public void StopScanning()
        {
            _scanner.StopScanning();
        }


    }
}
