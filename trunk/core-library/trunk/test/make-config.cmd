rem @echo off

rem  Make a configuration file for NUnit test assembly, by copying a
rem  template configuration file, and substituting the text "TEST_DATA_DIR"
rem  in the template with the actual path to the test data directory.
rem
rem  Usage: {this-script} {path-to-config-file} {path-to-data-dir}
rem
rem  {path-to-data-dir} is the relative path to the data directory from
rem  the directory containing {this script}.

setlocal
set configFile=%~1
if exist "%configFile%" goto :fileExists

set scriptDir=%~dp0
rem Trim trailing backslash
set scriptDir=%scriptDir:~0,-1%

set configTemplate=%scriptDir%\template.config

set dataDir=%~2
set dataDirPath=%scriptDir%\%dataDir%

set replaceTextScript=%scriptDir%\replace-text.vbs

cscript //nologo "%replaceTextScript%" TEST_DATA_DIR "%dataDirPath%" < "%configTemplate%" > "%configFile%"
if errorlevel 1 (
  echo Error creating configuration file: "%configFile%"
  if exist "%configFile%" del "%configFile%"
  exit /b 1
)
echo Created configuration file: "%configFile%"
goto :eof

::-------------------------------------------------------------------

:fileExists
echo Configuration file already exists: "%configFile%"
goto :eof
