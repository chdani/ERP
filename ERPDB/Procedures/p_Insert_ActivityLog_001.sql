USE [Linkia]
GO

/****** Object:  StoredProcedure [dbo].[p_Insert_ActivityLog_001]    Script Date: 11/21/2021 8:26:09 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[p_Insert_ActivityLog_001]
@UserId uniqueidentifier = null,
@Host NVARCHAR(200) = null,
@Headers NVARCHAR(MAX) = null,
@RequestBody NVARCHAR(MAX) = null,
@RequestMethod NVARCHAR(100) = null,
@UserHostAddress NVARCHAR(200) = null,
@UserAgent NVARCHAR(200) = null,
@AbsoluteURI NVARCHAR(200) = null,
@RequestedOn DATETIME
AS
BEGIN
	INSERT INTO [ActivityLog] VALUES 
	(@UserId, @Host, @Headers, @RequestBody, @RequestMethod, @UserHostAddress, @UserAgent, @AbsoluteURI, @RequestedOn)
END
GO


