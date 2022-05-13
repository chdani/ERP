USE [Linkia]
GO

/****** Object:  Table [dbo].[DirInvPostPayStatusHist]    Script Date: 18-Feb-22 12:31:35 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DirInvPostPayStatusHist](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[DirectInvPostPaymentId] [uniqueidentifier] NOT NULL,
	[Status] [varchar](20) NOT NULL,
	[Comments] [varchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DirInvPostPayStatusHist] ADD  CONSTRAINT [DF_DirInvPostPayStatusHist_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[DirInvPostPayStatusHist] ADD  CONSTRAINT [DF_DirInvPostPayStatusHist_Active]  DEFAULT (N'Y') FOR [Active]
GO


