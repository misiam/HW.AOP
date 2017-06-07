using System.IO;
using WindowsServices.HW.ImgScanner.Interfaces;
using WindowsServices.HW.Logging;

namespace WindowsServices.HW.ImgScanner.Utils
{
    class LocalFolderStorage : IStorageService
    {
        private string _outputLocation;
        private ILogger _logger;

        public LocalFolderStorage(string outputLocation)
        {
            _logger = Logger.Current;
            this._outputLocation = outputLocation;
        }

        public void SaveToStorage(string fileName)
        {
            System.IO.File.Move(fileName, Path.Combine(_outputLocation, Path.GetFileName(fileName)));
        }

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
