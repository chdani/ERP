IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'p_get_AppAccessMapById_001')
	DROP PROCEDURE p_get_AppAccessMapById_001
Go
CREATE   PROCEDURE [dbo].[p_get_AppAccessMapById_001](
	@Id uniqueIdentifier
) as
BEGIN
	SELECT 
		arp.Id, arp.UserRoleId, arp.AppAccessId, ur.RoleCode, ur.RoleName,  arp.Active, arp.CreatedBy,
		arp.CreatedDate, arp.ModifiedBy, arp.ModifiedDate,
		aa.AccessCode, aa.AccessName, aa.AccessType, aa.ScreenUrl
	FROM AppAccessRoleMap arp
	inner join UserRole ur on arp.UserRoleId = ur.id
	inner join AppAccess aa on arp.AppAccessId = aa.Id
	WHERE arp.id = @Id
END;

GO


