USE [Linkia]
GO

/****** Object:  Table [dbo].[EmbassyMaster]    Script Date: 21-Dec-21 1:32:23 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmbassyMaster](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[CountryCode] [varchar](20) NOT NULL,
	[NameEng] [nvarchar](50) NOT NULL,
	[NameArabic] [nvarchar](50) NOT NULL,
	[Number] [bigint] NOT NULL,
	[Address] [nvarchar](1000) NULL,
	[DefaultCurrency] [varchar](10) NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EmbassyMaster] ADD  CONSTRAINT [DF_EmbassyMaster_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[EmbassyMaster] ADD  CONSTRAINT [DF_EmbassyMaster_Active]  DEFAULT (N'Y') FOR [Active]
GO


