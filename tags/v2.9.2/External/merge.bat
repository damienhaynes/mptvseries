@echo off

md tmp
ilmerge /out:tmp\MP-TVSeries.dll MP-TVSeries.dll CookComputing.XmlRpcV2.dll FollwitApi.dll ICSharpCode.SharpZipLib.dll
IF EXIST MP-TVSeries_UNMERGED.dll del MP-TVSeries_UNMERGED.dll
ren MP-TVSeries.dll MP-TVSeries_UNMERGED.dll
IF EXIST MP-TVSeries_UNMERGED.pdb del MP-TVSeries_UNMERGED.pdb
ren MP-TVSeries.pdb MP-TVSeries_UNMERGED.pdb

move tmp\*.* .
rd tmp

