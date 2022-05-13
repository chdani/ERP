USE [Linkia]
GO

/****** Object:  Table [dbo].[EmpDependent]    Script Date: 27-Jan-22 7:58:08 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmpDependent](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[EmployeeId] [uniqueidentifier] NOT NULL,
	[FullName] [nvarchar](100) NOT NULL,
	[QatariID] [nvarchar](20) NOT NULL,
	[Passport] [nvarchar](20) NULL,
	[DOB] [date] NULL,
	[PlaceOfBirth] [nvarchar](100) NOT NULL,
	[RelationCode] [varchar](20) NOT NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EmpDependent] ADD  CONSTRAINT [DF_EmpDependent_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[EmpDependent] ADD  CONSTRAINT [DF_EmpDependent_Active]  DEFAULT (N'Y') FOR [Active]
GO


