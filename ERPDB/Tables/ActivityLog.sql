USE [Linkia]
GO

/****** Object:  Table [dbo].[ActivityLog]    Script Date: 11/21/2021 8:10:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ActivityLog](
	[UserId] [uniqueidentifier] NULL,
	[Host] [nvarchar](100) NULL,
	[Headers] [nvarchar](max) NULL,
	[RequestBody] [nvarchar](max) NULL,
	[RequestMethod] [nvarchar](50) NULL,
	[UserHostAddress] [nvarchar](100) NULL,
	[UserAgent] [nvarchar](1000) NULL,
	[AbsoluteURI] [nvarchar](500) NULL,
	[RequestedOn] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


