USE [Linkia]
GO

/****** Object:  Table [dbo].[AppMenuMaster]    Script Date: 01-Mar-22 10:24:03 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AppMenuMaster](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ModuleCode] [varchar](20) NOT NULL,
	[ModuleName] [nvarchar](50) NOT NULL,
	[ModuleDispOrder] [int] NOT NULL,
	[AppAccessId] [uniqueidentifier] NOT NULL,
	[MainMenuName] [nvarchar](50) NOT NULL,
	[SubMenuName] [nvarchar](50) NOT NULL,
	[DispOrder] [int] NOT NULL,
	[MainMenuCode] [varchar](20) NOT NULL,
	[SubMenuCode] [varchar](20) NOT NULL,
	[MainMenuIcon] [varchar](25) NULL,
	[SubMenuIcon] [varchar](25) NULL,
	[ModuleIcon] [varchar](25) NULL,
	[MainMenuDispOrd] [int] NOT NULL,
	[ShowFinYear] [bit] NOT NULL,
	[ShowOrg] [bit] NOT NULL,
	[Active] [nvarchar](1) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AppMenuMaster] ADD  CONSTRAINT [DF_AppMenuMaster_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[AppMenuMaster] ADD  CONSTRAINT [DF_AppMenuMaster_ShowFinYear]  DEFAULT ((0)) FOR [ShowFinYear]
GO

ALTER TABLE [dbo].[AppMenuMaster] ADD  CONSTRAINT [DF_AppMenuMaster_ShowOrg]  DEFAULT ((0)) FOR [ShowOrg]
GO

ALTER TABLE [dbo].[AppMenuMaster] ADD  CONSTRAINT [DF_AppMenuMaster_Active]  DEFAULT (N'Y') FOR [Active]
GO


