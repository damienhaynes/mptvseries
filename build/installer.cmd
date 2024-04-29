@echo off
cls
Title Creating TV Series Installer

IF "%programfiles(x86)%XXX"=="XXX" GOTO 32BIT
    :: 64-bit
    SET PROGS=%programfiles(x86)%
    GOTO CONT
:32BIT
    SET PROGS=%ProgramFiles%
:CONT

IF NOT EXIST "%PROGS%\Team MediaPortal\MediaPortal\" SET PROGS=C:

:: Get version from DLL
FOR /F "tokens=*" %%i IN ('..\Tools\Tools\sigcheck.exe /accepteula /nobanner /n "..\MP-TVSeries\bin\Release\MP-TVSeries.dll"') DO (SET version=%%i)

:: Temp xmp2 file
COPY ..\MPEI\MP-TVSeries.xmp2 ..\MPEI\MP-TVSeriesTemp.xmp2

:: Build MPE1
CD ..\MPEI
"%PROGS%\Team MediaPortal\MediaPortal\MPEMaker.exe" ..\MPEI\MP-TVSeriesTemp.xmp2 /B /V=%version% /UpdateXML
CD ..\build

:: Cleanup
del ..\MPEI\MP-TVSeriesTemp.xmp2
