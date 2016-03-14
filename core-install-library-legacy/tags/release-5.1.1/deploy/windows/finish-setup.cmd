@echo off

echo Installing Base extensions...

for %%F in (%*) do call :installPlugIn %%F
goto :EOF

::-----------------------------------------------------------------------------

:installPlugIn

echo   %~n1
echo ----------------------------------------------- >> "%~dpn0_log.txt"
echo %~n1                                            >> "%~dpn0_log.txt"
echo.                                                >> "%~dpn0_log.txt"
bin\Landis.PlugIns.Admin.exe add "..\plug-ins\%~nx1" >> "%~dpn0_log.txt"
