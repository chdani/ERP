USE [Linkia]
GO

/****** Object:  Table [dbo].[ProdCategory]    Script Date: 08-Mar-22 12:23:16 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProdSubCategory](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ProdCategoryId] [uniqueidentifier]  NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].ProdSubCategory ADD  CONSTRAINT [DF_ProdSubCategory_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].ProdSubCategory ADD  CONSTRAINT [DF_ProdSubCategory_Active]  DEFAULT (N'Y') FOR [Active]
GO


