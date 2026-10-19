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
IF NOT EXIST "%MSBUILD_PATH%" SET MSBUILD_PATH=%ProgramFiles%\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe
IF NOT EXIST "%MSBUILD_PATH%" SET MSBUILD_PATH=%ProgramFiles%\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe
IF NOT EXIST "%MSBUILD_PATH%" SET MSBUILD_PATH=%ProgramFiles%\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\bin\MSBuild.exe
IF NOT EXIST "%MSBUILD_PATH%" SET MSBUILD_PATH=%ProgramFiles%\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\bin\MSBuild.exe

:: Prepare version
for /f "tokens=*" %%a in ('git rev-list HEAD --count') do set REVISION=%%a 
set REVISION=%REVISION: =%
set /A REVISION=REVISION-1000
"Tools\Tools\sed.exe" -i -r "s/(Assembly(File)?Version\(.[0-9]+\.[0-9]+\.[0-9]+\.)[0-9]+(.\))/\1%REVISION%\3/g" "MP-TVSeries\Properties\AssemblyInfo.cs"

REM Prepare TVDB Legacy API Key
IF DEFINED TVDB_LEGACY_API_KEY (
  ECHO [INFO] TVDB LEGACY API Key found...
  "Tools\Tools\sed.exe" -i -r "s/SECRET_PLACEHOLDER_TVDB_LEGACY_API_KEY/%TVDB_LEGACY_API_KEY%/g" "MP-TVSeries\Online Parsing Classes\Credentials.cs"
)

REM TVDB API Key
IF DEFINED TVDB_API_KEY (
  ECHO [INFO] TVDB API Key found...
  "Tools\Tools\sed.exe" -i -r "s/SECRET_PLACEHOLDER_TVDB_API_KEY/%TVDB_API_KEY%/g" "MP-TVSeries\Online Parsing Classes\Credentials.cs"
)

:: Build
"%MSBUILD_PATH%" /target:Rebuild /property:Configuration=RELEASE /property:Platform=%ARCH% /fl /flp:logfile=MP-TVSeries.log;verbosity=diagnostic MP-TVSeries.sln

:: Revert version
git checkout "MP-TVSeries\Properties\AssemblyInfo.cs"
git checkout "MP-TVSeries\Online Parsing Classes\Credentials.cs"

CD build

PAUSE
