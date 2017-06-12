using System;
using System.ServiceProcess;
using WindowsServices.HW.ImgScanner.Interfaces;
using WindowsServices.HW.ImgScanner.Services;
using WindowsServices.HW.Logging.CodeRewriting;
using WindowsServices.HW.Utils.Props;
using Autofac;
using Autofac.Core;
using Autofac.Extras.DynamicProxy;
using Castle.Components.DictionaryAdapter;
using ILogger = WindowsServices.HW.Logging.ILogger;

namespace WindowsServices.HW.ScanService
{
    public interface IScannerService
    {
        void StartScanning();
        void StopScanning();
    }

    
    public class ScannerService : ServiceBase, IScannerService
    {
        //private readonly IContainer _ioc;
        private readonly IScannerWorker _scanner;
        private readonly ILogger _logger;

        [LoggerAspect]
        public ScannerService(IContainer ioc)
        {
            _logger = ioc.Resolve<ILogger>();
          
            _scanner = ioc.Resolve<IScannerWorker>();
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
