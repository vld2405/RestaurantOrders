CREATE OR ALTER PROCEDURE [dbo].[DeleteAllergen]
    @AllergenId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Check if allergen exists
        IF NOT EXISTS (SELECT 1 FROM Allergens WHERE Id = @AllergenId)
        BEGIN
            SELECT -1 AS Result, 'Allergen not found' AS Message;
            ROLLBACK;
            RETURN;
        END

        -- Check if the allergen is associated with any products
        IF EXISTS (
            SELECT 1 
            FROM AllergenProduct 
            WHERE AllergensId = @AllergenId
        )
        BEGIN
            SELECT -2 AS Result, 'Cannot delete allergen because it is associated with one or more products' AS Message;
            ROLLBACK;
            RETURN;
        END

        -- Delete the allergen
        DELETE FROM Allergens 
        WHERE Id = @AllergenId;

        COMMIT;
        SELECT 1 AS Result, 'Allergen deleted successfully' AS Message;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;
            
        SELECT -999 AS Result, ERROR_MESSAGE() AS Message;
    END CATCH
END