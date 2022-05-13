USE [Linkia]
GO

/****** Object:  Table [dbo].[ServiceReqDetComment]    Script Date: 22-Mar-22 2:22:56 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ServiceReqDetComment](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ServiceRequestDEtId] [uniqueidentifier] NOT NULL,
	[Comments] [nvarchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ServiceReqDetComment] ADD  CONSTRAINT [DF_ServiceReqDetComment_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[ServiceReqDetComment] ADD  CONSTRAINT [DF_ServiceReqDetComment_Active]  DEFAULT (N'Y') FOR [Active]
GO


