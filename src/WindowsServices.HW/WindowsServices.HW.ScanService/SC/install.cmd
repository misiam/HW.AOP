for %%A in (..\WindowsServices.HW.ScanService.exe) DO set P=%%~fA
sc create WindowsServices.HW.ScanService binPath="%P% -props:inputFolders=C:\winserv\inputs\1;C:\winserv\inputs\2|scanInterval=5000|outputsLocation=C:\winserv\outputs\|logPath=C:\winserv\scanner.log"
	