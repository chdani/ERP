USE [Linkia]
GO

/****** Object:  Table [dbo].[UserRole]    Script Date: 11-Oct-21 9:33:50 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserRole](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[RoleName] [nvarchar](50) NOT NULL,
	[RoleCode] [nvarchar](30) NOT NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserRole] ADD  CONSTRAINT [DF_UserRole_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[UserRole] ADD  CONSTRAINT [DF_UserRole_Active]  DEFAULT (N'Y') FOR [Active]
GO


