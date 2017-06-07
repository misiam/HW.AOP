using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace WindowsServices.HW.ScanService
{
    [RunInstaller(true)]
    public class ServiceInstaller : Installer
    {
        public ServiceInstaller()
        {
            var process = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalService,

            };
            var service = new System.ServiceProcess.ServiceInstaller
            {
                ServiceName = "HW.ScanService",
                DisplayName = "HW ScanService",
                StartType = ServiceStartMode.Manual
            };

            Installers.Add(process);
            Installers.Add(service);
        }

    }
}
