USE [Linkia]
GO

/****** Object:  Table [dbo].[CodesMaster]    Script Date: 25-Oct-21 7:40:20 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CodesMaster](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL,
	[CodeType] [varchar](1) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CodesMaster] ADD  CONSTRAINT [DF_CodesMaster_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[CodesMaster] ADD  CONSTRAINT [DF_CodesMaster_Active]  DEFAULT (N'Y') FOR [Active]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'''S''  - System type which will not be exposed to the user, U - User type, user can change through screens' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CodesMaster', @level2type=N'COLUMN',@level2name=N'CodeType'
GO


