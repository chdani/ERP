IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'p_insert_update_appaccess_001')
	DROP PROCEDURE p_insert_update_appaccess_001
Go 


CREATE PROCEDURE [dbo].[p_insert_update_appaccess_001](
	@Id UNIQUEIDENTIFIER = null,
	@AccessName nvarchar(50),
	@AccessCode nvarchar(20),
	@AccessType nvarchar(20),
	@ScreenUrl nvarchar(20) = null,
	@Active NVARCHAR(50),
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
		IF(NOT EXISTS(select id from AppAccess where AccessCode = @AccessCode OR AccessName = @AccessName))
		BEGIN
			INSERT INTO AppAccess
				(AccessCode, AccessName, AccessType, ScreenUrl, CreatedBy, CreatedDate, Active)
				OUTPUT INSERTED.Id INTO @newGuid
			VALUES	
				(@AccessCode, @AccessName,@AccessType, @ScreenUrl, @CreatedBy, SYSDATETIME(), @Active)
			SELECT @RecordId = ColGuid from @newGuid;
			SET @Result = 'SUCCESS';
		END
		ELSE 
			SET @Result = 'DUPLICATE';
	 END
	 ELSE
	 BEGIN
	 		IF(NOT EXISTS(select id from AppAccess where ID <> @ID AND (AccessCode = @AccessCode OR AccessName = @AccessName)))
			BEGIN
				IF ( exists(select id from AppAccess 
						where id = @Id AND (ModifiedDate IS NULL OR ModifiedDate = @ModifiedOn) ))
				BEGIN
					UPDATE AppAccess SET
						AccessCode = @AccessCode,
						AccessName = @AccessName,
						AccessType = @AccessType,
						ScreenUrl = @ScreenUrl,
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


