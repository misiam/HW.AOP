using System;
using System.Collections.Generic;
using System.IO;


namespace WindowsServices.HW.ImgScanner.Interfaces
{
    public interface IStorageService
    {
        void SaveToStorage(string fileName);
        void SaveToStorage(Stream fileStream, string path);
    }
}
