USE [Linkia]
GO

/****** Object:  StoredProcedure [dbo].[p_get_userInfo_001]    Script Date: 11-Oct-21 9:31:42 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[p_get_userInfo_001](
	@UserName  nvarchar(50) = null,
	@UserId uniqueIdentifier = null,
	@FirstName nvarchar(50) = null,
	@LastName nvarchar(50) = null,
	@Active nvarchar(1) = null
) as
BEGIN
	SELECT 
		U.Id, u.UserName, u.Password, u.FirstName, u.LastName, u.EmailId , u.UserType, u.Active, u.CreatedBy,
		u.CreatedOn, u.ModifiedBy, u.ModifiedOn
	FROM UserMaster U 
	WHERE 
		(@UserName IS NULL OR UserName = @UserName)
		AND (@UserId IS NULL OR Id = @UserId)
		AND (@FirstName IS NULL OR FirstName = @FirstName)
		AND (@LastName IS NULL OR LastName = @LastName)
		AND (@Active IS NULL OR Active = @Active)
END;


GO


