rem Batch File to Run a Scenario 

set name=scenario

if not exist "c:\program files\landis-ii\5.1\examples\%name%" mkdir "c:\program files\landis-ii\5.1\examples\%name%"
cd "c:\program files\landis-ii\5.1\examples\%name%"

copy ..\%name%.txt

if not exist "c:\program files\landis-ii\5.1\examples\%name%\replicate1" mkdir "c:\program files\landis-ii\5.1\examples\%name%\replicate1"
cd "c:\program files\landis-ii\5.1\examples\%name%\replicate1"
copy ..\%name%.txt
call landis %name%.txt

if not exist "c:\program files\landis-ii\5.1\examples\%name%\replicate2" mkdir "c:\program files\landis-ii\5.1\examples\%name%\replicate2"
cd c:\program files\landis-ii\5.1\examples\%name%\replicate2
copy ..\%name%.txt
call landis %name%.txt

if not exist "c:\program files\landis-ii\5.1\examples\%name%\replicate3" mkdir "c:\program files\landis-ii\5.1\examples\%name%\replicate3"
cd "c:\program files\landis-ii\5.1\examples\%name%\replicate3"
copy ..\%name%.txt
call landis %name%.txt


pause
