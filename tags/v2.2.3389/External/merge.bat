@echo off
IF EXIST MP-TVSeries_UNMERGED.dll del MP-TVSeries_UNMERGED.dll
ren MP-TVSeries.dll MP-TVSeries_UNMERGED.dll
ilmerge /out:MP-TVSeries.dll MP-TVSeries_UNMERGED.dll Cornerstone.MP.dll
