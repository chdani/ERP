IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'p_get_appAccessRoleMaps_001')
	DROP PROCEDURE p_get_appAccessRoleMaps_001
Go


CREATE  PROCEDURE [dbo].[p_get_appAccessRoleMaps_001](
	@Id uniqueIdentifier = null,
	@UserRoleId uniqueIdentifier = null,
	@AppAccessId uniqueIdentifier = null,
	@Active nvarchar(1) = null
) as
BEGIN
	SELECT 
		arp.Id, arp.UserRoleId, arp.AppAccessId, ur.RoleCode, ur.RoleName, arp.AllowAdd, arp.AllowDelete, arp.AllowEdit, arp.Active, arp.CreatedBy,
		arp.CreatedDate, arp.ModifiedBy, arp.ModifiedDate,
		aa.AccessCode, aa.AccessName, aa.AccessType, aa.ScreenUrl
	FROM AppAccessRoleMap arp
	inner join UserRole ur on arp.UserRoleId = ur.id
	inner join AppAccess aa on arp.AppAccessId = aa.Id
	WHERE 
		(@Id IS NULL OR arp.id = @Id)
		AND (@UserRoleId IS NULL OR arp.UserRoleId = @UserRoleId)
		AND (@AppAccessId IS NULL OR arp.AppAccessId = @AppAccessId)
		AND (@Active IS NULL OR arp.Active = @Active)
END;

GO


