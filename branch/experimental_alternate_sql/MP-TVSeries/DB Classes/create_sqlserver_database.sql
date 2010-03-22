CREATE TABLE [dbo].[Fanart](
	[id] int NOT NULL,
	[seriesID] int NULL,
	[Chosen] varchar(100) NULL,
	[LocalPath] varchar(1024) NULL,
	[BannerPath] varchar(1024) NULL,
	[ThumbnailPath] varchar(1024) NULL,
	[Colors] varchar(100) NULL,
	[Disabled] varchar(10) NULL,
	[SeriesName] varchar(20) NULL,
	[BannerType] varchar(40) NULL,
	[BannerType2] varchar(40) NULL,
	[Language] varchar(10) NULL,
	[VignettePath] varchar(100) NULL,
	[Season] varchar(40) NULL,	
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)
)
GO

CREATE TABLE [dbo].[ImportPathes](
	[ID] int NOT NULL,
	[enabled] int NULL,
	[Path] varchar(1024) NULL,
	[removable] int NULL,
	[keep_references] int NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)
)
GO

CREATE TABLE [dbo].[local_episodes](
	[EpisodeFilename] varchar(1024) NOT NULL,--MySqlReplace
	[CompositeID] varchar(20) NULL,
	[SeriesID] int NULL,
	[SeasonIndex] int NULL,
	[EpisodeIndex] int NULL,
	[LocalEpisodeName] varchar(1024) NULL,
	[LocalImportProcessed] int NULL,
	[AvailableSubtitles] varchar(1024) NULL,
	[CompositeID2] varchar(20) NULL,
	[EpisodeIndex2] int NULL,
	[videoWidth] int NULL,
	[videoHeight] int NULL,
	[FileDateAdded] varchar(20) NULL,
	[FileDateCreated] varchar(20) NULL,
	[IsAvailable] int NULL,
	[ext] varchar(20) NULL,
	[Removable] char(2) NULL,
	[localPlaytime] varchar(20) NULL,
	[VideoCodec] varchar(20) NULL,
	[VideoBitrate] varchar(20) NULL,
	[VideoFrameRate] varchar(20) NULL,
	[VideoAspectRatio] varchar(20) NULL,
	[AudioCodec] varchar(20) NULL,
	[AudioBitrate] varchar(20) NULL,
	[AudioChannels] varchar(20) NULL,
	[AudioTracks] varchar(40) NULL,
	[TextCount] varchar(40) NULL,
	[StopTime] varchar(20) NULL,
	[tags] varchar(20) NULL,
	[originalFilename] varchar(100) NULL,
PRIMARY KEY CLUSTERED 
(
	[EpisodeFilename] ASC
)
)
GO

CREATE NONCLUSTERED INDEX [epComp1] ON [dbo].[local_episodes] 
(
	[CompositeID] ASC
)
GO

CREATE NONCLUSTERED INDEX [epComp2] ON [dbo].[local_episodes] 
(
	[CompositeID2] ASC
)
GO

CREATE TABLE [dbo].[local_series](
	[Parsed_Name] varchar(200) NOT NULL,
	[ID] int NULL,
	[ScanIgnore] int NULL,
	[DuplicateLocalName] int NULL,
	[Hidden] int NULL,
PRIMARY KEY CLUSTERED 
(
	[Parsed_Name] ASC
)
)
GO

CREATE NONCLUSTERED INDEX [seriesIDLocal] ON [dbo].[local_series] 
(
	[ID] ASC
)
GO

CREATE TABLE [dbo].[torrent](
	[ID] varchar(20) NOT NULL,
	[searchUrl] varchar(1024) NULL,
	[searchRegex] varchar(1024) NULL,
	[detailsUrl] varchar(1024) NULL,
	[detailsRegex] varchar(1024) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)
)
GO

CREATE TABLE [dbo].[Views](
	[ID] int NOT NULL,
	[enabled] int NULL,
	[sort] int NULL,
	[TransToken] varchar(50) NULL,
	[PrettyName] varchar(50) NULL,
	[viewConfig] varchar(1024) NULL,
	[TaggedView] int NULL,
	[ParentalControl] int NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)
)
GO

CREATE TABLE [dbo].[online_episodes](
	[CompositeID] varchar(20) NOT NULL,
	[EpisodeID] int NULL,
	[SeriesID] int NULL,
	[EpisodeIndex] int NULL,
	[SeasonIndex] int NULL,
	[EpisodeName] varchar(200) NULL,
	[Watched] int NULL,
	[Summary] text NULL,
	[FirstAired] varchar(20) NULL,
	[OnlineDataImported] int NULL,
	[GuestStars] text NULL,
	[Director] varchar(200) NULL,
	[Writer] varchar(200) NULL,
	[Hidden] int NULL,
	[lastupdated] varchar(20) NULL,
	[DownloadPending] int NULL,
	[DownloadExpectedName] varchar(200) NULL,
	[ThumbUrl] varchar(200) NULL,
	[thumbFilename] varchar(200) NULL,
	[Combined_episodenumber] varchar(10) NULL,
	[Combined_season] varchar(10) NULL,
	[DVD_chapter] varchar(20) NULL,
	[DVD_discid] varchar(100) NULL,
	[DVD_episodenumber] varchar(10) NULL,
	[DVD_season] varchar(10) NULL,
	[EpImgFlag] varchar(10) NULL,
	[IMDB_ID] varchar(20) NULL,
	[Language] char(4) NULL,
	[ProductionCode] varchar(50) NULL,
	[Rating] varchar(20) NULL,
	[absolute_number] varchar(20) NULL,
	[airsafter_season] varchar(20) NULL,
	[airsbefore_episode] varchar(20) NULL,
	[airsbefore_season] varchar(20) NULL,
	[seasonid] varchar(20) NULL,
PRIMARY KEY CLUSTERED 
(
	[CompositeID] ASC
)
)
GO

CREATE NONCLUSTERED INDEX [seriesIDOnlineEp] ON [dbo].[online_episodes] 
(
	[SeriesID] ASC
)
GO

CREATE TABLE [dbo].[season](
	[ID] varchar(20) NOT NULL,
	[SeriesID] int NULL,
	[SeasonIndex] int NULL,
	[BannerFileNames] text NULL,
	[CurrentBannerFileName] varchar(1024) NULL,
	[HasLocalFiles] int NULL,
	[HasLocalFilesTemp] int NULL,
	[HasOnlineEpisodes] int NULL,
	[HasOnlineEpisodesTemp] int NULL,
	[Hidden] int NULL,
	[ForomSubtitleRoot] varchar(1024) NULL,
	[UnwatchedItems] int NULL,
	[EpisodeCount] int NULL,
	[EpisodesUnWatched] int NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)
)
GO

CREATE TABLE [dbo].[expressions](
	[ID] int NOT NULL,
	[enabled] int NULL,
	[type] varchar(20) NULL,
	[expression] text NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)
)
GO

CREATE TABLE [dbo].[online_series](
	[ID] int NOT NULL,
	[Pretty_Name] varchar(100) NULL,
	[SortName] varchar(100) NULL,
	[origName] varchar(100) NULL,
	[Status] varchar(20) NULL,
	[Genre] varchar(100) NULL,
	[BannerFileNames] text NULL,
	[CurrentBannerFileName] varchar(200) NULL,
	[PosterFileNames] text NULL,
	[PosterBannerFileName] varchar(200) NULL,	
	[Summary] text NULL,
	[OnlineDataImported] int NULL,
	[AirsDay] varchar(20) NULL,
	[AirsTime] varchar(20) NULL,
	[Actors] text NULL,
	[BannersDownloaded] int NULL,
	[HasLocalFiles] int NULL,
	[HasLocalFiles_Temp] int NULL,
	[GetEpisodesTimeStamp] int NULL,
	[UpdateBannersTimeStamp] int NULL,
	[WatchedFileTimeStamp] int NULL,
	[UnwatchedItems] int NULL,
	[EpisodeCount] int NULL,
	[EpisodesUnWatched] int NULL,
	[ViewTags] varchar(100) NULL,
	[language] char(4) NULL,
	[banner] varchar(100) NULL,
	[FirstAired] varchar(50) NULL,
	[IMDB_ID] varchar(20) NULL,
	[zap2it_id] varchar(20) NULL,
	[EpisodeOrders] varchar(50) NULL,
	[choosenOrder] varchar(20) NULL,
	[ContentRating] varchar(20) NULL,
	[Network] varchar(100) NULL,
	[NetworkID] varchar(10) NULL,
	[Rating] varchar(20) NULL,
	[Runtime] varchar(20) NULL,
	[SeriesID] varchar(20) NULL,
	[added] varchar(50) NULL,
	[addedBy] varchar(100) NULL,
	[fanart] varchar(200) NULL,
	[lastupdated] varchar(20) NULL,
	[poster] varchar(200) NULL,
	[Parsed_Name] varchar(200) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)
)
GO

CREATE TABLE [dbo].[options](
	[option_id] int IDENTITY(1,1) NOT NULL,
	[property] varchar(50) NULL,
	[value] text NULL,
PRIMARY KEY CLUSTERED 
(
	[option_id] ASC
)
)
GO

CREATE TABLE [dbo].[replacements](
	[ID] int NOT NULL,
	[enabled] int NULL,
	[tagEnabled] int NULL,
	[before] int NULL,
	[toreplace] varchar(1024) NULL,
	[with] varchar(1024) NULL,
	[isRegex] int NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)
)
GO

CREATE TABLE [dbo].[news](
	[ID] varchar(20) NOT NULL,
	[searchUrl] varchar(1024) NULL,
	[searchRegexMain] varchar(1024) NULL,
	[searchRegexName] varchar(1024) NULL,
	[searchRegexID] varchar(1024) NULL,
	[searchRegexSize] varchar(1024) NULL,
	[searchRegexPost] varchar(1024) NULL,
	[searchRegexReport] varchar(1024) NULL,
	[searchRegexFormat] varchar(1024) NULL,
	[searchRegexLanguage] varchar(1024) NULL,
	[searchRegexGroup] varchar(1024) NULL,
	[searchRegexIsolateArticleName] varchar(1024) NULL,
	[searchRegexParseArticleName] varchar(1024) NULL,
	[login] varchar(100) NULL,
	[password] varchar(100) NULL,
	[cookielist] varchar(1024) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)
)
GO

CREATE TABLE [dbo].[FormattingRules](
	[ID] int NOT NULL,
	[enabled] int NULL,
	[Replace] varchar(1024) NULL,
	[With] varchar(1024) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)
)
GO

CREATE TABLE [dbo].[ignored_downloaded_files](
	[filename] varchar(1024) NOT NULL,--MySqlReplace
PRIMARY KEY CLUSTERED 
(
	[filename] ASC
)
)
GO
