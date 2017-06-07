using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using WindowsServices.HW.ImgScanner.Interfaces;
using WindowsServices.HW.ImgScanner.ModelObjects;
using WindowsServices.HW.ImgScanner.Utils;
using WindowsServices.HW.Logging;
using WindowsServices.HW.Utils.Files;
using WindowsServices.HW.Utils.Props;
using ZXing;


namespace WindowsServices.HW.ImgScanner.Services
{
    public class Scanner
    {
        private readonly string[] _inputFolders;
        private readonly int _interval;

        private ILogger _logger = Logger.Current;

        Thread workingThread;
        ManualResetEvent workStop;
        AutoResetEvent newFile;

        private IStorageService _storageService;


        public Scanner(ScanProperties props )
        {
            _inputFolders = props.InputLocations;
            _interval = props.ServiceProperties.ScanInterval;

            _storageService = StorageServiceFactory.GetStorageService(props.OutputLocation);

            FileSystemHelper.CreateDirectoryIfNotExists(_inputFolders);
            FileSystemHelper.CreateDirectoryIfNotExists(props.OutputLocation);

            workStop = new ManualResetEvent(false);
            newFile = new AutoResetEvent(false);
            workingThread = new Thread(Scanning);
        }


        private void Scanning()
        {
            do
            {
                _logger.LogInfo("Scanning... ");

                var files = GetFiles(_inputFolders);
                var scanChunk = ScanFiles(files);

                if (workStop.WaitOne(TimeSpan.Zero))
                    return;


                if (scanChunk.ChunkFiles.Any())
                {
                    if (!string.IsNullOrEmpty(scanChunk.Name))
                    {
                        IFilesHandler filesHandler = new PdfAggregatorFilesHandler();
                        filesHandler.Handle(scanChunk.ChunkFiles, _storageService,
                            $"{scanChunk.Name ?? "out"}_{DateTime.UtcNow.ToFileTimeUtc()}.pdf");

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
            while (WaitHandle.WaitAny(new WaitHandle[] { workStop, newFile } , _interval) != 0);
        }

        private ScanChunk ScanFiles(IEnumerable<string> files)
        {
            var scanChunk = new ScanChunk();

            foreach (var file in files)
            {
                _logger.LogInfo("   file:" + file);
                if (workStop.WaitOne(TimeSpan.Zero))
                    return scanChunk;

                if (FileSystemHelper.TryOpen(file, 3))
                {
                    string barcodeText = GetBarcodeIfExists(file);
                    if (!string.IsNullOrEmpty(barcodeText))
                    {
                        File.Delete(file);
                        scanChunk.Name = barcodeText;

                        break;
                    }

                    scanChunk.ChunkFiles.Add(file);
                }
            }
            return scanChunk;
        }

        private string GetBarcodeIfExists(string file)
        {
            var reader = new BarcodeReader { AutoRotate = true };
            using (var bmp = (Bitmap) Bitmap.FromFile(file))
            {
                var result = reader.Decode(bmp);
                bmp.Dispose();

                return result != null && result.Text.StartsWith("SCAN") ? result.Text : null;
            }
        }

        private IEnumerable<string> GetFiles(string[] inputFolders)
        {
            const string IMG_FILE_PATTERN = "Img_*.*";
            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".bmp" };
            var files = new List<string>();

            foreach (var folder in inputFolders)
            {
                _logger.LogInfo("GetFiles from: " + folder);
                var filesToAdd = FileSystemHelper.GetFiles(folder, IMG_FILE_PATTERN, allowedExtensions);
                files.AddRange(filesToAdd);
            }
            return files.OrderBy(Path.GetFileName);
        }
        
        public void StartScan()
        {
            workingThread.Start();
        }

        public void StopScanning()
        {
            workingThread.Abort();
        }
    }
}
