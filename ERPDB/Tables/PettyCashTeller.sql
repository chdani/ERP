USE [Linkia]
GO

/****** Object:  Table [dbo].[PettyCashTeller]    Script Date: 07-Dec-21 12:24:07 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PettyCashTeller](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[TellerCode] [varchar](20) NOT NULL,
	[TellerName] [nvarchar](50) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[IsHeadTeller] [bit] NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO


 