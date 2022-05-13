USE [Linkia]
GO

/****** Object:  Table [dbo].[SystemSettings]    Script Date: 17-Apr-22 10:44:18 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SystemSettings](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ConfigKey] [nvarchar](20) NOT NULL,
	[Description] [nvarchar](200) NULL,
	[ConfigValue] [nvarchar](500) NOT NULL,
	[Type] [varchar](1) NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SystemSettings] ADD  CONSTRAINT [DF_SystemSettings_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[SystemSettings] ADD  CONSTRAINT [DF_SystemSettings_Active]  DEFAULT (N'Y') FOR [Active]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'''S''  - System type which will not be exposed to the user, U - User type, user can change through screens' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SystemSettings', @level2type=N'COLUMN',@level2name=N'Type'
GO


