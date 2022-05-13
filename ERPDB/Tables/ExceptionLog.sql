USE [Linkia]
GO

/****** Object:  Table [dbo].[ExceptionLog]    Script Date: 11/21/2021 8:14:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ExceptionLog](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ExceptionMessage] [varchar](max) NOT NULL,
	[InnerException] [varchar](max) NULL,
	[StackTrace] [varchar](max) NULL,
	[ExceptionOccurredAt] [smalldatetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


