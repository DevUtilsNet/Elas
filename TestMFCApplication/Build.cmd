@echo off
"D:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe" ..\Release.sln
if errorlevel 1 (
   echo Failure Reason Given is %errorlevel%
   exit /b %errorlevel%
)
"D:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe" TestMFCApplication.vcxproj /pp:MSBuldPreProcess.xml
if errorlevel 1 (
   echo Failure Reason Given is %errorlevel%
   exit /b %errorlevel%
)
"D:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe" @@MSBuildCommandLine.txt
