USE [Linkia]
GO

/****** Object:  Table [dbo].[ProdInvIssueDet]    Script Date: 28-Mar-22 6:04:48 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProdInvIssueDet](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ProdInvIssueId] [uniqueidentifier] NOT NULL,
	[ProductMasterId] [uniqueidentifier] NOT NULL,
	[WareHouseLocationId] [uniqueidentifier] NOT NULL,
	[UnitMasterId] [uniqueidentifier] NULL,
	[ExpiryDate] [date] NULL,
	[Quantity] [numeric](18, 6) NULL,
	[Remarks] [nvarchar](1000) NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProdInvIssueDet] ADD  CONSTRAINT [DF_ProdInvIssueDet_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[ProdInvIssueDet] ADD  CONSTRAINT [DF_ProdInvIssueDet_Active]  DEFAULT (N'Y') FOR [Active]
GO
 

