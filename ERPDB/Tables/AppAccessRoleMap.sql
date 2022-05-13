USE [Linkia]
GO

/****** Object:  Table [dbo].[AppAccessRoleMap]    Script Date: 08-Feb-22 2:42:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AppAccessRoleMap](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[AppAccessId] [uniqueidentifier] NOT NULL,
	[UserRoleId] [uniqueidentifier] NOT NULL,
	[AllowAdd] [varchar](1) NOT NULL,
	[AllowEdit] [varchar](1) NOT NULL,
	[AllowDelete] [varchar](1) NOT NULL,
	[AllowApprove] [varchar](1) NOT NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AppAccessRoleMap] ADD  CONSTRAINT [DF_AppAccessRoleMap_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[AppAccessRoleMap] ADD  CONSTRAINT [DF_AppAccessRoleMap_Write]  DEFAULT (N'N') FOR [AllowAdd]
GO

ALTER TABLE [dbo].[AppAccessRoleMap] ADD  CONSTRAINT [DF_AppAccessRoleMap_AllowEdit]  DEFAULT ('N') FOR [AllowEdit]
GO

ALTER TABLE [dbo].[AppAccessRoleMap] ADD  CONSTRAINT [DF_AppAccessRoleMap_Delete]  DEFAULT (N'N') FOR [AllowDelete]
GO

ALTER TABLE [dbo].[AppAccessRoleMap] ADD  CONSTRAINT [DF_AppAccessRoleMap_AllowDelete1]  DEFAULT (N'N') FOR [AllowApprove]
GO

ALTER TABLE [dbo].[AppAccessRoleMap] ADD  CONSTRAINT [DF_AppAccessRoleMap_Active]  DEFAULT (N'Y') FOR [Active]
GO


