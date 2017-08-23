@echo off
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe ..\Release.sln
if errorlevel 1 (
   echo Failure Reason Given is %errorlevel%
   exit /b %errorlevel%
)
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe TestWaApplication.csproj /pp:MSBuldPreProcess.xml
if errorlevel 1 (
   echo Failure Reason Given is %errorlevel%
   exit /b %errorlevel%
)
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe @@MSBuildCommandLine.txt
