USE [Linkia]
GO

/****** Object:  Table [dbo].[DirInvPostPayComment]    Script Date: 18-Feb-22 12:30:45 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DirInvPostPayComment](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[DirectInvPostPaymentId] [uniqueidentifier] NOT NULL,
	[Comments] [varchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DirInvPostPayComment] ADD  CONSTRAINT [DF_DirInvPostPayComment_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[DirInvPostPayComment] ADD  CONSTRAINT [DF_DirInvPostPayComment_Active]  DEFAULT (N'Y') FOR [Active]
GO


