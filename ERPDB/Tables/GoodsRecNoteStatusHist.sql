USE [Linkia]
GO

/****** Object:  Table [dbo].[GoodsRecNoteStatusHist]    Script Date: 22-Mar-22 2:19:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[GoodsRecNoteStatusHist](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[GoodsReceiptNoteId] [uniqueidentifier] NOT NULL,
	[Status] [varchar](20) NOT NULL,
	[Comments] [nvarchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[GoodsRecNoteStatusHist] ADD  CONSTRAINT [DF_GoodsRecNoteStatusHist_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[GoodsRecNoteStatusHist] ADD  CONSTRAINT [DF_GoodsRecNoteStatusHist_Active]  DEFAULT (N'Y') FOR [Active]
GO


