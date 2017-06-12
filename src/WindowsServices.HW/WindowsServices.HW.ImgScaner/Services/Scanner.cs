using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using WindowsServices.HW.ImgScanner.Interfaces;
using WindowsServices.HW.ImgScanner.ModelObjects;
using WindowsServices.HW.Logging;
using WindowsServices.HW.Logging.CodeRewriting;
using WindowsServices.HW.Utils.Interfaces;
using WindowsServices.HW.Utils.Props;
using ZXing;


namespace WindowsServices.HW.ImgScanner.Services
{
    public class Scanner : IScanner
    {
        private readonly IFileSystemHelper _fileSystemHelper;
        private readonly string[] _inputFolders;

        private ILogger _logger;

        [LoggerAspect]
        public Scanner(ScanProperties props, IFileSystemHelper fileSystemHelper, ILogger logger)
        {
            _fileSystemHelper = fileSystemHelper;
            _logger = logger;

            _inputFolders = props.InputLocations;

            fileSystemHelper.CreateDirectoryIfNotExists(_inputFolders);
            fileSystemHelper.CreateDirectoryIfNotExists(props.OutputLocation);
        }

        [LoggerAspect]
        public ScanChunk ScanFiles(IEnumerable<string> files)
        {
            var scanChunk = new ScanChunk();

            foreach (var file in files)
            {
                _logger.LogInfo("   file:" + file);

                if (_fileSystemHelper.TryOpen(file, 3))
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

        [LoggerAspect]
        public string GetBarcodeIfExists(string file)
        {
            var reader = new BarcodeReader { AutoRotate = true };
            using (var bmp = (Bitmap) Bitmap.FromFile(file))
            {
                var result = reader.Decode(bmp);
                bmp.Dispose();

                return result != null && result.Text.StartsWith("SCAN") ? result.Text : null;
            }
        }

        [LoggerAspect]
        public IEnumerable<string> GetFiles(string[] inputFolders = null)
        {
            const string IMG_FILE_PATTERN = "Img_*.*";
            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".bmp" };
            var files = new List<string>();

            foreach (var folder in inputFolders ?? _inputFolders)
            {
                _logger.LogInfo("GetFiles from: " + folder);
                var filesToAdd = _fileSystemHelper.GetFiles(folder, IMG_FILE_PATTERN, allowedExtensions);
                files.AddRange(filesToAdd);
            }
            return files.OrderBy(Path.GetFileName).ToList();
        }


    }
}
