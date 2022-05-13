USE [Linkia]
GO

/****** Object:  Table [dbo].[ProdInvIssue]    Script Date: 30-Mar-22 9:48:16 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProdInvIssue](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Type] [varchar](20) NOT NULL,
	[TransDate] [date] NOT NULL,
	[TransNo] [bigint] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [uniqueidentifier] NULL,
	[ServiceRequestId] [uniqueidentifier] NULL,
	[Status] [varchar](20) NOT NULL,
	[Remarks] [nvarchar](4000) NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProdInvIssue] ADD  CONSTRAINT [DF_ProdInvIssue_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[ProdInvIssue] ADD  CONSTRAINT [DF_ProdInvIssue_CompleteServReq]  DEFAULT ((0)) FOR [Status]
GO

ALTER TABLE [dbo].[ProdInvIssue] ADD  CONSTRAINT [DF_ProdInvIssue_Active]  DEFAULT (N'Y') FOR [Active]
GO


