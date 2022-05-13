USE [Linkia]
GO

/****** Object:  Table [dbo].[ProdInvIssueDetHist]    Script Date: 28-Mar-22 6:05:02 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProdInvIssueDetHist](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ProdInvIssueDetId] [uniqueidentifier] NOT NULL,
	[FieldName] [varchar](50) NOT NULL,
	[PrevValue] [nvarchar](max) NULL,
	[CurrentValue] [nvarchar](max) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProdInvIssueDetHist] ADD  CONSTRAINT [DF_ProdInvIssueDetHist_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[ProdInvIssueDetHist] ADD  CONSTRAINT [DF_ProdInvIssueDetHist_Active]  DEFAULT (N'Y') FOR [Active]
GO


