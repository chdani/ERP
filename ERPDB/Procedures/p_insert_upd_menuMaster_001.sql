IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'p_insert_upd_menuMaster_001')
	DROP PROCEDURE p_insert_upd_menuMaster_001
Go 


CREATE PROCEDURE [dbo].[p_insert_upd_menuMaster_001](
	@Id UNIQUEIDENTIFIER = null,
	@MainMenuCode NVARCHAR(20),
	@MainMenuName NVARCHAR(50), 
	@MainMenuIcon NVARCHAR(50), 
	@MainMenuDispOrd int, 
	@SubMenuName NVARCHAR(50), 
	@SubMenuCode NVARCHAR(20), 
	@SubMenuIcon NVARCHAR(50),
	@DispOrder int,
	@AppAccessId UNIQUEIDENTIFIER,
	@Active NVARCHAR(1),
	@CreatedBy UNIQUEIDENTIFIER,
	@ModifiedBy UNIQUEIDENTIFIER = null,
	@ModifiedOn smalldatetime = null,
	@RecordId UNIQUEIDENTIFIER OUTPUT,
	@Result NVARCHAR(20) OUTPUT
) as
BEGIN
	 DECLARE @newGuid TABLE (ColGuid uniqueidentifier)
	  
	 IF @id IS NULL  
	 BEGIN
	 	IF(NOT EXISTS(select id from AppMenuMaster where MainMenuCode = @MainMenuCode OR MainMenuName = @MainMenuName OR 
				SubMenuCode = @SubMenuCode OR SubMenuName = @SubMenuName))
		BEGIN
			INSERT INTO AppMenuMaster
				(MainMenuCode, MainMenuName, MainMenuIcon, MainMenuDispOrd, SubMenuName, SubMenuCode, SubMenuIcon,DispOrder, AppAccessId, CreatedBy, CreatedDate, Active)
				OUTPUT INSERTED.Id INTO @newGuid
			VALUES	
				(@MainMenuCode, @MainMenuName, @MainMenuIcon, @MainMenuDispOrd, @SubMenuName, @SubMenuCode, @SubMenuIcon, @DispOrder, @AppAccessId, @CreatedBy, SYSDATETIME(), @Active)
			SELECT @RecordId = ColGuid from @newGuid;

			SET @Result = 'SUCCESS';
		END
		ELSE 
			SET @Result = 'DUPLICATE';

	 END
	 ELSE
	 BEGIN
	 	 	IF(NOT EXISTS(select id from AppMenuMaster where ID <> @ID AND (MainMenuCode = @MainMenuCode OR MainMenuName = @MainMenuName OR 
				SubMenuCode = @SubMenuCode OR SubMenuName = @SubMenuName)))
			BEGIN
				IF ( exists(select id from AppMenuMaster where id = @Id AND (ModifiedDate IS NULL OR ModifiedDate = @ModifiedOn) ))
				BEGIN
					UPDATE AppMenuMaster SET
						MainMenuCode = @MainMenuCode, 
						MainMenuName = @MainMenuName, 
						MainMenuIcon = MainMenuIcon, 
						MainMenuDispOrd = MainMenuDispOrd, 
						SubMenuName = @SubMenuName, 
						SubMenuCode = @SubMenuCode, 
						SubMenuIcon = @SubMenuIcon,
						DispOrder = @DispOrder, 
						AppAccessId = @AppAccessId,
						ModifiedBy = @ModifiedBy,
						ModifiedDate = SYSDATETIME(),
						Active = @Active
					WHERE Id = @Id 
						AND (ModifiedDate IS NULL OR ModifiedDate = @ModifiedOn);

					SET @RecordId = @id
					SET @Result = 'SUCCESS'
				END
				ELSE 
					SET @Result = 'NOTLATEST';
			END
			ELSE
				SET @Result = 'DUPLICATE';
	 END
END;
GO


