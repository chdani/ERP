USE [Linkia]
GO

/****** Object:  Table [dbo].[UserSetting]    Script Date: 25-Oct-21 7:40:36 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserSetting](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[UserMasterId] [uniqueidentifier] NOT NULL,
	[ConfigKey] [nvarchar](20) NOT NULL,
	[ConfigValue] [nvarchar](500) NOT NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserSetting] ADD  CONSTRAINT [DF_UserSetting_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[UserSetting] ADD  CONSTRAINT [DF_UserSetting_Active]  DEFAULT (N'Y') FOR [Active]
GO


