USE [Linkia]
GO

/****** Object:  Table [dbo].[EmpDepartmentHist]    Script Date: 18-Jan-22 6:17:16 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmpDepartmentHist](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[EmployeeId] [uniqueidentifier] NOT NULL,
	[DepartmentId] [uniqueidentifier] NOT NULL,
	[PeriodFrom] [date] NOT NULL,
	[PeriodTo] [date] NULL,
	[IsCurrent] [bit] NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EmpDepartmentHist] ADD  CONSTRAINT [DF_EmpDepartmentHist_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[EmpDepartmentHist] ADD  CONSTRAINT [DF_EmpDepartmentHist_Active]  DEFAULT (N'Y') FOR [Active]
GO


