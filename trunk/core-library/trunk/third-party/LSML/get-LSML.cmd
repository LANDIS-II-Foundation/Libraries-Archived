@echo off

rem  Run this script in the directory where it's located
pushd %~dp0
setlocal

rem  Get tools for downloading and unpacking zip files on Windows.
set ToolsVer=1.0
set ToolsDir=download
set DownloadTool=%ToolsDir%\Landis.Tools.DownloadFile.exe
set ChecksumTool=%ToolsDir%\checksum.exe
set UnZipTool=%ToolsDir%\unzip.exe
if exist %DownloadTool% (
  echo WinPkgTools already downloaded.
) else (
  .\get-WinPkgTools %ToolsVer% %ToolsDir%
)

rem  Read LSML version # and the SHA1 checksum for that version
for /f "tokens=1,2" %%i in (version.txt) do (
  set LibraryVer=%%i
  set LibrarySHA1=%%j
)

rem  Download the specific library version
set LibraryFileName=LSML-%LibraryVer%.zip
set LibraryURL=http://landis-spatial.googlecode.com/files/%LibraryFileName%
set LibraryPackage=download\%LibraryFileName%
if exist %LibraryPackage% (
  echo %LibraryFileName% already downloaded.
) else (
  echo Downloading %LibraryFileName% ...
  %DownloadTool% %LibraryUrl% %LibraryPackage%
  call :checksum %LibraryPackage% %LibrarySHA1%
  if errorlevel 1 goto :eof
)

rem  Unpack the library if not done already
if exist Landis.SpatialModeling.dll (
  echo Library assemblies have already been unpacked.
) else (
  echo Unpacking %LibraryPackage% ...
  %UnZipTool% %LibraryPackage%
)
goto :eof

rem -------------------------------------------------------------------------

:checksum

set FileToCheck=%1
set ExpectedChecksum=%2

echo Verifying checksum of %FileToCheck% ...
%ChecksumTool% -a SHA1 -c %ExpectedChecksum% -q %FileToCheck%
if %ERRORLEVEL% == -1 (
  echo ERROR: Invalid checksum
  exit /b 1
)

goto :eof

rem -------------------------------------------------------------------------
