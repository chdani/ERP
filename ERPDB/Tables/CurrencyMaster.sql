CREATE TABLE [dbo].[CurrencyMaster](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Code] [varchar](10) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[CountryCode] [varchar](10) NULL,
	[CountryName] [nvarchar](100)  NULL,
	[Active] [nvarchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL,
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CurrencyMaster] ADD  CONSTRAINT [DF_CurrencyMaster_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[CurrencyMaster] ADD  CONSTRAINT [DF_CurrencyMaster_Active]  DEFAULT (N'Y') FOR [Active]
GO

