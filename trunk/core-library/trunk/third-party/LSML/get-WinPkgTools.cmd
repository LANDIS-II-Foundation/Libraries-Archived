@echo off

setlocal
if "%1" == "" (
  echo Error: No version number specified
  goto :usage
)
set WinPkgToolsVersion=%1
if "%2" == "" (
  echo Error: No directory specified for downloaded files
  goto :usage
)
set DownloadDir=%2
rem Don't use spaces in name

set RootUrl=https://landis-spatial.googlecode.com/svn/tags/tools/WinPkg
rem Note the "https" above; Google closes the connection if it's "http".
rem Substitute the URL above if retrieving from a different site.

set SetupName=WinPkgTools-setup.exe
set UrlOfSetupExe=%RootUrl%/%WinPkgToolsVersion%/%SetupName%

rem Tools will be retrieved and set up in a subfolder.
if not exist %DownloadDir% (
  echo Making "%DownloadDir%" ...
  mkdir %DownloadDir%
)

rem Use VBscript to download the setup program.  Requires full path to
rem the local file.
set DownloadedSetup=%CD%\%DownloadDir%\%SetupName%
if exist %DownloadedSetup% goto :runSetup
echo Downloading %SetupName% into %DownloadDir%\ ...
cscript /nologo download-file.vbs %UrlOfSetupExe% %DownloadedSetup%

:runSetup
rem Run the self-extracting executable.
echo Setting up WinPkgTools in %DownloadDir% ...
pushd %DownloadDir%
.\%SetupName%
popd
goto :eof

rem ----------------------------------------------------------------------

:usage
echo Usage: %0 #.# DIR
exit /b 1
