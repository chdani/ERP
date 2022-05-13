USE [Linkia]
GO

/****** Object:  Table [dbo].[UserLedgerAccnt]    Script Date: 06-Feb-22 10:35:22 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserLedgerAccnt](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[AccountCode] [int] NOT NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserLedgerAccnt] ADD  CONSTRAINT [DF_UserLedgerAccnt_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[UserLedgerAccnt] ADD  CONSTRAINT [DF_UserLedgerAccntActive]  DEFAULT (N'Y') FOR [Active]
GO


