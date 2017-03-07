SET D=%~dp0
SET TYPE=scanner
SET OPTS=-t inline
SET IFN=%D%Scanner.l
SET OFN=Scanner.designer.cs
SET OFP=%D%%OFN%
SET TOFP=%TEMP%\%OFN%
SET PP=\local\bin\Lad.exe
CALL :doit
IF ERRORLEVEL 1 EXIT /B 1
EXIT /B 0

:doit
"%PP%" %OPTS% "%IFN%" "%TOFP%"
IF NOT EXIST "%TOFP%" (
	ECHO error creating %TYPE%
	EXIT /B 1
) ELSE IF EXIST "%OFP%" (
	FC "%TOFP%" "%OFP%" > nul
	IF ERRORLEVEL 1 (
		MOVE /Y "%TOFP%" "%OFP%"
		ECHO changed %TYPE%
		EXIT /B 1
	) ELSE (
		DEL "%TOFP%"
		ECHO no change to %TYPE%
	)
) ELSE (
	MOVE /Y "%TOFP%" "%OFP%"
	ECHO created %TYPE%
	EXIT /B 1
)
