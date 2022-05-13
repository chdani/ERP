IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'p_get_userRoleMapById_001')
	DROP PROCEDURE p_get_userRoleMapById_001
Go 

CREATE   PROCEDURE [dbo].[p_get_userRoleMapById_001](
	@Id uniqueIdentifier = null 
) as
BEGIN
	SELECT 
		Urp.Id, urp.UserID, urp.UserRoleId, ur.RoleCode, ur.RoleName,  urp.Active, urp.CreatedBy,
		urp.CreatedDate, urp.ModifiedBy, urp.ModifiedDate,
		um.UserName
	FROM UserRoleMap urp
	inner join UserRole ur on urp.UserRoleId = ur.id
	inner join UserMaster um on urp.UserID = um.Id
	WHERE @Id IS NULL OR urp.id = @Id
END;

GO


