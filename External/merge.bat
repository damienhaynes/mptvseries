@echo off
IF EXIST MP-TVSeries_UNMERGED.dll del MP-TVSeries_UNMERGED.dll
ren MP-TVSeries.dll MP-TVSeries_UNMERGED.dll
ilmerge /out:MP-TVSeries.dll MP-TVSeries_UNMERGED.dll ICSharpCode.SharpZipLib.dll CookComputing.XmlRpcV2.dll HtmlAgilityPack.dll Bierdopje.API.dll SubtitleDownloader.dll
