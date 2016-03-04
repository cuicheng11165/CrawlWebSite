USE [WebCrawler]
GO

/****** Object:  Table [dbo].[CacheWeb]    Script Date: 2016/3/4 18:42:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CacheWeb](
	[Url] [nvarchar](500) NULL
) ON [PRIMARY]

CREATE TABLE [dbo].[FailedWeb](
	[PrimaryDomain] [nvarchar](500) NULL,
	[FailedUrl] [nvarchar](500) NULL
) ON [PRIMARY]

CREATE TABLE [dbo].[SuccessfulWeb](
	[PrimaryDomain] [nvarchar](1000) NULL,
	[Url] [nvarchar](1000) NULL,
	[Keyword] [nvarchar](MAX) NULL,
) ON [PRIMARY]


GO


