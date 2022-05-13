IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'p_get_userRole_byId_001')
	DROP PROCEDURE p_get_userRole_byId_001
Go

CREATE PROCEDURE [dbo].[p_get_userRole_byId_001](
	@UserRoleId uniqueIdentifier = null,
	@RoleName nvarchar(50) = null,
	@RoleCode nvarchar(50) = null,
	@Active nvarchar(1) = null
) as
BEGIN
	SELECT 
		U.Id, u.RoleCode, u.RoleName,  u.Active, u.CreatedBy,
		u.CreatedDate, u.ModifiedBy, u.ModifiedDate
	FROM UserRole U 
	WHERE Id = @UserRoleId
END;


GO


