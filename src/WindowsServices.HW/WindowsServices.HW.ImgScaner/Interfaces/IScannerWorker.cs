using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServices.HW.ImgScanner.Interfaces
{
    public interface IScannerWorker
    {
        void StartScan();
        void StopScanning();
    }
}
