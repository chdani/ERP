USE [Linkia]
GO

/****** Object:  Table [dbo].[UserMaster]    Script Date: 18-Mar-22 3:34:40 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserMaster](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[UserName] [nvarchar](20) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[EmailId] [nvarchar](50) NOT NULL,
	[EmployeeId] [uniqueidentifier] NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL,
	[Active] [nvarchar](1) NULL,
	[UserType] [nvarchar](1) NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserMaster] ADD  CONSTRAINT [DF_Table_1_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[UserMaster] ADD  CONSTRAINT [DF_UserMaster_Active]  DEFAULT (N'Y') FOR [Active]
GO

ALTER TABLE [dbo].[UserMaster] ADD  CONSTRAINT [DF_UserMaster_UserType]  DEFAULT (N'A') FOR [UserType]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'A - Admin, W - Windows Authentication, P - Password required' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserMaster', @level2type=N'COLUMN',@level2name=N'UserType'
GO


