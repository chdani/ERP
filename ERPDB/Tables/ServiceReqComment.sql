USE [Linkia]
GO

/****** Object:  Table [dbo].[ServiceReqComment]    Script Date: 22-Mar-22 2:22:49 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ServiceReqComment](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ServiceRequestId] [uniqueidentifier] NOT NULL,
	[Comments] [nvarchar](1000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ServiceReqComment] ADD  CONSTRAINT [DF_ServiceReqComment_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[ServiceReqComment] ADD  CONSTRAINT [DF_ServiceReqComment_Active]  DEFAULT (N'Y') FOR [Active]
GO


