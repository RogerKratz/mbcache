@echo off
SET ROOTDIR=%~dp0
set ROOTDIR=%ROOTDIR:~0,-7%
SET sn="%programfiles%\Microsoft SDKs\Windows\V6.0A\Bin\sn.exe"
set gacutil="%programfiles%\Microsoft SDKs\Windows\V6.0A\Bin\gacutil.exe"
set msbuild="%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
set configuration=Release
set nugetFolder="%ROOTDIR%\.nuget"
set PackageFolder="%ROOTDIR%\packages"

echo Installing msbuildtasks to %PackageFolder%. Please wait...
%nugetFolder%\NuGet install MsBuildTasks -o %PackageFolder%

echo.
set /p Version=Please enter version number, eg 1.2.0.0: 

%msbuild% default.msbuild /v:q /t:MakeRelease
echo -------------------------------
echo.
echo Updated assemblyinfo files to %Version%.
echo Created a new nuget package to output folder.
echo.
echo Remember to...
echo Commit changes
echo Tag current changeset with version %version%
echo Push changes to server repo

echo.

pause