USE [Linkia]
GO

/****** Object:  Table [dbo].[CashTransacionHist]    Script Date: 06-Mar-22 1:08:59 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CashTransacionHist](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[CashTransacionId] [uniqueidentifier] NOT NULL,
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

ALTER TABLE [dbo].[CashTransacionHist] ADD  CONSTRAINT [DF_CashTransacionHist_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[CashTransacionHist] ADD  CONSTRAINT [DF_CashTransacionHist_Active]  DEFAULT (N'Y') FOR [Active]
GO


