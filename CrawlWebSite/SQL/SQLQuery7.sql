USE [test]
GO

/****** Object:  Table [dbo].[WebSite]    Script Date: 2016/12/28 17:49:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WebSite](
	[Host] [nvarchar](1000) NOT NULL,
	[Title] [nvarchar](1000) NULL,
	[Keywords] [nvarchar](1000) NULL,
	[Status] [int] NULL
) ON [PRIMARY]

GO


