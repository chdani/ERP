USE [Linkia]
GO

/****** Object:  Table [dbo].[ProdInvIssueComment]    Script Date: 28-Mar-22 6:04:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProdInvIssueComment](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ProdInvIssueId] [uniqueidentifier] NOT NULL,
	[Comments] [nvarchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProdInvIssueComment] ADD  CONSTRAINT [DF_ProdInvIssueComment_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[ProdInvIssueComment] ADD  CONSTRAINT [DF_ProdInvIssueComment_Active]  DEFAULT (N'Y') FOR [Active]
GO


