USE [Linkia]
GO

/****** Object:  Table [dbo].[PettyCashTransferComment]    Script Date: 06-Mar-22 1:09:13 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PettyCashTransferComment](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[PettyCashTransferId] [uniqueidentifier] NOT NULL,
	[Comments] [nvarchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PettyCashTransferComment] ADD  CONSTRAINT [DF_PettyCashTransferComment_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[PettyCashTransferComment] ADD  CONSTRAINT [DF_PettyCashTransferComment_Active]  DEFAULT (N'Y') FOR [Active]
GO


