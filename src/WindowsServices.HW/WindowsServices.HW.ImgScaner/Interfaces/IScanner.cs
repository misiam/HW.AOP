using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsServices.HW.ImgScanner.ModelObjects;

namespace WindowsServices.HW.ImgScanner.Interfaces
{
    public interface IScanner
    {
        ScanChunk ScanFiles(IEnumerable<string> files);
        string GetBarcodeIfExists(string file);
        IEnumerable<string> GetFiles(string[] inputFolders = null);
    }
}
