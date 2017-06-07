using WindowsServices.HW.ImgScanner.Interfaces;

namespace WindowsServices.HW.ImgScanner.Utils
{
    class StorageServiceFactory
    {
        public static IStorageService GetStorageService(string outputLocation)
        {
            return new LocalFolderStorage(outputLocation);
        }
    }
}
