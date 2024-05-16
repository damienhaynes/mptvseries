@ECHO OFF
CLS

IF NOT "%1"=="" (
  SET ARCH=%1
) ELSE (
  SET ARCH=x86
)

Title Building TV Series (RELEASE)
CD ..

REM Select program path based on current machine environment
SET progpath=%ProgramFiles%
IF NOT "%ProgramFiles(x86)%".=="". SET progpath=%ProgramFiles(x86)%

REM Define MSbuild path
SET MSBUILD_PATH=%progpath%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe
IF NOT EXIST "%MSBUILD_PATH%" SET MSBUILD_PATH=%progpath%\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe
IF NOT EXIST "%MSBUILD_PATH%" SET MSBUILD_PATH=%progpath%\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\bin\MSBuild.exe
IF NOT EXIST "%MSBUILD_PATH%" SET MSBUILD_PATH=%progpath%\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\bin\MSBuild.exe

:: Build
"%MSBUILD_PATH%" /target:Rebuild /property:Configuration=RELEASE /property:Platform=%ARCH% /fl /flp:logfile=MP-TVSeries.log;verbosity=diagnostic MP-TVSeries.sln

CD build

PAUSE
