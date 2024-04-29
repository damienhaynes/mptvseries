# What is MP-TVSeries #

[![MP AnyCPU](https://img.shields.io/badge/MP-AnyCPU-blue?logo=windows&logoColor=white)](https://github.com/Mediaportal-Plugin-Team/mptvseries/releases)
[![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/Mediaportal-Plugin-Team/mptvseries/build.yml?logo=github)](https://github.com/Mediaportal-Plugin-Team/mptvseries/actions)
[![GitHub](https://img.shields.io/github/license/Mediaportal-Plugin-Team/mptvseries?color=blue)](https://github.com/Mediaportal-Plugin-Team/mptvseries/blob/master/LICENSE)
[![GitHub release (latest SemVer including pre-releases)](https://img.shields.io/github/v/release/Mediaportal-Plugin-Team/mptvseries?include_prereleases)](https://github.com/Mediaportal-Plugin-Team/mptvseries/releases)
[![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/downloads-pre/Mediaportal-Plugin-Team/mptvseries/latest/total?label=release@downloads)](https://github.com/Mediaportal-Plugin-Team/mptvseries/releases)
[![StandWithUkraine](https://raw.githubusercontent.com/vshymanskyy/StandWithUkraine/main/badges/StandWithUkraine.svg)](https://github.com/vshymanskyy/StandWithUkraine/blob/main/docs/README.md)

MP-TVSeries is a popular TV Series plug-in for MediaPortal, an open-source media center application. It focuses on managing the user's TV Series library with minimal user interaction, allowing for a more user friendly and ease of use experience.

The MP-TVSeries plugin will scan your hard drive (including network and removable drives) for video files, it then analyzes them by their path structures to determine if they are TV Shows. If the file(s) are recognized then the plugin will go online to thetvdb.com and retrieve information about them. You can then browse, manage and play your episodes from inside MediaPortal in a nice graphical layout.

This Plugin does not play Live TV

# MPTVSeries Window ID #
  * 9811

# Questions, Suggestions & Bug Reports #

There are 3 ways you can receive support for the plugin, Submitting log files and or Screenshots is recommended

> ## Mediaportal Forum ##
MP-TVSeries has a dedicated [sub-forum](http://forum.team-mediaportal.com/my-tvseries-162/), try searching as you question may have been answered, if not please create a new thread

> ## Issue Tracker ##
MP-TVSeries has a issue tracker [here](https://github.com/damienhaynes/mptvseries/issues). Recommended if you have a re-producible bug or enhancement request

> ## IRC ##
Join the #mp-tvseries IRC channel on Freenode

> ## Submitting Useful Log Files ##
If you require assitance its smart to include the log files with your post, you will likely find the community will more often look at your issue if you provide all the necessary information. Instructions are below.

  * Launch **MediaPortal Debug-Mode** shortcut from your Windows Start Menu (Team MediaPortal/MediaPortal)
  * Select the second option _Report a bug to a Plugin Developer or a Skin Designer_
  <img src="https://cloud.githubusercontent.com/assets/16772925/20226509/a81480c6-a848-11e6-8d6e-9f1d7bff347d.jpg" with="400" height="400">
  * Click _Proceed_, (Mediaportal will now automatically run)
  * **Reproduce the issue** in MP-TVSeries, do what is needed to show the problem, reproduce the issue only
  * Close MediaPortal
  * A **MediaPortalLogs\_Date\_Time.zip** file will be automatically created on your desktop. Attach this file to your thread/post
---

# Integration with Social Media Websites #

MP-TVSeries optionally integrates with the [trakt.tv](http://trakt.tv) website, which are built around sharing with your friends what shows and movies you are into and watching.

One of the advantages of  beeing connected to [trakt.tv](http://trakt.tv) is the syncronisation of your watch flags and ratings, which we recommend. This is very useful if you have multiple HTPC's, or if you need to start your tv series database from scratch.

You can use any or all of the sites at the same time.

## Trakt.tv ##

  * From version 2.8.1+, the Trakt.tv support will be moved to its own plugin called [Trakt for Mediaportal](https://trakt.tv/a/mediaportal). This plugin will be included with the MP-TVSeries installer should you wish to install it or you can download MPEI package at any time.

> ### Configuration After Install ###

  * Load Mediaportal Configuration, Plugins.
  * Load _Trakt_ which is under _Windows Plugins_
  * Type in your trakt.tv username and password
  * Check the box for MP-TVSeries and any optional plugin
  * Click OK
  <img src="https://cloud.githubusercontent.com/assets/16772925/20227301/1dde1de6-a84c-11e6-98ba-632cc61a3cfe.jpg" with="400" height="400">
---

# File Naming Conventions #
MP-TVSeries supports various naming conventions, if you want a perfect match without any user interaction, it's best to name the Series as it is shown on thetvdb.com. If the Show contains a : such as CSI: Miami you could use: _CSI Miami_ or _CSI- Miami_

> ## Examples Of Supported Filenames ##
Here are a few random examples which will parse.
  * _Alias.S01E01.avi_
  * _Alias - S01E01 - Pilot.avi_
  * _Alias.1x1.avi_
  * _Alias - 1x1 - Pilot.avi_
  * _Alias 01x01.mkv_

> ## Series with Specials ##
Specials need to be labeled as Season 0, Lets take _Top Gear Polar Special_ for example, find _Top Gear_ on thetvdb and click on [Specials](http://www.thetvdb.com/?tab=season&seriesid=74608&seasonid=22070&lid=7). We can see in the Specials list, its labeled as Episode 49, so a filename for that could be
  * _Top Gear - S00E49 - Polar Special.avi_
  * _Top Gear - 0x49.mkv_

> ## Original Series And Remakes ##
If you have a series which has been rebooted, lets take _Battlestar Galatica_ for example, there is the Original, a 1980's version and a 2003 version, How do you name these? go to [thetvdb.com](http://www.thetvdb.com), search _Battlestar Galatica_. We need to name our files the same way. note each series has a different show id comfirming they are all different series

  * _Battlestar Galactica  - S01E01.avi  (match as original series)_
  * _Battlestar Galactica (1980) - S01E01.avi  (match as 1980's series)_
  * _Battlestar Galactica (2003) - S01E01.avi  (match as 2003 series)_

If a series was already imported incorrectly in Mediaportal, highlight the series, press _F9_ or _Info_ on remote, then press _Force new online lookup for series_. This will prompt you to select a series or manually enter a search term.

> ## Mini-Series ##
A standalone Mini Series, like _Dune_, will usually have the _part 1_, _part 2_ etc listed at _Season 1_, _Episode 1_, _Episode 2_ etc on TheTVDB. If for example, the Mini Series lead to a TV Show, such as _BattleStar Galactica (2003)_ that Mini Series may be listed in _Specials / Season 0_.

The Solution is to check the Mini-Series on [thetvdb.com](http://thetvdb.com), match your files names to their syntax. Examples:

  * _Frank Herbert's Dune - S01E01.avi_
  * _Frank Herbert's Dune - S01E02.avi_
  * _Frank Herbert's Dune - S01E03.avi_
  * _Battlestar Galactica (2003) - S00E01 - MiniSeries(1).avi_
  * _Battlestar Galactica (2003) - S00E02 - MiniSeries(2).avi_

> ## Single File - Double Episode ##
MP-TVSeries can support 2 episodes in a single file. More than 2 is not currently supported

See examples below:

  * _Alias.1x01.1x02.avi_
  * _Alias S01E01-E02.avi_
  * _Alias S01E01 + S01E02.avi_
  * _Alias - s01e01 - s01e02.mkv_
  * _Alias #1.01 + #1.02.avi_

If you start playback on _1x01_ from _Alias - S01E01-E02.avi_, then both episodes will be played, the end result is both _episode 1_ and _2_ will be marked as watched. You can still start playback on _1x02_, but it will start at the beginning of the file (but this is what one would expect anyway). If you start _1x01_, but stop at the halfway point where _1x02_ begins, then both _1x01+02_ show as unwatched and when you try to restart playback you will get the "resume from" dialog.

> ## Single Episode - Multiple Files ##
MP-TVSeries is does not currently support combining multiple files into a single episode, It will show those files as duplicates in the episode list

We recommend in this scenario to join the files together, Click the following links for example programs for [AVI files](http://www.alexander-noe.com/video/amg/) and for [MKV files](http://www.bunkus.org/videotools/mkvtoolnix/).

> ## Episodes with Air Date ##
MP-TVSeries now supports AirDate parsing, matched up from episodes listed at [thetvdb.com](http://thetvdb.com)
Some examples below are episodes which will now parse by default - Note: requires atleast v2.9.3 for MP 1.1 or v3.0.5 for MP 1.2B

  * _Conan.(2010).2010.11.08.Baa.Baa.Blackmail.mkv_
  * _Conan.(2010).2011.07.20.Hell.Hath.No.Cell.Phone.Reception.mkv_
  * _The.Colbert.Report.2011.07.27.Dr.Missy.Cummings.mkv_

> ## Auto Renaming of Files ##
MP-TVSeries does not currently support the auto renaming of your files. However their are free 3rd party tools such as [TheRenamer](http://www.theRenamer.com), [Tv Rename](http://www.tvrename.com), [Media Centre Master](http://www.mediacentermaster.com) which you could use to organise/rename the files in your collection.

> ## Folder Structure ##
There are many variations you could use, below is a way that works well with the renaming tools above.
  * TV  --> Series Name  --> Season # ---> 30 Rock - S01E01 - Pilot.avi
---

# Episode Ordering - Online Matching #
MP-TVSeries has options to change the episode ordering for a series, you will only see options available if its applicable to that series.

> ## Air Date ##
This is the default option, shows will be ordered by the date they were aired.

> ## DVD Order ##
Choose this option to have your episodes listed in the order they were released on DVD, DVD ordering if often used to correct minor, sometimes major changes in episode ordering that was initially aired on TV.

> ## Title ##
This matches episode by title.  Example: _firefly - 1x01 - Serenity.mkv_. In this case will be parsed as _Serenity_ and will attempt to find an online match with episodes that have that title. This may be useful for _Specials_ which keep on getting re-ordered online.

> ## Absolute Order ##
This option can only be selected on a Series, not a Season. Some series, like _Anime_ may not be broken in Seasons. In order to accommodate this _Absolute Order_ can be used.

> ## Changing Matching Method ##
You can initially select these options on your MP TV-Series Configuration, or you can change it inside Mediaportal
  * In Mediaportal - Select the _Series_ or _Season_
  * Press _F9_ on keyboard or _INFO_ on MCE Remote
  * Choose _ACTIONS_
  * Choose _CHANGE ONLINE MATCHING METHOD_
---

# Download Episode Information for Whole Series #
To download all the available information for your Series, including Episodes you may not have, Episodes that have not aired yet, and display these episodes inside Mediaportal, do either or the following to enable. note: all episode info will be downloaded next import run.

> ## Option 1: Enable in Configuration ##
  * Load MP-TVSeries Plugin Configuration
  * _OnlineData Tab_
  * Check the box __Download Episode Information for the whole Series__
  * The next import may take a while as it downloads all the available information for all your imported Series_
  <img src="https://cloud.githubusercontent.com/assets/16772925/20229406/7df7986a-a856-11e6-8c7d-26cedd030d13.jpg" with="150" height="100">

> ## Option 2: Enable in Mediaportal ##
  * In Mediaporal, MP-TVSeries Plugin
  * Press _F9_ on keyboard or _INFO_ on MCE Remote
  * Click _OPTIONS_
  * Change __DOWNLOAD ALL EPISODE INFO__ to __ON__

> ## Show / Hide ##
  * In Mediaporal, MP-TVSeries Plugin
  * Press _F9_ on keyboard or _INFO_ on MCE Remote
  * Click _OPTIONS_
  * Change __SHOW ALL EPISODES__ to __ON__

You will now see all episodes for a Series, Depending on Skin, those episodes may show in a different colour, e.g. _Orange_ in _Blue3Wide_ & _StreamedMP_. If a Season only shows a partial amount of all the expected episodes, then those episodes havent been added yet on [thetvdb.com](http://thetvdb.com).
---

# Importing #

> ## Imported Episode Image is Incorrect ##
If you find a episode which has a incorrect episode image, it may be been incorrectly uploaded at [thetvdb.com](http://thetvdb.com). When the image is replaced with a correct version, MP-TVSeries will not automatically download this, however you can do this manually.

_Note: MP-TVSeries v2.7+ will show a FanArt episode image if there is no episode image available, which will be updated automatically, and not to be confused with an Incorrect Episode Image._

> ## Update Images ##
  * In Mediaportal, Select the _Episode_
  * Press _F9_ on keyboard, or _INFO_ on MCE remote
  * Click __ACTIONS__
  * Click __UPDATE__

> ## Importing Videos Not At TheTVDB ##
MP-TVSeries is not intended for this scenario, but there is a solution you can use. If you have videos that you want to show in MP-TVSeries, and its not appropriate to create it at [thetvdb.com](http://thetvdb.com), such as personal movies recorded on video cameras etc, you can try the following - Recommend that you use MP-TVSeries v2.9 or higher.

  * Create proper filenames with season and episode syntax e.g.
    * _My Trip to Fiji - S01E01.avi_
    * _My Trip to Fiji - S01E02.avi_
    * _My Trip to Fiji - S01E03.avi_
    * _My Trip to Fiji - S01E04.avi_

  * Load Media Portal Configuration, MP-TVSeries plugin.
  * Start the Import Wizard, and select Skip when no match is found for the _My Trip to Fiji_
  * Finish Import
  * Select _Details_ tab.
  * Find your Series, Enter Series and Episode info (This will create USEREDIT fields in database)
  * Right Click on series and select _Ignore on Scan_ so we dont prompt to match series everytime an import is run.
  * Enter GUI and select series, then play!

By default MP-TVSeries will automatically generate episode titles if they dont exist online e.g _Episode 1_, _Episode 2_ etc. If this behaviour is not wanted you can manually edit the Options Table in the database and set **AutoGenerateEpisodeTitles** to Zero. This change will only take affect for **NEW** episodes added to database, if you want to respect episode titles from filenames for existing files that have been import then you need to clear the database and re-import.
---

# Custom Artwork #
Typically if you are importing videos not available on [theTVDB.com](http://thetvdb.com) such as sports videos, then you will most likely want to add some artwork. Follow these instructions to get custom artwork in tvseries:

  * In your thumbs\MPTVSeriesBanners\ folder create the **Show Name** if it doesnt already exist (this should match your series name, replace invalid filename characters with underscore).
  * In the **episodes** folder create artwork in the following format: custom-{seasonindex}x{episodeindex}.jpg e.g. **custom-2001x1.jpg**
  * In the **seasons** folder create artwork in the following format: custom-{seasonindex}.jpg e.g **custom-2011.jpg**
  * In the **widebanner** and **posters** folders create artwork in the following format: **custom.jpg**
  * For **fanart**, you must first find the **Online Series ID** which is located in the configuration details tab for the selected series. This is typically a negative number e.g. **-70**. Now simply drop a fanart file into your fanart directory e.g. **thumbs\Fan Art\fanart\original\-70.jpg**

Now in Configuration, select the series and pick the **custom** artwork from the drop down list.
---

# Subtitle Support #
MP-TVSeries suppors the [SubCentral](http://forum.team-mediaportal.com/forums/subcentral.544/) Plugin.

> ## Installation ##
  * [Download](http://www.team-mediaportal.com/extensions/movies-videos/subcentral) the SubCentral Plugin
  * Check SubCentral Plugin is enabled in MP Configuration, Plugins
  * in MP-TVSeries Configuration, Click General tab
  * (Optional) Tick _Offer to download subtitles before playing (SubCentral)_ if you want to perform an automatic subtitle search before playing an episode that does not have any subtitles.

> ## Download a Subtitle ##
  * In the Episode list, highlight the _episode_ you want to download a subtitle
  * Press _F9_ on keyboard or _INFO_ on MCE Remote
  * Select __SUBTITLES...__
  * Find Select and Download the required Subtitle
  * Return to the TV Series Plugin

> ## Turn on Subtitles ##
  * Play An Episode
  * Press _Y_ on keyboard or press _INFO_ on MCE remote, select _Subtitles_
  * Choose the language of subitle if available
  * Highlight __ENABLED__ and press _OK_
  * Return to episode, press _Back_ on MCE Remote or _ESC_ on keyboard
  * __TIP:__ Press Yellow on MCE remote to toggle subtitles on / off
---

# MP-TVSeries File Locations #
The following are the locations of the files and folders used by MP-TVSeries, should you need to backup / restore or just have a look around

> ## MP-TVSeries.dll ##
This is the plugin itself which is used by Mediaportal, located in
| x86 | C:\Program Files \Team MediaPortal\MediaPortal\plugins\Windows |
|:----|:---------------------------------------------------------------|
| x64 | C:\Program Files (x86)\Team MediaPortal\MediaPortal\plugins\Windows |

> ## TVSeriesDatabase4.db3 ##
This is the database file, where your MP-TVSeries data is stored. its recommended to backup this file incase you experience database corruption.
| XP |  C:\Documents and Settings\All Users\Application Data\Team MediaPortal\MediaPortal\database |
|:---|:--------------------------------------------------------------------------------------------|
| Vista, Win7 & Win10 |  C:\ProgramData\Team MediaPortal\MediaPortal\database                      |

> ## MPTVSeriesBanners and Fan Art (Folders) ##
These two folders inside the thumbs folder are where the artwork is stored.
| XP |  C:\Documents and Settings\All Users\Application Data\Team MediaPortal\MediaPortal\thumbs |
|:---|:------------------------------------------------------------------------------------------|
| Vista, Win7 & Win10 |  C:\ProgramData\Team MediaPortal\MediaPortal\thumbs                      |

> ## MP-TVSeries.log ##
This file contains a log of activities in the MP-TVSeries plugin, which is useful for diagnostic purposes
| XP |  C:\Documents and Settings\All Users\Application Data\Team MediaPortal\MediaPortal\log |
|:---|:---------------------------------------------------------------------------------------|
| Vista, Win7 & Win10 |  C:\ProgramData\Team MediaPortal\MediaPortal\log                      |

> ## Language.XML (Files) ##
This directory contains all the language files, in XML format that can be used in MP-TVSeries
| XP |  C:\Documents and Settings\All Users\Application Data\Team MediaPortal\Mediaportal\language\MP-TVSeries\ |
|:---|:---------------------------------------------------------------------------------------------------------|
| Vista, Win7 & Win10 |  C:\ProgramData\Team MediaPortal\Mediaportal\language\MP-TVSeries\                      |

> ## Series Cache (Files) ##
From MP-TVSeries v2.9+, there will be caching of the files downloaded from [theTVDB.com](http://thetvdb.com) to speed up your import time. Cached series are only removed if the online API reports newer information is available.

| XP |  C:\Documents and Settings\All Users\Application Data\Team MediaPortal\Mediaportal\MP-TVSeries\Cache\ |
|:---|:------------------------------------------------------------------------------------------------------|
| Vista, Win7 & Win10 |  C:\ProgramData\Team MediaPortal\Mediaportal\MP-TVSeries\Cache\                      |

# MP-TVSeries Registry Location #

If you have modified the default path to the DB, that is stored and can be changed in the registry using the following key

| HKEY\_CURRENT\_USER\Software\MPTVSeries\DBFile |
|:-----------------------------------------------|

__Note:__ if you are using the default path, this will be empty.
---

# Backup and Migration #

If you want to backup your MP-TVSeries data, or migrate it to a new computer you need to backup/restore the following.

  * Backup [TVSeriesDatabase4.db3](#TVSeriesDatabase4.db3)
  * Backup [MPTVSeriesBanners and Fan Art (Folders)](#MPTVSeriesBanners and Fan Art (Folders)). 

__Note:__ you could just copy the entire 'thumbs' folder (recommend), especially if you have ClearART etc are doing the same process with MovingPictures.
---

# Multiple HTPCs and MP-TVSeries #

Mediaportal 1 does not have native support for sharing its databases across multiple clients, this is something scheduled for Mediaportal 2. There are options, the first, is the [multi-seat](http://wiki.team-mediaportal.com/UserGuides/CentralisedDatabases) Approach (not recommended/supported). The Second option, is below

> ## Main HTPC ##

  * Make sure all paths are using UNC paths, no local paths, mapped drives are not recommended
  * Configure your first HTPC with all the options/settings you prefer
  * Check your main HTPC is synced with trakt.tv
  * Run a Full Import
  * Close Mediaportal

> ## Additional HTPCs ##

Its recommeded to have the same version of the MP-TVSeries plugin as your Main HTPC

  * Close Mediaporal
  * From the main HTPC, copy its [TVSeriesDatabase4.db3](#TVSeriesDatabase4.db3) to your additional HTPCs, overwriting.
  * From the main HTPC, copy its [MPTVSeriesBanners and Fan Art (Folders)](#MPTVSeriesBanners_and_Fan_Art_(Folders)) to your additional HTPCs, overwriting. Note you could just copy the entire thumbs folder (recommend), especially if you are doing the same process with MovingPictures etc.
  * Open Mediaportal and you should have a working MP-TVSeries. When a show is watched on 1 HTPC it will be replicated to the others when it next syncs with Trakt.tv

> ### Advantages ###
  * You can use all HTPCs at the same time
  * No greater risk of Database Corruption

> ### Disadvantages ###
  * Every new episode added after you copied the database/thumbs will be imported on every additional HTPC, so this will use additional internet traffic
  * Changes you make on one HTPC, e.g. disabling fanart, changing views, adding/removing favourites are not replicated to the other HTPCs. (Until you copy the database again)
---

# Language Support #
MP-TVSeries supports multiple languages, and you are welcome to contribute by updating or adding a new language. There are also an option to select different language for different series.

> ## Configuration for one language ##
  * Launch the MP-TVSeries Plugin Configuration
  * Click __General__ Tab
  * Drop down the Language box, choose your _language_
  * Close MediaPortal

> ## Configuraiton for different languages for different series ##
  * Lunch the MP-TVSeries Plugin Configuration
  * Click __General__ tab
  * Enable the __Override Language for Series__ checkbox
  * Click __Details__ tab
  * Select a Series and change the Laguage combobox to you're prefered _language_
  * Right click on the Series name and choose _update_

> ## Update Language ##
If you find a language has incorrect spelling, or missing some translation fields, you can update the appropriate .xml file in the [Language Folder](#Language.XML_(Files)). Please submit the updated file on the [forum](http://forum.team-mediaportal.com/my-tvseries-162/)

> ## Add New Language ##
  * Download the latest English XML [here](https://github.com/damienhaynes/mptvseries/tree/master/MP-TVSeries/Resources/Languages/en(us).xml)
  * Rename en(us).xml to yourlanguage.xml
  * Update All Fields Possible
  * Submit the new xml to the [forum](http://forum.team-mediaportal.com/my-tvseries-162/) to be included in an upcoming build
---

# Edit Database #

If you need to edit the MP-TVSeries database directly,you can download a SQLite tool, such as [sqlitebrowser](http://sqlitebrowser.org) and open the [TVSeriesDatabase4.db3](#TVSeriesDatabase4.db3) file.
---

# Database Corruption #
Its sometimes reported that the database file TVSeriesDatabase4.db3 has become damaged in some fashion. The MP-TVSeries plugin in Mediaportal may not function correctly, such as your Series/Episodes are missing etc.

> ## Identify Corruption ##
The MP-TVSeries.log file will usually contain multiple events like the following:
  * Commit failed on this command:
  * SQLiteClient:  cmd:sqlite3\_finalize err:CORRUPT detailed:database disk image is malformed query

> ## Causes ##
  * If you are using Mediaportal/MP-TVSeries in (unsupported and not recommended) [multi-seat mode](http://wiki.team-mediaportal.com/UserGuides/CentralisedDatabases), this will likely lead to database corruption if multiple clients write to the database at the same time
  * Hard Shutdown of PC while MP-TVSeries is preforming a database operation, (i.e Power Loss)
  * PC Crash / Lockup / BSOD
  * Your just unlucky!

> ## Solution ##
There are 2 options, restore from a backup file, start a new database.

__Note:__ Artwork that has already been downloaded will be automatically re-used and not re-downloaded.

> ### Option 1 - Restore from Backup File ###
If you installed MP-TVSeries from the installer, it would have made a backup database in the below directory location. You could take the backup file from this location and overwrite the corrupt [TVSeriesDatabase4.db3](#TVSeriesDatabase4.db3). If the backups are extremely old, it may be wiser to use Option 2

| XP | C:\Documents and Settings\All Users\Application Data\Team MediaPortal\MediaPortal\database\TVSeries\_Backup\ |
|:---|:-------------------------------------------------------------------------------------------------------------|
| Vista, Win7 & Win10 | C:\ProgramData\Team MediaPortal\MediaPortal\database\TVSeries\_Backup\                                       |

> ### Option 2 - New Database ###

  * (Optional) To retain your watch flags, and your NOT currently synced with [trakt.tv](#Trakt.tv), _Export Watched Flags.._ from the MP-TVSeries plugin configuration, General Tab
  <img src="https://cloud.githubusercontent.com/assets/16772925/20230428/37fbc286-a85c-11e6-842b-1a4047fefc99.jpg" with="150" height="100"> 
  * Delete your database file [TVSeriesDatabase4.db3](#TVSeriesDatabase4.db3)
  * TVSeriesDatabase4.db3 will be re-created next time mediportal or the MP-TVSeries plugin configuration executes
  * Reimport your shows
  * (Optional) _Import Watched Flags_ after import is complete, or resetup your [trakt.tv](#Trakt.tv) account.
---

# Tips And Tricks #

> ## Delete From Disk, Database, Disk & Database ##
This lets you delete files or just entries from the database so the can be rescanned
 * Enable _Allow user to delete files from context menu_ in the general tab, in MP-TVSeries Plugin Configuration
 * Select a Series Season or Episode, press 0 on remote or keyboard and select the option


> ## Show Date In Regional Format ##
MP-TVSeries by default shows episode dates in YYYY-MM-DD format. To change this to use your regional settings
 * Enable _Use regional settings to display dates_ in the general tab, in MP-TVSeries Plugin Configuration
 * You can further tweak this by changing the Short Date syntax settings in Control Panel, Regional settings.


> ## Use "String Replacements" To Resolve Weird filenames ##
You might have made a typo in the filenames, are using cryptic/acronym based filenames or using a different language then what [thetvdb.com](http://thetvdb.com) uses to identify the TV Show. Instead of renaming a lot of files, you can simply use the _String Replacements_ system to fix these problems. You can find this in the _Import_ tab in MP-TVSeries plugin configuration.


> ## Fan Art - View Disable Download ##
  * Select a Series/Season/Episode
  * Press Info on Remote or F9 on keyboard and Select FanArt
  * Click on a Fanart tagged Local to toggle between Default (only used if Random FanArt is off)
  * Click on a Fanart tagged Remote to Download that Fanart
  * Press F9 on Keyboard, INFO on remote to Delete a FanArt

> ## Modify 'Recently Added' 'Latest Aired' Views To Exclude Watched Episodes ##
To change these views so it only shows episodes currently unwatched, you can [edit](#Edit_Database) the database, and modify the query to below (Views table)

  * Latest Aired Unwatched
```
  episode<;><Episode.FirstAired>;<=;<today><cond><Episode.FirstAired>;>=;<today-30><cond><Episode.Watched>;=;0<;><Episode.FirstAired>;desc<;>
```

  * Recently Added UnWatched
```
  episode<;><Episode.FileDateCreated>;>=;<today-7><cond><Episode.Watched>;=;0<;><Episode.FileDateCreated>;desc<;>
```


> ## Speed Up Import - Ignore Old Series ##
If you have a large collection and notice it takes a while for the import process to run, you could tell MP-TVSeries to ignore some of your collection. You should only do this on old shows that are well and truley finished and are unlikely to be edited/changed at the [thetvdb.com](http://thetvdb.com)
  * Load MP-TVSeries plugin Configuration
  * Click on the _Details_ Tab
  * Right Click an Old Series
  * Select _Ignore on Scan_


> ## Speed Up Import - Upgrade to MP-TVSeries 2.9+ ##
MP-TVSeries 2.9+ has significantly faster import times compared to MP-TVSeries 2.8 and below due to a new [Caching](#Series_Cache_%28Files%29) feature. If you have a large collection, this is highly recommended.


> ## Speed Up Series Browsing - Change Layout ##
In Mediaportal, if you find browsing the Series list is slow or not smooth when scrolling, you may have older HTPC hardware for example, try switching the Series layouts to _LIST POSTERS_ or _LIST BANNERS_
  * In Mediportal, go into MP-TVSeries and browse your entire Series.
  * Press F9 or INFO on MCE Remote
  * Choose _CHANGE LAYOUT_
  * Select _LIST POSTERS_ or _LIST BANNERS_


> ## Fast Cycling of Artwork via Keyboard or Remote ##
There is a way to bind a key in Mediaportal for the purpose of having a quick way to change the artwork without having to do into the menu. This need to be linked with the either or both MP actions.

  * ACTION\_PREV\_PICTURE
  * ACTION\_NEXT\_PICTURE


> ### To map a keyboard button to these actions: ###

  * Load Mediaportal Configuration
  * Click General,  Keys and Sounds
  * Select the _WINDOWS_ node
  * Click _ADD_ button
  * In Description field type _MP-TVSeries specific_ or similar
  * In Action field, enter _9811_ (the window id of MP-TVSeries)
  * Click _ADD_

> ### This will allow you to map the actions ###

  * Click  _<New Action>_ node
  * Enter in description e.g. _Cycle Artwork Next_
  * In actions, choose _ACTION\_NEXT\_PICTURE_
  * Click on Key: and press keyboard character
  * Click OK
  * Repeat these steps if you want to bind ACTION\_PREV\_PICTURE

Load Mediaportal, TVSeries plugin, and press the KEY you set to change the artwork


> ## Playlist Keyboard Shortcuts ##
The following are shortcuts avaialble for playlists in MP-TVSeries
  * Play Next = F8
  * Play Prev = F7
  * Add to Playlist = Y
  * Show Playlist = F1

__Note:__ for Add to Playlist and Show Playlist shorcuts, you need to add them to _Keys and Sounds_ in the Mediaportal Configuration. See screenshow below. Note: Window ID for MP-TVSeries is 9811
---
