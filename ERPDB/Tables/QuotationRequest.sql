USE [Linkia]
GO

/****** Object:  Table [dbo].[QuotationRequest]    Script Date: 07-Apr-22 2:17:07 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[QuotationRequest](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[TransNo] [bigint] IDENTITY(1,1) NOT NULL,
	[TransDate] [date] NOT NULL,
	[Remarks] [nvarchar](1000) NULL,
	[Status] [varchar](20) NOT NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[QuotationRequest] ADD  CONSTRAINT [DF_QuotationRequest_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[QuotationRequest] ADD  CONSTRAINT [DF_QuotationRequest_Status]  DEFAULT ('PURTRNSTSSUBMITTED') FOR [Status]
GO

ALTER TABLE [dbo].[QuotationRequest] ADD  CONSTRAINT [DF_QuotationRequest_Active]  DEFAULT (N'Y') FOR [Active]
GO


