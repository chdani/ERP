USE [Linkia]
GO

/****** Object:  Table [dbo].[Employee]    Script Date: 17-Mar-22 7:25:19 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Employee](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[EmpNumber] [varchar](20) NOT NULL,
	[FullNameEng] [nvarchar](100) NOT NULL,
	[FullNameArb] [nvarchar](100) NOT NULL,
	[QatariID] [nvarchar](20) NOT NULL,
	[Passport] [nvarchar](20) NULL,
	[DOB] [date] NULL,
	[PlaceOfBirth] [nvarchar](100) NOT NULL,
	[Nationality] [nvarchar](100) NOT NULL,
	[Address] [nvarchar](1000) NULL,
	[PhoneNumber] [varchar](50) NULL,
	[Email] [varchar](100) NULL,
	[MaritalStatusCode] [varchar](20) NULL,
	[CurrDepartmentId] [uniqueidentifier] NOT NULL,
	[CurrPositionId] [uniqueidentifier] NOT NULL,
	[ManagerId] [uniqueidentifier] NULL,
	[CurrentGrade] [int] NOT NULL,
	[IsDepratmentHead] [bit] NOT NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Employee] ADD  CONSTRAINT [DF_Employee_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[Employee] ADD  CONSTRAINT [DF_Employee_IsDepratmentHead]  DEFAULT ((0)) FOR [IsDepratmentHead]
GO

ALTER TABLE [dbo].[Employee] ADD  CONSTRAINT [DF_Employee_Active]  DEFAULT (N'Y') FOR [Active]
GO


