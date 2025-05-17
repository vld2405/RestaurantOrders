CREATE PROCEDURE [dbo].[VerifyUserCredentials]
    @Email NVARCHAR(100),
    @Password NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        Id,
        UserType,
        FirstName,
        LastName,
        Email,
        PhoneNo,
        Address
    FROM 
        [dbo].[Users]
    WHERE 
        Email = @Email 
        AND Password = @Password;
END