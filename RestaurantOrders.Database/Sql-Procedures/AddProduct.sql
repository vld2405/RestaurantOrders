CREATE OR ALTER PROCEDURE [dbo].[AddProduct]
    @Name NVARCHAR(100),
    @Quantity INT,
    @CategoryId INT,
    @Price DECIMAL(6, 2),
    @RestaurantStockQuantity INT,
    @AllergenIds NVARCHAR(MAX) = NULL  -- Comma-separated allergen IDs
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Check if product name already exists
        IF EXISTS (SELECT 1 FROM [dbo].[Products] WHERE Name = @Name)
        BEGIN
            SELECT -1 AS ProductId, 'Product with this name already exists' AS Message;
            ROLLBACK;
            RETURN;
        END
        
        -- Check if category exists
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Categories] WHERE Id = @CategoryId)
        BEGIN
            SELECT -2 AS ProductId, 'Category does not exist' AS Message;
            ROLLBACK;
            RETURN;
        END
        
        -- Insert product
        INSERT INTO [dbo].[Products] (
            Name,
            Quantity,
            CategoryId,
            Price
        )
        VALUES (
            @Name,
            @Quantity,
            @CategoryId,
            @Price
        );
        
        DECLARE @ProductId INT = SCOPE_IDENTITY();
        
        -- Insert restaurant stock
        INSERT INTO [dbo].[RestaurantStocks] (
            ProductId,
            StockQuantity
        )
        VALUES (
            @ProductId,
            @RestaurantStockQuantity
        );
        
        -- Process allergens if provided
        IF @AllergenIds IS NOT NULL AND LEN(@AllergenIds) > 0
        BEGIN
            -- Table variable to hold the parsed IDs
            DECLARE @AllergenTable TABLE (AllergenId INT);
            
            -- Split the comma-delimited string
            INSERT INTO @AllergenTable (AllergenId)
            SELECT CAST(value AS INT)
            FROM STRING_SPLIT(@AllergenIds, ',')
            WHERE TRIM(value) <> '';
            
            -- Link product to allergens
            INSERT INTO [dbo].[AllergenProduct] (ProductId, AllergenId)
            SELECT @ProductId, AllergenId
            FROM @AllergenTable
            WHERE EXISTS (SELECT 1 FROM [dbo].[Allergens] WHERE Id = AllergenId);
        END
        
        COMMIT;
        
        SELECT @ProductId AS ProductId, 'Product added successfully' AS Message;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;
            
        -- Return error information
        SELECT -999 AS ProductId, 
               ERROR_MESSAGE() AS Message;
    END CATCH
END