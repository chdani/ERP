USE [Linkia]
GO

/****** Object:  Table [dbo].[UserRoleMap]    Script Date: 11-Oct-21 9:27:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserRoleMap](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[UserRoleId] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserRoleMap] ADD  CONSTRAINT [DF_UserRoleMap_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[UserRoleMap] ADD  CONSTRAINT [DF_UserRoleMap_Active]  DEFAULT (N'Y') FOR [Active]
GO


