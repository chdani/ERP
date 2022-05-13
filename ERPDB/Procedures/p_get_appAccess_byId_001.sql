USE [Linkia]
GO

/****** Object:  StoredProcedure [dbo].[p_get_appAccess_byId_001]    Script Date: 11-Oct-21 9:31:05 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[p_get_appAccess_byId_001](
	@AppAccessId uniqueIdentifier
) as
BEGIN
	SELECT 
		a.Id, a.AccessCode, a.AccessName, a.AccessType, a.ScreenUrl,  a.Active, a.CreatedBy,
		a.CreatedDate, a.ModifiedBy, a.ModifiedDate
	FROM AppAccess a 
	WHERE Id = @AppAccessId
END;


GO


