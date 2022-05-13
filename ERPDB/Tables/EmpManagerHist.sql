USE [Linkia]
GO

/****** Object:  Table [dbo].[EmpManagerHist]    Script Date: 14-Mar-22 5:06:19 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmpManagerHist](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[EmployeeId] [uniqueidentifier] NOT NULL,
	[ManagerId] [uniqueidentifier] NOT NULL,
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

ALTER TABLE [dbo].[EmpManagerHist] ADD  CONSTRAINT [DF_EmpManagerHist_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[EmpManagerHist] ADD  CONSTRAINT [DF_EmpManagerHist_Active]  DEFAULT (N'Y') FOR [Active]
GO


