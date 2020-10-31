# Service installing advices

1. In the file Watcher.cs change paths of files 
2. Move files /yourDirectory/lab2/bin/Debug/lab2.exe and /yourDirectory/lab2/bin/Debug/ServiceLibraries.dll to the other directory (Not necessary, but preferable)
3. Open CMD with administrator rights and execute command: 
"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe" "C:\Services\lab2.exe (path to your service)"
to install service
4. Open task manager in your computer, go to the tab services and find service "ServiceLab2" and at last run it.


After using service you can delete it using command in CMD
"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe" -u "C:\Services\lab2.exe (path to your service)"

If you still have questions read article https://metanit.com/sharp/tutorial/21.2.php 
