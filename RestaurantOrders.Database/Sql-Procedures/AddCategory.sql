CREATE PROCEDURE [dbo].[AddCategory]
    @Name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [dbo].[Categories] WHERE Name = @Name)
    BEGIN
        SELECT -1 AS CategoryId;
        RETURN;
    END
    
    INSERT INTO [dbo].[Categories] (
        Name
    )
    VALUES (
        @Name
    );
    
    SELECT SCOPE_IDENTITY() AS CategoryId;
END