CREATE OR ALTER PROCEDURE [dbo].[DeleteCategory]
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Check if category exists
        IF NOT EXISTS (SELECT 1 FROM Categories WHERE Id = @CategoryId)
        BEGIN
            SELECT -1 AS Result, 'Category not found' AS Message;
            ROLLBACK;
            RETURN;
        END

        -- Check if the category is used in any products
        IF EXISTS (
            SELECT 1 
            FROM Products 
            WHERE CategoryId = @CategoryId
        )
        BEGIN
            SELECT -2 AS Result, 'Cannot delete category because it is used by one or more products' AS Message;
            ROLLBACK;
            RETURN;
        END

        -- Check if the category is used in any menus
        IF EXISTS (
            SELECT 1 
            FROM Menus 
            WHERE CategoryId = @CategoryId
        )
        BEGIN
            SELECT -3 AS Result, 'Cannot delete category because it is used in one or more menus' AS Message;
            ROLLBACK;
            RETURN;
        END

        -- Delete the category
        DELETE FROM Categories 
        WHERE Id = @CategoryId;

        COMMIT;
        SELECT 1 AS Result, 'Category deleted successfully' AS Message;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;
            
        SELECT -999 AS Result, ERROR_MESSAGE() AS Message;
    END CATCH
END