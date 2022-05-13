USE [Linkia]
GO

/****** Object:  Table [dbo].[DirInvPrePayStatusHist]    Script Date: 18-Feb-22 12:39:04 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DirInvPrePayStatusHist](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[DirectInvPrePaymentId] [uniqueidentifier] NOT NULL,
	[Status] [varchar](20) NOT NULL,
	[Comments] [varchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DirInvPrePayStatusHist] ADD  CONSTRAINT [DF_DirInvPrePayStatusHist_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[DirInvPrePayStatusHist] ADD  CONSTRAINT [DF_DirInvPrePayStatusHist_Active]  DEFAULT (N'Y') FOR [Active]
GO


