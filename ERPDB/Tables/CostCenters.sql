USE [Linkia]
GO

/****** Object:  Table [dbo].[CostCenters]    Script Date: 21-Nov-21 8:05:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CostCenters](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Code] [varchar](20) NOT NULL,
	[Description] [nvarchar](100) NOT NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CostCenters] ADD  CONSTRAINT [DF_CostCenters_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[CostCenters] ADD  CONSTRAINT [DF_CostCenters_Active]  DEFAULT (N'Y') FOR [Active]
GO


