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
set /p NugetVersion=Please enter nuget version number, eg 1.2: 

%msbuild% default.msbuild /v:q /t:MakeRelease

hg revert -C %ROOTDIR%\MbCache.ProxyImpl.Castle\Properties\AssemblyInfo.cs
hg revert -C %ROOTDIR%\MbCache.ProxyImpl.LinFu\Properties\AssemblyInfo.cs
hg revert -C %ROOTDIR%\MbCache\Properties\AssemblyInfo.cs
hg revert -C %ROOTDIR%\MbCacheTest\Properties\AssemblyInfo.cs
hg revert -C %ROOTDIR%\MbCacheNet4Test\Properties\AssemblyInfo.cs

echo -------------------------------
echo.
echo Updated assemblyinfo files to %Version%.
echo Created a new nuget package to output folder.
echo.
echo Remember to...
echo * Tag current changeset with version %version%
echo * Update release notes
echo * Push changes to server repo
echo * Push nuget package to nuget server (and symbol server)

echo.

pause