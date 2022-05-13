CREATE TABLE [dbo].[EmpEducation](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[EmployeeId] [uniqueidentifier] NOT NULL,
	[EduLevelCode] [varchar](20) NOT NULL,
	[EstablishmentCode] [nvarchar](20) NOT NULL,
	[Specialization] [nvarchar](20) NOT NULL,
	[CompletedYear] [varchar](5) NULL,
	[GradePercentage] [nvarchar](5) NULL,
	[Remarks] [nvarchar](1000) NOT NULL,
	[Active] [varchar](1) NOT NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedDate] [smalldatetime] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [smalldatetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EmpEducation] ADD  CONSTRAINT [DF_EmpEducation_id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[EmpEducation] ADD  CONSTRAINT [DF_EmpEducation_Active]  DEFAULT (N'Y') FOR [Active]
GO
