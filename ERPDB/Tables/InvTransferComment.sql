USE [Linkia]
GO

/****** Object:  Table [dbo].[InvTransferComment]    Script Date: 22-Mar-22 2:19:51 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[InvTransferComment](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[InventoryTransferId] [uniqueidentifier] NOT NULL,
	[Comments] [nvarchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[InvTransferComment] ADD  CONSTRAINT [DF_InvTransferComment_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[InvTransferComment] ADD  CONSTRAINT [DF_InvTransferComment_Active]  DEFAULT (N'Y') FOR [Active]
GO


