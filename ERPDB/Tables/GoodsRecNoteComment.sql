USE [Linkia]
GO

/****** Object:  Table [dbo].[GoodsRecNoteComment]    Script Date: 22-Mar-22 2:19:08 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[GoodsRecNoteComment](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[GoodsReceiptNoteId] [uniqueidentifier] NOT NULL,
	[Comments] [nvarchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[GoodsRecNoteComment] ADD  CONSTRAINT [DF_GoodsRecNoteComment_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[GoodsRecNoteComment] ADD  CONSTRAINT [DF_GoodsRecNoteComment_Active]  DEFAULT (N'Y') FOR [Active]
GO


