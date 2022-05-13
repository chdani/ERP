USE [Linkia]
GO

/****** Object:  StoredProcedure [dbo].[p_Insert_ExceptionLog_001]    Script Date: 11/21/2021 8:27:17 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[p_Insert_ExceptionLog_001]
@Id uniqueidentifier = null,  
@ExceptionMessage NVARCHAR(MAX) = null, 
@InnerException NVARCHAR(MAX) = null,  
@StackTrace NVARCHAR(MAX) = null, 
@ExceptionOccurredAt DATETIME
AS
BEGIN
INSERT INTO [ExceptionLog] VALUES 
(NEWID(), @ExceptionMessage, @InnerException, @StackTrace, @ExceptionOccurredAt)
END
GO


