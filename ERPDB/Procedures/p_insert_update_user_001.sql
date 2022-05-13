IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'p_insert_update_user_001')
	DROP PROCEDURE p_insert_update_user_001
Go 


CREATE PROCEDURE [dbo].[p_insert_update_user_001](
	@Id UNIQUEIDENTIFIER = null,
	@UserName NVARCHAR(20),
	@FirstName NVARCHAR(50),
	@LastName NVARCHAR(50),
	@EmailId NVARCHAR(50),
	@Active NVARCHAR(50),
	@Password NVARCHAR(200) = null,
	@UserType NVARCHAR(1),
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
	 	IF(NOT EXISTS(select id from UserMaster where UserName = @UserName OR EmailId = @EmailId))
		BEGIN
			INSERT INTO UserMaster 
				(UserName, FirstName, LastName, EmailId, CreatedBy, CreatedOn, Active, Password, UserType)
				OUTPUT INSERTED.Id INTO @newGuid
			VALUES	
				(@UserName, @FirstName, @LastName, @EmailId, @CreatedBy, SYSDATETIME(), @Active, @Password, @UserType)
			SELECT @RecordId = ColGuid from @newGuid;
			SET @Result = 'SUCCESS';
		END
		ELSE 
			SET @Result = 'DUPLICATE';

	 END
	 ELSE
	 BEGIN
	 	 	IF(NOT EXISTS(select id from UserMaster where ID <> @ID AND ( UserName = @UserName OR EmailId = @EmailId)))
			BEGIN
				IF ( exists(select id from UserMaster WHERE id = @Id AND (ModifiedOn IS NULL OR ModifiedOn = @ModifiedOn) ))
				BEGIN
					UPDATE UserMaster SET
						UserName = @UserName,
						FirstName = @FirstName,
						LastName = @LastName,
						EmailId = @EmailId,
						ModifiedBy = @ModifiedBy,
						ModifiedOn = SYSDATETIME(),
						Active = @Active,
						UserType = @UserType
					WHERE Id = @Id 
						AND (ModifiedOn IS NULL OR ModifiedOn = @ModifiedOn);
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


