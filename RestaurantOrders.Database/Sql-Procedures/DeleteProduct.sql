CREATE OR ALTER PROCEDURE [dbo].[DeleteProduct]
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Check if product exists
        IF NOT EXISTS (SELECT 1 FROM Products WHERE Id = @ProductId)
        BEGIN
            SELECT -1 AS Result, 'Product not found' AS Message;
            ROLLBACK;
            RETURN;
        END

        -- Check if the product is used in any active orders
        IF EXISTS (
            SELECT 1 
            FROM OrderDetails od
            INNER JOIN Orders o ON od.OrderId = o.Id
            WHERE od.ProductId = @ProductId
            AND o.OrderState IN (0, 1, 2) -- Registered, InPreparation, Delivering
        )
        BEGIN
            SELECT -2 AS Result, 'Cannot delete product because it is used in active orders' AS Message;
            ROLLBACK;
            RETURN;
        END

        -- Check if the product is used in any menus
        IF EXISTS (
            SELECT 1 
            FROM MenuDetails 
            WHERE ProductId = @ProductId
        )
        BEGIN
            SELECT -3 AS Result, 'Cannot delete product because it is used in one or more menus' AS Message;
            ROLLBACK;
            RETURN;
        END

        -- Delete any associated allergens
        DELETE FROM AllergenProduct 
        WHERE ProductsId = @ProductId;

        -- Delete any images associated with the product
        DELETE FROM Images 
        WHERE ProductId = @ProductId;

        -- Delete the product from RestaurantStock if it exists
        DELETE FROM RestaurantStocks
        WHERE ProductId = @ProductId;

        -- Delete the product itself
        DELETE FROM Products 
        WHERE Id = @ProductId;

        COMMIT;
        SELECT 1 AS Result, 'Product deleted successfully' AS Message;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;
            
        SELECT -999 AS Result, ERROR_MESSAGE() AS Message;
    END CATCH
END