USE [test]
GO

/****** Object:  Table [dbo].[WebContent]    Script Date: 2016/12/28 17:49:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WebContent](
	[Url] [nvarchar](1000) NOT NULL,
	[Title] [nvarchar](1000) NULL,
	[Keywords] [nvarchar](1000) NULL
) ON [PRIMARY]

GO


