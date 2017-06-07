using System.Collections.Generic;


namespace WindowsServices.HW.ImgScanner.Interfaces
{
    public interface IFilesHandler
    {
        void Handle(IList<string> filesToHandle, IStorageService storageService, string path);
    }
}
