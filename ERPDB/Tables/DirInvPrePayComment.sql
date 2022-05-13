USE [Linkia]
GO

/****** Object:  Table [dbo].[DirInvPrePayComment]    Script Date: 18-Feb-22 12:38:38 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DirInvPrePayComment](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[DirectInvPrePaymentId] [uniqueidentifier] NOT NULL,
	[Comments] [varchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DirInvPrePayComment] ADD  CONSTRAINT [DF_DirInvPrePayComment_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[DirInvPrePayComment] ADD  CONSTRAINT [DF_DirInvPrePayComment_Active]  DEFAULT (N'Y') FOR [Active]
GO


 