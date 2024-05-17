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

:: Prepare version
for /f "tokens=*" %%a in ('git rev-list HEAD --count') do set REVISION=%%a 
set REVISION=%REVISION: =%
set /A REVISION=REVISION-1000
"Tools\Tools\sed.exe" -i -r "s/(Assembly(File)?Version\(.[0-9]+\.[0-9]+\.[0-9]+\.)[0-9]+(.\))/\1%REVISION%\3/g" "MP-TVSeries\Properties\AssemblyInfo.cs"

:: Build
"%MSBUILD_PATH%" /target:Rebuild /property:Configuration=RELEASE /property:Platform=%ARCH% /fl /flp:logfile=MP-TVSeries.log;verbosity=diagnostic MP-TVSeries.sln

:: Revert version
git checkout "MP-TVSeries\Properties\AssemblyInfo.cs"

CD build

PAUSE
