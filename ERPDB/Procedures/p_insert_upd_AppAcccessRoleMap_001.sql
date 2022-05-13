IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'p_insert_upd_AppAcccessRoleMap_001')
	DROP PROCEDURE p_insert_upd_AppAcccessRoleMap_001
Go

CREATE PROCEDURE [dbo].[p_insert_upd_AppAcccessRoleMap_001](
	@Id UNIQUEIDENTIFIER = null,
	@UserRoleId UNIQUEIDENTIFIER,
	@AppAccessId UNIQUEIDENTIFIER,
	@AllowAdd NVARCHAR(1) = 'N',
	@AllowEdit NVARCHAR(1) = 'N',
	@AllowDelete NVARCHAR(1) = 'N',
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
	 	IF(NOT EXISTS(select id from AppAccessRoleMap where AppAccessId = @AppAccessId AND UserRoleId = @UserRoleId))
		BEGIN
			INSERT INTO AppAccessRoleMap
				(UserRoleId, AppAccessId, AllowDelete, AllowAdd, AllowEdit, CreatedBy, CreatedDate, Active)
				OUTPUT INSERTED.Id INTO @newGuid
			VALUES	
				(@UserRoleId, @AppAccessId, @AllowDelete, @AllowAdd, @AllowEdit, @CreatedBy, SYSDATETIME(), @Active)
			SELECT @RecordId = ColGuid from @newGuid;

			SET @Result = 'SUCCESS';
		END
		ELSE 
			SET @Result = 'DUPLICATE';

	 END
	 ELSE
	 BEGIN
	 	 	IF(NOT EXISTS(select id from AppAccessRoleMap where ID <> @ID AND  AppAccessId = @AppAccessId AND UserRoleId = @UserRoleId))
			BEGIN
				IF ( exists(select id from AppAccessRoleMap WHERE id = @Id AND (ModifiedDate IS NULL OR ModifiedDate = @ModifiedOn) ))
				BEGIN
					UPDATE AppAccessRoleMap SET
						UserRoleId = @UserRoleId,
						AppAccessId = @AppAccessId,
						AllowDelete = @AllowDelete,
						AllowAdd = @AllowAdd,
						AllowEdit = @AllowEdit,
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


