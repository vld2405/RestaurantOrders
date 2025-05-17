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
    
    -- Check if email already exists
    IF EXISTS (SELECT 1 FROM [dbo].[Users] WHERE Email = @Email)
    BEGIN
        -- Return -1 to indicate email already exists
        SELECT -1 AS UserId;
        RETURN;
    END
    
    -- Insert the new user
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
        @Password, -- Note: In a production system, you should store a salted hash, not the raw password
        @UserType
    );
    
    -- Return the new user's ID
    SELECT SCOPE_IDENTITY() AS UserId;
END