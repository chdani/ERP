USE [Linkia]
GO

/****** Object:  Table [dbo].[UserOrganization]    Script Date: 06-Feb-22 10:43:21 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserOrganization](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[OrganizationId] [uniqueidentifier] NOT NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserOrganization] ADD  CONSTRAINT [DF_UserOrganization_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[UserOrganization] ADD  CONSTRAINT [DF_UserOrganization_Active]  DEFAULT (N'Y') FOR [Active]
GO


