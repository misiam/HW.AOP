using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using WindowsServices.HW.Logging.CodeRewriting;

namespace WindowsServices.HW.Utils.Files
{
    public class FileSystemHelper
    {
        [LoggerAspect]
        public static void CreateDirectoryIfNotExists(params string[] folders)
        {
            //TODO
            //_logger.LogInfo("CreateDirectoryIfNotExists: " + folder);
            foreach (var folder in folders)
            {
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
            }
        }

        [LoggerAspect]
        public static bool TryOpen(string fullPath, int tryCount, int sleepTime = 3000)
        {
            for (var i = 0; i < tryCount; i++)
            {
                try
                {
                    File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.None).Close();
                    ;
                    return true;
                }
                catch (IOException)
                {
                    Thread.Sleep(sleepTime);
                }
            }

            return false;
        }

        [LoggerAspect]
        public static IEnumerable<string> GetFiles(string folder, string filePattern = "*.*", string[] allowedExtensions = null)
        {
            var filesToAdd = Directory.GetFiles(folder, filePattern);

            if (allowedExtensions != null && allowedExtensions.Length > 0)
            {
                filesToAdd = filesToAdd.Where(file => allowedExtensions.Any(file.ToLower().EndsWith)).ToArray();
            }
            return filesToAdd.OrderBy(Path.GetFileName).ToList();
        }
    }
}
