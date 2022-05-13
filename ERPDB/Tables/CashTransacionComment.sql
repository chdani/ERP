USE [Linkia]
GO

/****** Object:  Table [dbo].[CashTransacionComment]    Script Date: 06-Mar-22 1:08:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CashTransacionComment](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[CashTransacionId] [uniqueidentifier] NOT NULL,
	[Comments] [nvarchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CashTransacionComment] ADD  CONSTRAINT [DF_CashTransacionComment_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[CashTransacionComment] ADD  CONSTRAINT [DF_CashTransacionComment_Active]  DEFAULT (N'Y') FOR [Active]
GO


