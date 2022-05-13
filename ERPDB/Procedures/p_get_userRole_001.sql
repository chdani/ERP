USE [Linkia]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'p_get_userRole_001')
	DROP PROCEDURE p_get_userRole_001
Go

CREATE PROCEDURE [dbo].[p_get_userRole_001](
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
	WHERE 
		(@RoleName IS NULL OR RoleName = @RoleName)
		AND (@UserRoleId IS NULL OR Id = @UserRoleId)
		AND (@RoleCode IS NULL OR RoleCode = @RoleCode)
		AND (@Active IS NULL OR Active = @Active)
END;


GO


