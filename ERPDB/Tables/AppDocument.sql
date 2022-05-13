USE [Linkia]
GO

/****** Object:  Table [dbo].[AppDocument]    Script Date: 11-Mar-22 9:36:17 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AppDocument](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[TransactionId] [uniqueidentifier] NOT NULL,
	[TransactionType] [varchar](20) NOT NULL,
	[DocumentType] [varchar](20) NULL,
	[UniqueNumber] [varchar](50) NULL,
	[ExpiryDate] [date] NULL,
	[FileContent] [varbinary](max) NULL,
	[FileName] [nvarchar](500) NOT NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[AppDocument] ADD  CONSTRAINT [DF_AppDocument_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[AppDocument] ADD  CONSTRAINT [DF_AppDocument_Active]  DEFAULT (N'Y') FOR [Active]
GO


