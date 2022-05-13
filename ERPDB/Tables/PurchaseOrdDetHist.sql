USE [Linkia]
GO

/****** Object:  Table [dbo].[PurchaseOrdDetHist]    Script Date: 22-Mar-22 2:20:47 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PurchaseOrdDetHist](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[PurchaseOrderDetId] [uniqueidentifier] NOT NULL,
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

ALTER TABLE [dbo].[PurchaseOrdDetHist] ADD  CONSTRAINT [DF_PurchaseOrdDetHist_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[PurchaseOrdDetHist] ADD  CONSTRAINT [DF_PurchaseOrdDetHist_Active]  DEFAULT (N'Y') FOR [Active]
GO


