USE [Linkia]
GO

/****** Object:  Table [dbo].[ProdCatDepartment]    Script Date: 08-Mar-22 12:23:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProdCatDepartment](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ProdCategoryId] [uniqueidentifier] NOT NULL,
	[DepartmentId] [uniqueidentifier] NOT NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProdCatDepartment] ADD  CONSTRAINT [DF_ProdCatDepartment_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[ProdCatDepartment] ADD  CONSTRAINT [DF_ProdCatDepartment_Active]  DEFAULT (N'Y') FOR [Active]
GO


