USE [Linkia]
GO

/****** Object:  StoredProcedure [dbo].[p_get_ActivityLog_001]    Script Date: 11/21/2021 8:20:25 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[p_get_ActivityLog_001]
@LogType VARCHAR(100),
@FromDate DATETIME = null,
@ToDate DATETIME = null,
@PageFrom INT = 0,
@PageTo INT = 0,
@RowCount INT output
AS
BEGIN
DECLARE @Count INT;
DECLARE @From VARCHAR(100) = CAST(@FromDate AS DATE);
DECLARE @To VARCHAR(100) = CAST(@ToDate AS DATE);
SET @To = @To + ' 23:59:59';

DECLARE @Bit BIT = CASE WHEN @FromDate IS NULL AND @ToDate IS NULL THEN 0 ELSE 1 END;
IF @LogType = 'activitylog'
BEGIN
	SET @Count = (SELECT COUNT(*) FROM [ActivityLog] WHERE (@Bit = 0 OR RequestedOn BETWEEN @From AND @To))
	SET @RowCount=@Count
	SELECT UserId, Host, Headers, RequestBody, RequestMethod, UserHostAddress, UserAgent, AbsoluteURI, RequestedOn FROM [dbo].[ActivityLog]
	WHERE (@Bit = 0 OR RequestedOn BETWEEN @From AND @To)
	ORDER BY RequestedOn DESC
	OFFSET @PageFrom ROWS FETCH NEXT @PageTo ROWS ONLY
END
ELSE IF @LogType = 'exceptionlog'
BEGIN
	SET @Count = (SELECT COUNT(*) FROM [ExceptionLog] WHERE (@Bit = 0 OR ExceptionOccurredAt BETWEEN @From AND @To));
	set @RowCount=@Count
	SELECT Id, ExceptionMessage, InnerException, StackTrace, ExceptionOccurredAt FROM [dbo].[ExceptionLog]
	WHERE (@Bit = 0 OR ExceptionOccurredAt BETWEEN @From AND @To)
	ORDER BY ExceptionOccurredAt DESC
	OFFSET @PageFrom ROWS FETCH NEXT @PageTo ROWS ONLY
END
END
GO


