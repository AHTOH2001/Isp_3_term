# Service installing advices

1. In the file Watcher.cs change paths of files 
2. Move files /yourDirectory/lab3/bin/Debug/lab3.exe, /yourDirectory/lab3/bin/Debug/ServiceLibraries_lab2.dll, /yourDirectory/lab3/bin/Debug/ServiceLibraries_lab3.dll and one of configurations file (xml or json) to the other directory (Not necessary, but preferable)
3. Open CMD with administrator rights and execute command: 
"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe" "C:\Services\lab3.exe (path to your service)"
to install service
4. Open task manager in your computer, go to the tab services and find service "ServiceLab3" and at last run it.


After using service you can delete it using command in CMD
"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe" -u "C:\Services\lab3.exe (path to your service)"

If you still have questions read article https://metanit.com/sharp/tutorial/21.2.php 
or ask question directly to me in telegram @AHTOH007
