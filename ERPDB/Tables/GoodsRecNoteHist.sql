USE [Linkia]
GO

/****** Object:  Table [dbo].[GoodsRecNoteHist]    Script Date: 22-Mar-22 2:19:28 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[GoodsRecNoteHist](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[GoodsReceiptNoteId] [uniqueidentifier] NOT NULL,
	[FieldName] [varchar](50) NOT NULL,
	[PrevValue] [nvarchar](max) NULL,
	[CurrentValue] [nvarchar](max) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[GoodsRecNoteHist] ADD  CONSTRAINT [DF_GoodsRecNoteHist_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[GoodsRecNoteHist] ADD  CONSTRAINT [DF_GoodsRecNoteHist_Active]  DEFAULT (N'Y') FOR [Active]
GO


