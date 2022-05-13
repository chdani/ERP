USE [Linkia]
GO

/****** Object:  StoredProcedure [dbo].[p_get_userInfo_byId_001]    Script Date: 11-Oct-21 9:31:48 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[p_get_userInfo_byId_001](
	@UserId uniqueIdentifier
) as
BEGIN
	SELECT 
		U.Id, u.UserName, u.Password, u.FirstName, u.LastName, u.EmailId , u.UserType, u.Active, u.CreatedBy,
		u.CreatedOn, u.ModifiedBy, u.ModifiedOn
	FROM UserMaster U 
	WHERE 
		 Id = @UserId
END;


GO


