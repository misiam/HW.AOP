using System;
using System.IO;
using System.Linq;
using System.Threading;
using WindowsServices.HW.ImgScanner.Interfaces;
using WindowsServices.HW.Logging;
using WindowsServices.HW.Logging.CodeRewriting;
using WindowsServices.HW.Utils.Props;

namespace WindowsServices.HW.ImgScanner.Services
{
    public class ScannerWorker : IScannerWorker
    {
        private readonly IScanner _scanner;
        private readonly IPdfAggregatorFilesHandler _pdfAggregatorFilesHandler;
        private readonly IStorageService _storageService;
        private readonly ServiceProperties _props;
        private readonly ILogger _logger;

        Thread workingThread;
        ManualResetEvent workStop;
        AutoResetEvent newFile;

        [LoggerAspect]
        public ScannerWorker(IScanner scanner, IPdfAggregatorFilesHandler pdfAggregatorFilesHandler, IStorageService storageService, ServiceProperties props, ILogger logger)
        {
            _scanner = scanner;
            _pdfAggregatorFilesHandler = pdfAggregatorFilesHandler;
            _storageService = storageService;
            _props = props;
            _logger = logger;


            workStop = new ManualResetEvent(false);
            newFile = new AutoResetEvent(false);
            workingThread = new Thread(ScanWork);
        }

        [LoggerAspect]
        public void ScanWork()
        {
            do
            {
                _logger.LogInfo("Scanning... ");
                if (workStop.WaitOne(TimeSpan.Zero))
                    return;

                var files = _scanner.GetFiles();
                var scanChunk = _scanner.ScanFiles(files);



                if (scanChunk.ChunkFiles.Any())
                {
                    if (!string.IsNullOrEmpty(scanChunk.Name))
                    {
                        _pdfAggregatorFilesHandler.Handle(scanChunk.ChunkFiles, _storageService, $"{scanChunk.Name ?? "out"}_{DateTime.UtcNow.ToFileTimeUtc()}.pdf");

                        foreach (var includedFile in scanChunk.ChunkFiles)
                        {
                            _logger.LogInfo(" File.Delete " + includedFile);
                            File.Delete(includedFile);
                        }
                    }
                    else
                    {
                        _logger.LogInfo($"   [{scanChunk.ChunkFiles.Count}] files in a chunk. Waiting for barcode.");
                    }
                }
                else
                {
                    _logger.LogInfo("no files in a chunk");
                }


            }
            while (WaitHandle.WaitAny(new WaitHandle[] { workStop, newFile }, _props.ScanInterval) != 0);
        }


        [LoggerAspect]
        public void StartScan()
        {
            workingThread.Start();
        }

        [LoggerAspect]
        public void StopScanning()
        {
            workingThread.Abort();
        }
    }
}
