USE [Linkia]
GO

/****** Object:  StoredProcedure [dbo].[p_get_appAccess_001]    Script Date: 11-Oct-21 9:31:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[p_get_appAccess_001](
	@AppAccessId uniqueIdentifier = null,
	@AccessName nvarchar(50) = null,
	@AccessCode nvarchar(20) = null,
	@AccessType nvarchar(20) = null,
	@ScreenUrl nvarchar(200) = null,
	@Active nvarchar(1) = null
) as
BEGIN
	SELECT 
		a.Id, a.AccessCode, a.AccessName, a.AccessType, a.ScreenUrl,  a.Active, a.CreatedBy,
		a.CreatedDate, a.ModifiedBy, a.ModifiedDate
	FROM AppAccess a 
	WHERE 
		(@AccessName IS NULL OR AccessName = @AccessName)
		AND (@AppAccessId IS NULL OR Id = @AppAccessId)
		AND (@AccessCode IS NULL OR AccessCode = @AccessCode)
		AND (@AccessType IS NULL OR AccessType = @AccessType)		
		AND (@ScreenUrl IS NULL OR ScreenUrl = @ScreenUrl)
		AND (@Active IS NULL OR Active = @Active)
END;


GO


