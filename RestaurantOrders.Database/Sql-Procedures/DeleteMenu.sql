CREATE OR ALTER PROCEDURE [dbo].[DeleteMenu]
    @MenuId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Check if menu exists
        IF NOT EXISTS (SELECT 1 FROM Menus WHERE Id = @MenuId)
        BEGIN
            SELECT -1 AS Result, 'Menu not found' AS Message;
            ROLLBACK;
            RETURN;
        END

        -- Check if the menu is used in any active orders
        IF EXISTS (
            SELECT 1 
            FROM OrderDetails od
            INNER JOIN Orders o ON od.OrderId = o.Id
            WHERE od.MenuId = @MenuId
            AND o.OrderState IN (0, 1, 2) -- Registered, InPreparation, Delivering
        )
        BEGIN
            SELECT -2 AS Result, 'Cannot delete menu because it is used in active orders' AS Message;
            ROLLBACK;
            RETURN;
        END

        -- Get menu details to delete
        DECLARE @MenuDetailsIDs TABLE (Id INT);
        INSERT INTO @MenuDetailsIDs
        SELECT Id FROM MenuDetails WHERE MenuId = @MenuId;
        
        -- Delete menu details
        DELETE FROM MenuDetails
        WHERE MenuId = @MenuId;
        
        -- Delete the menu from order details for completed or canceled orders (reference only)
        UPDATE OrderDetails
        SET MenuId = NULL
        WHERE MenuId = @MenuId
        AND OrderId IN (
            SELECT Id FROM Orders
            WHERE OrderState IN (3, 4) -- Delivered, Canceled
        );
        
        -- Delete the menu itself
        DELETE FROM Menus 
        WHERE Id = @MenuId;

        COMMIT;
        SELECT 1 AS Result, 'Menu deleted successfully' AS Message;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;
            
        SELECT -999 AS Result, ERROR_MESSAGE() AS Message;
    END CATCH
END