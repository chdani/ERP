IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'p_insert_upd_usrRoleMap_001')
	DROP PROCEDURE p_insert_upd_usrRoleMap_001
Go 

CREATE PROCEDURE [dbo].[p_insert_upd_usrRoleMap_001](
	@Id UNIQUEIDENTIFIER = null,
	@UserRoleId UNIQUEIDENTIFIER,
	@UserId UNIQUEIDENTIFIER,
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
	 	IF(NOT EXISTS(select id from UserRoleMap where UserRoleId = @UserRoleId AND UserID = @UserId))
		BEGIN
			INSERT INTO UserRoleMap
				(UserRoleId, UserID, CreatedBy, CreatedDate, Active)
				OUTPUT INSERTED.Id INTO @newGuid
			VALUES	
				(@UserRoleId, @UserId, @CreatedBy, SYSDATETIME(), @Active)
			SELECT @RecordId = ColGuid from @newGuid;
			SET @Result = 'SUCCESS';
		END
		ELSE 
			SET @Result = 'DUPLICATE';
	 END
	 ELSE
	 BEGIN
	 	 	IF(NOT EXISTS(select id from UserRoleMap where ID <> @ID AND UserRoleId = @UserRoleId AND UserID = @UserId))
			BEGIN
				IF ( exists(select id from UserRoleMap WHERE id = @Id AND (ModifiedDate IS NULL OR ModifiedDate = @ModifiedOn) ))
				BEGIN
					UPDATE UserRoleMap SET
						UserRoleId = @UserRoleId,
						UserID = @UserId,
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

