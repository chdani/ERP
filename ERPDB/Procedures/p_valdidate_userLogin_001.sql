IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'p_valdidate_userLogin_001')
	DROP PROCEDURE p_valdidate_userLogin_001
Go 

CREATE   PROCEDURE [dbo].[p_valdidate_userLogin_001](
	@UserName  nvarchar(50) 
) as
BEGIN
	SELECT 
		U.Id, 
		u.UserName,
		u.EmailId, 
		u.FirstName, 
		u.LastName, u.EmailId , 
		u.UserType,
		u.EmployeeId
	FROM UserMaster U  
	WHERE 
		(UserName = @UserName  OR EmailId = @UserName)
		AND u.Active = 'Y' ;
END;


GO


 