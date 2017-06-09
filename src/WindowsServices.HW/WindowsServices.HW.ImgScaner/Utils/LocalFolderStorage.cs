using System.IO;
using WindowsServices.HW.ImgScanner.Interfaces;
using WindowsServices.HW.Logging;
using WindowsServices.HW.Logging.CodeRewriting;

namespace WindowsServices.HW.ImgScanner.Utils
{
    class LocalFolderStorage : IStorageService
    {
        private string _outputLocation;
        private ILogger _logger;

        [LoggerAspect]
        public LocalFolderStorage(string outputLocation)
        {
            _logger = Logger.Current;
            this._outputLocation = outputLocation;
        }

        [LoggerAspect]
        public void SaveToStorage(string fileName)
        {
            System.IO.File.Move(fileName, Path.Combine(_outputLocation, Path.GetFileName(fileName)));
        }

        [LoggerAspect]
        public void SaveToStorage(Stream stream, string fileName)
        {
            fileName = Path.Combine(_outputLocation, fileName);
            _logger.LogInfo(" PushChanges: " + fileName);
            using (var fileStream = File.Create(fileName))
            {
                stream.CopyTo(fileStream);
                fileStream.Close();
            }
        }
    }
}
