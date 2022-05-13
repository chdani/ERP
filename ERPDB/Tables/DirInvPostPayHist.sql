USE [Linkia]
GO

/****** Object:  Table [dbo].[DirInvPostPayHist]    Script Date: 18-Feb-22 12:31:14 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DirInvPostPayHist](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[DirectInvPostPaymentId] [uniqueidentifier] NOT NULL,
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

ALTER TABLE [dbo].[DirInvPostPayHist] ADD  CONSTRAINT [DF_DirInvPostPayHist_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[DirInvPostPayHist] ADD  CONSTRAINT [DF_DirInvPostPayHist_Active]  DEFAULT (N'Y') FOR [Active]
GO


 