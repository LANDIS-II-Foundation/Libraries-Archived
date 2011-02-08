rem Batch File to Run a Scenario 
rem The 'rem' keyword indicates that this is a remark

call landis-ii scenario.txt

rem Always use the 'call' keyword before invoking landis-ii in a batch file.
rem The call keyword is necessary because the landis-ii command is itself a batch file.

pause

rem Add a pause so that you can assess whether the scenario ran to completion or whether 
rem it encountered input parameter errors or any other error.

rem You can run many scenarios at once:
rem NOTE:  The following scenario names and directories are for illustrative purposes only.

call landis-ii scenario2.txt

rem To avoid overwriting data, put each scenario into a separate sub-directory:

call landis-ii c:\landis data\scenario1\scenario1.txt
