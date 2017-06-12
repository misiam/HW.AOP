using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServices.HW.Utils.Interfaces
{
    public interface IFileSystemHelper
    {
        void CreateDirectoryIfNotExists(params string[] folders);
        bool TryOpen(string fullPath, int tryCount, int sleepTime = 3000);
        IEnumerable<string> GetFiles(string folder, string filePattern = "*.*", string[] allowedExtensions = null);
    }
}
