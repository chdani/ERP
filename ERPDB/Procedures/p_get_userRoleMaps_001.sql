IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'p_get_userRoleMaps_001')
	DROP PROCEDURE p_get_userRoleMaps_001
Go 

CREATE   PROCEDURE [dbo].[p_get_userRoleMaps_001](
	@Id uniqueIdentifier = null,
	@UserRoleId uniqueIdentifier = null,
	@UserId uniqueIdentifier = null,
	@Active nvarchar(1) = null
) as
BEGIN
	SELECT 
		Urp.Id, urp.UserID, urp.UserRoleId, ur.RoleCode, ur.RoleName,  urp.Active, urp.CreatedBy,
		urp.CreatedDate, urp.ModifiedBy, urp.ModifiedDate,
		um.UserName
	FROM UserRoleMap urp
	inner join UserRole ur on urp.UserRoleId = ur.id
	inner join UserMaster um on urp.UserID = um.Id
	WHERE 
		(@Id IS NULL OR urp.id = @Id)
		AND (@UserRoleId IS NULL OR urp.UserRoleId = @UserRoleId)
		AND (@UserId IS NULL OR urp.UserID = @UserId)
		AND (@Active IS NULL OR urp.Active = @Active)
END;


GO


