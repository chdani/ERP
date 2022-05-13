USE [Linkia]
GO

/****** Object:  Table [dbo].[InventoryTransfer]    Script Date: 16-Apr-22 12:34:17 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[InventoryTransfer](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[TransNo] [bigint] IDENTITY(1,1) NOT NULL,
	[TransDate] [date] NOT NULL,
	[FromWareHouseLocationId] [uniqueidentifier] NOT NULL,
	[ToWareHouseLocationId] [uniqueidentifier] NOT NULL,
	[Remarks] [nvarchar](1000) NULL,
	[Status] [varchar](20) NOT NULL,
	[ApprovedBy] [uniqueidentifier] NULL,
	[ApprovedDate] [smalldatetime] NULL,
	[ApproverRemarks] [nvarchar](500) NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[InventoryTransfer] ADD  CONSTRAINT [DF_InventoryTransfer_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[InventoryTransfer] ADD  CONSTRAINT [DF_InventoryTransfer_Active]  DEFAULT (N'Y') FOR [Active]
GO


