USE [Linkia]
GO

/****** Object:  Table [dbo].[LedgerAccountGrps]    Script Date: 01-Feb-22 3:38:50 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LedgerAccountGrps](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[AccountCode] [nvarchar](20) NOT NULL,
	[AccountDesc] [nvarchar](100) NOT NULL,
	[LedgerCodeFrom] [int] NOT NULL,
	[LedgerCodeTo] [int] NOT NULL,
	[OrgId] [uniqueidentifier] NOT NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[LedgerAccountGrps] ADD  CONSTRAINT [DF_LedgerAccountGrps_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[LedgerAccountGrps] ADD  CONSTRAINT [DF_LedgerAccountGrps_Active]  DEFAULT (N'Y') FOR [Active]
GO


