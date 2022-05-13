USE [Linkia]
GO

/****** Object:  Table [dbo].[ServiceReqApproval]    Script Date: 25-Mar-22 4:41:50 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ServiceReqApproval](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ServiceRequestId] [uniqueidentifier] NOT NULL,
	[ApprovalLevel] [int] NOT NULL,
	[Status] [varchar](50) NOT NULL,
	[ApprovedBy] [uniqueidentifier] NULL,
	[ApprovedDate] [smalldatetime] NULL,
	[Remarks] [nvarchar](4000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ServiceReqApproval] ADD  CONSTRAINT [DF_ServiceReqApproval_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[ServiceReqApproval] ADD  CONSTRAINT [DF_ServiceReqApproval_Active]  DEFAULT (N'Y') FOR [Active]
GO


