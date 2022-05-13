USE [Linkia]
GO

/****** Object:  Table [dbo].[DirInvPrePayHist]    Script Date: 18-Feb-22 12:38:49 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DirInvPrePayHist](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[DirectInvPrePaymentId] [uniqueidentifier] NOT NULL,
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

ALTER TABLE [dbo].[DirInvPrePayHist] ADD  CONSTRAINT [DF_DirInvPrePayHist_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[DirInvPrePayHist] ADD  CONSTRAINT [DF_DirInvPrePayHist_Active]  DEFAULT (N'Y') FOR [Active]
GO

