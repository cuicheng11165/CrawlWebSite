USE [test]
GO

/****** Object:  Table [dbo].[WebContents]    Script Date: 2016/12/27 20:37:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WebContents](
	[Host] [nvarchar](1000) NOT NULL,
	[Title] [nvarchar](500) NULL,
	[Status] [int] NULL,
	[Keywords] [nvarchar](500) NULL
) ON [PRIMARY]

GO


