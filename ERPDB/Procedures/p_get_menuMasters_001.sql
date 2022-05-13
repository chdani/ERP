IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'p_get_menuMasters_001')
	DROP PROCEDURE p_get_menuMasters_001
Go 


CREATE PROCEDURE [dbo].[p_get_menuMasters_001](
	@Id UNIQUEIDENTIFIER = null,
	@MainMenuCode NVARCHAR(20) = null,
	@MainMenuName NVARCHAR(50) = null, 
	@SubMenuName NVARCHAR(50) = null, 
	@SubMenuCode NVARCHAR(20) = null, 
	@AppAccessId UNIQUEIDENTIFIER = null,
	@Active NVARCHAR(1) = null
) as
BEGIN
	 SELECT 
		Id, MainMenuCode, MainMenuName, MainMenuIcon, MainMenuDispOrd, SubMenuName, SubMenuCode, 
		SubMenuIcon,DispOrder, AppAccessId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
	FROM AppMenuMaster 
	WHERE 
		(@Id IS NULL OR id = @Id)
		AND (@MainMenuCode IS NULL OR MainMenuCode = @MainMenuCode)
		AND (@MainMenuName IS NULL OR MainMenuName = @MainMenuName)
		AND (@SubMenuName IS NULL OR SubMenuName = @SubMenuName)
		AND (@SubMenuCode IS NULL OR SubMenuCode = @SubMenuCode)
		AND (@AppAccessId IS NULL OR AppAccessId = @AppAccessId)
		AND (@Active IS NULL OR Active = @Active)
END;
GO


