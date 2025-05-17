CREATE PROCEDURE [dbo].[AddUser]
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @Email NVARCHAR(100),
    @PhoneNo NVARCHAR(20),
    @Address NVARCHAR(255),
    @Password NVARCHAR(100),
    @UserType INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [dbo].[Users] WHERE Email = @Email)
    BEGIN
        SELECT -1 AS UserId;
        RETURN;
    END
    
    INSERT INTO [dbo].[Users] (
        FirstName,
        LastName,
        Email,
        PhoneNo,
        Address,
        Password,
        UserType
    )
    VALUES (
        @FirstName,
        @LastName,
        @Email,
        @PhoneNo,
        @Address,
        @Password,
        @UserType
    );
    
    SELECT SCOPE_IDENTITY() AS UserId;
END