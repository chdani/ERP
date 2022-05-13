IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'p_get_userbased_access_001')
	DROP PROCEDURE p_get_userbased_access_001
Go 

CREATE   PROCEDURE [dbo].[p_get_userbased_access_001](
	@UserId uniqueIdentifier 
) as
BEGIN
	DECLARE @Lang VARCHAR(20)

	SELECT @Lang = us.ConfigValue From UserSetting us where us.ConfigKey = 'USR_LANG' and us.UserMasterId = @UserId;

	IF @Lang IS NULL
		SET @Lang = 'en'

	IF(not exists(select id from UserMaster where id = @UserId AND UserType = 'A' ))
	BEGIN 
		SELECT distinct
			 ur.RoleCode,
			 ISNULL(lur.Description, ur.RoleName) RoleName,
			 arm.AllowDelete,
			 arm.AllowAdd,
			 arm.AllowEdit,
			 arm.AllowApprove,
			 aa.AccessCode,
			 ISNULL(laa.Description, aa.AccessName) AccessName,
			 aa.AccessType,
			 aa.ScreenUrl,
			 amm.ModuleCode,
			 amm.MainMenuCode,
			 ISNULL(lAMD.Description,amm.ModuleName) ModuleName,
			 ISNULL(lamm.Description,amm.MainMenuName) MainMenuName,
			 ISNULL(lasm.Description,amm.SubMenuName) SubMenuName,
			 amm.SubMenuCode,
			 amm.MainMenuIcon,
			 amm.SubMenuIcon,			 
			 amm.ShowFinYear,
			 amm.ShowOrg,
			 amm.ModuleIcon,
			 amm.DispOrder,
			 amm.MainMenuDispOrd,
			 amm.ModuleDispOrder
		FROM
			UserRoleMap urm
			inner join UserRole ur on urm.UserRoleId = ur.id
			inner join AppAccessRoleMap arm on ur.Id = arm.UserRoleId
			inner join AppAccess aa on arm.AppAccessId = aa.id
			left join AppMenuMaster amm on aa.id = amm.AppAccessId
			left join LangMaster lur on ur.RoleCode = lur.Code and lur.CodeType = 'ROLES' and lur.Language = @Lang and lur.Active = 'Y'
			left join LangMaster lAMM on amm.MainMenuCode = lAMM.Code and lAMM.CodeType = 'MENUS' and lAMM.Language = @Lang and lAMM.Active = 'Y'
			left join LangMaster lASM on amm.SubMenuCode = lASM.Code and lASM.CodeType = 'SUBMENUS' and lASM.Language = @Lang and lASM.Active = 'Y'
			left join LangMaster lAA on aa.AccessCode = lAA.Code and lAA.CodeType = 'ACCESS' and lAA.Language = @Lang and lAA.Active = 'Y'
			left join LangMaster lAMD on amm.ModuleCode = lAMD.Code and lAMD.CodeType = 'MODULES' and lAMD.Language = @Lang and lAMD.Active = 'Y'
		WHERE 
			urm.UserID = @UserId
			AND urm.Active ='Y'
			AND ur.Active ='Y'
			AND arm.Active = 'Y' 
			AND aa.Active =  'Y'
			and ISNULL(amm.Active,'Y') = 'Y'
	END
	ELSE
	BEGIN
		SELECT distinct
			 ur.RoleCode,
			 ISNULL(lur.Description, ur.RoleName) RoleName,
			 arm.AllowDelete,
			 arm.AllowAdd,
			 arm.AllowEdit,
			 arm.AllowApprove,
			 aa.AccessCode,
			 ISNULL(laa.Description, aa.AccessName) AccessName,
			 aa.AccessType,
			 aa.ScreenUrl,
			 amm.ModuleCode,
			 amm.MainMenuCode,
			 ISNULL(lAMD.Description,amm.ModuleName) ModuleName,
			 ISNULL(lamm.Description,amm.MainMenuName) MainMenuName,
			 ISNULL(lasm.Description,amm.SubMenuName) SubMenuName,
			 amm.SubMenuCode,
			 amm.MainMenuIcon,
			 amm.SubMenuIcon,			 
			 amm.ShowFinYear,
			 amm.ShowOrg,
			 amm.ModuleIcon,
			 amm.DispOrder,
			 amm.MainMenuDispOrd,
			 amm.ModuleDispOrder
		FROM
			UserRole ur
			inner join AppAccessRoleMap arm on ur.Id = arm.UserRoleId
			inner join AppAccess aa on arm.AppAccessId = aa.id
			left join AppMenuMaster amm on aa.id = amm.AppAccessId
			left join LangMaster lur on ur.RoleCode = lur.Code and lur.CodeType = 'ROLES' and lur.Language = @Lang and lur.Active = 'Y'
			left join LangMaster lAMM on amm.MainMenuCode = lAMM.Code and lAMM.CodeType = 'MENUS' and lAMM.Language = @Lang and lAMM.Active = 'Y'
			left join LangMaster lASM on amm.SubMenuCode = lASM.Code and lASM.CodeType = 'SUBMENUS' and lASM.Language = @Lang and lASM.Active = 'Y'
			left join LangMaster lAA on aa.AccessCode = lAA.Code and lAA.CodeType = 'ACCESS' and lASM.Language = @Lang and lASM.Active = 'Y'
			left join LangMaster lAMD on amm.ModuleCode = lAMD.Code and lAMD.CodeType = 'MODULES' and lAMD.Language = @Lang and lAMD.Active = 'Y'
		WHERE 
			ur.Active ='Y'
			AND ur.RoleCode = 'ITADMIN'
			AND arm.Active = 'Y' 
			AND aa.Active =  'Y'
			and ISNULL(amm.Active,'Y') = 'Y'
	END
END;


GO


