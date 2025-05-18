CREATE PROCEDURE [dbo].[AddAllergen]
    @Name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [dbo].[Allergens] WHERE Name = @Name)
    BEGIN
        SELECT -1 AS AllergenId;
        RETURN;
    END
    
    INSERT INTO [dbo].[Allergens] (
        Name
    )
    VALUES (
        @Name
    );
    
    SELECT SCOPE_IDENTITY() AS AllergenId;
END