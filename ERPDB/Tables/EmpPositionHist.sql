USE [Linkia]
GO

/****** Object:  Table [dbo].[EmpPositionHist]    Script Date: 18-Jan-22 6:18:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmpPositionHist](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[EmployeeId] [uniqueidentifier] NOT NULL,
	[JobPositionId] [uniqueidentifier] NOT NULL,
	[PeriodFrom] [date] NOT NULL,
	[PeriodTo] [date] NULL,
	[Grade] [int] NULL,
	[IsCurrent] [bit] NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EmpPositionHist] ADD  CONSTRAINT [DF_EmpPositionHist_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[EmpPositionHist] ADD  CONSTRAINT [DF_EmpPositionHist_Active]  DEFAULT (N'Y') FOR [Active]
GO


