USE [Linkia]
GO

/****** Object:  Table [dbo].[ServiceRequest]    Script Date: 27-Apr-22 1:09:39 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ServiceRequest](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[EmployeeId] [uniqueidentifier] NOT NULL,
	[TransDate] [date] NOT NULL,
	[TransNo] [bigint] IDENTITY(1,1) NOT NULL,
	[ProdCategoryId] [uniqueidentifier] NOT NULL,
	[ProdSubCategoryId] [uniqueidentifier] NULL,
	[ProductMasterId] [uniqueidentifier] NULL,
	[UnitMasterId] [uniqueidentifier] NULL,
	[Quantity] [numeric](18, 6) NOT NULL,
	[Remarks] [nvarchar](4000) NULL,
	[ProdConfiguration] [nvarchar](4000) NULL,
	[CurApprovalLevel] [int] NOT NULL,
	[NextApprovalLevel] [int] NOT NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL,
	[RequiredQty] [numeric](18, 6) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ServiceRequest] ADD  CONSTRAINT [DF_ServiceRequest_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[ServiceRequest] ADD  CONSTRAINT [DF_ServiceRequest_Active]  DEFAULT (N'Y') FOR [Active]
GO


