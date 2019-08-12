@echo off

echo Before creating a release, remember to...
echo * Update release notes
echo.

echo.
set /p Version=Please enter version number, eg 4.1.0: 

dotnet msbuild default.msbuild /t:MakeRelease

echo -------------------------------
echo.
echo Created a new nuget package to output folder.
echo Did NOT push it to nuget.org
echo.

pause