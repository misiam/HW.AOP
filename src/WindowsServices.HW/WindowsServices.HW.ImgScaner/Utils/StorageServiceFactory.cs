using WindowsServices.HW.ImgScanner.Interfaces;
using WindowsServices.HW.Logging.CodeRewriting;

namespace WindowsServices.HW.ImgScanner.Utils
{
    class StorageServiceFactory
    {
        [LoggerAspect]
        public static IStorageService GetStorageService(string outputLocation)
        {
            return new LocalFolderStorage(outputLocation);
        }
    }
}
