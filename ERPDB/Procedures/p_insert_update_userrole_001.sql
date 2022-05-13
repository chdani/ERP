
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'p_insert_update_userrole_001')
	DROP PROCEDURE p_insert_update_userrole_001
Go

CREATE PROCEDURE [dbo].[p_insert_update_userrole_001](
	@Id UNIQUEIDENTIFIER = null,
	@RoleName nvarchar(50),
	@RoleCode nvarchar(30),
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
		IF(NOT EXISTS(select id from UserRole where RoleCode = @RoleCode OR RoleName = @RoleName))
		BEGIN
			INSERT INTO UserRole
				(RoleCode, RoleName, CreatedBy, CreatedDate, Active)
				OUTPUT INSERTED.Id INTO @newGuid
			VALUES	
				(@RoleCode, @RoleName, @CreatedBy, SYSDATETIME(), @Active);
			SELECT @RecordId = ColGuid from @newGuid;
			SET @Result = 'SUCCESS';
		END
		ELSE 
			SET @Result = 'DUPLICATE';
	END
	ELSE
	BEGIN
		IF(NOT EXISTS(select id from UserRole where ID <> @ID AND (RoleCode = @RoleCode OR RoleName = @RoleName)))
		BEGIN
			IF ( exists(select id from UserRole 
					where id = @Id AND (ModifiedDate IS NULL OR ModifiedDate = @ModifiedOn) ))
			BEGIN
				UPDATE UserRole SET
					RoleCode = @RoleCode,
					RoleName = @RoleName,
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
 

