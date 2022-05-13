USE [Linkia]
GO

/****** Object:  Table [dbo].[ProdInvIssueDetComment]    Script Date: 28-Mar-22 6:04:55 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProdInvIssueDetComment](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ProdInvIssueDetId] [uniqueidentifier] NOT NULL,
	[Comments] [nvarchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProdInvIssueDetComment] ADD  CONSTRAINT [DF_ProdInvIssueDetComment_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[ProdInvIssueDetComment] ADD  CONSTRAINT [DF_ProdInvIssueDetComment_Active]  DEFAULT (N'Y') FOR [Active]
GO


