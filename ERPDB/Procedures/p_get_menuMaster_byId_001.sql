IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'p_get_menuMaster_byId_001')
	DROP PROCEDURE p_get_menuMaster_byId_001
Go 

CREATE PROCEDURE [dbo].[p_get_menuMaster_byId_001](
	@Id UNIQUEIDENTIFIER 
) as
BEGIN
	 SELECT 
		Id,MainMenuCode, MainMenuName, MainMenuIcon, MainMenuDispOrd, SubMenuName, SubMenuCode, SubMenuIcon,DispOrder, AppAccessId, CreatedBy, CreatedDate,ModifiedBy, ModifiedDate, Active
	FROM AppMenuMaster 
	WHERE  id = @Id
END;
GO


