@echo off

if "%programfiles(x86)%XXX"=="XXX" goto 32BIT
    :: 64-bit
    set PROGS=%programfiles(x86)%
    rem pause
    rem echo Current path is %PROGS%	
    goto CONT
:32BIT
    set PROGS=%ProgramFiles%
    rem echo Current path is %PROGS%
    rem pause
:CONT

md tmp
ilmerge /out:tmp\MP-TVSeries.dll MP-TVSeries.dll CookComputing.XmlRpcV2.dll FollwitApi.dll /target:dll /targetplatform:"v4,%PROGS%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0" /wildcards
rem ilmerge /out:tmp\MP-TVSeries.dll MP-TVSeries.dll CookComputing.XmlRpcV2.dll FollwitApi.dll
IF EXIST MP-TVSeries_UNMERGED.dll del MP-TVSeries_UNMERGED.dll
ren MP-TVSeries.dll MP-TVSeries_UNMERGED.dll
IF EXIST MP-TVSeries_UNMERGED.pdb del MP-TVSeries_UNMERGED.pdb
ren MP-TVSeries.pdb MP-TVSeries_UNMERGED.pdb

move tmp\*.* .
rd tmp

