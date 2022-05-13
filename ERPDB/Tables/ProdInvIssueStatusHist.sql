USE [Linkia]
GO

/****** Object:  Table [dbo].[ProdInvIssueStatusHist]    Script Date: 30-Mar-22 9:48:54 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProdInvIssueStatusHist](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ProdInvIssueId] [uniqueidentifier] NOT NULL,
	[Status] [varchar](20) NOT NULL,
	[Comments] [nvarchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProdInvIssueStatusHist] ADD  CONSTRAINT [DF_ProdInvIssueStatusHist_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[ProdInvIssueStatusHist] ADD  CONSTRAINT [DF_ProdInvIssueStatusHist_Active]  DEFAULT (N'Y') FOR [Active]
GO


