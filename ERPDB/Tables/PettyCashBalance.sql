USE [Linkia]
GO

/****** Object:  Table [dbo].[PettyCashBalance]    Script Date: 01-Feb-22 3:40:22 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PettyCashBalance](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[TellerId] [uniqueidentifier] NOT NULL,
	[AccountId] [uniqueidentifier] NOT NULL,
	[OrgId] [uniqueidentifier] NOT NULL,
	[BalanceDate] [date] NOT NULL,
	[OpeningBalance] [numeric](20, 6) NOT NULL,
	[FinYear] [varchar](20) NOT NULL,
	[Credit] [numeric](20, 6) NOT NULL,
	[Debit] [numeric](20, 6) NOT NULL,
	[ClosingBalance] [numeric](20, 6) NOT NULL,
	[DayClosed] [bit] NOT NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PettyCashBalance] ADD  CONSTRAINT [DF_PettyCashBalance_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[PettyCashBalance] ADD  CONSTRAINT [DF_PettyCashBalance_DayClosed]  DEFAULT ((0)) FOR [DayClosed]
GO

ALTER TABLE [dbo].[PettyCashBalance] ADD  CONSTRAINT [DF_PettyCashBalance_Active]  DEFAULT (N'Y') FOR [Active]
GO


