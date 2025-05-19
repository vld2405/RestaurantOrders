CREATE PROCEDURE [dbo].[AddMenu]
    @Name NVARCHAR(100),
    @CategoryId INT, -- Category ID (should generally be 6 for Menu category)
    @ProductIds NVARCHAR(MAX), -- Comma-separated product IDs
    @ProductQuantities NVARCHAR(MAX), -- Comma-separated quantities corresponding to products
    @Price DECIMAL(6, 2) = NULL -- Optional: if NULL, will calculate based on products with discount
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Check if menu name already exists
        IF EXISTS (SELECT 1 FROM [dbo].[Menus] WHERE Name = @Name)
        BEGIN
            SELECT -1 AS MenuId, 'Menu with this name already exists' AS Message;
            ROLLBACK;
            RETURN;
        END
        
        -- Check if category exists
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Categories] WHERE Id = @CategoryId)
        BEGIN
            SELECT -2 AS MenuId, 'Category does not exist' AS Message;
            ROLLBACK;
            RETURN;
        END
        
        -- Create tables to hold parsed product IDs and quantities
        DECLARE @ProductTable TABLE (ProductId INT, Position INT IDENTITY(1,1));
        DECLARE @QuantityTable TABLE (Quantity INT, Position INT IDENTITY(1,1));
        
        -- Parse product IDs
        INSERT INTO @ProductTable (ProductId)
        SELECT CAST(value AS INT)
        FROM STRING_SPLIT(@ProductIds, ',')
        WHERE TRIM(value) <> '';
        
        -- Parse quantities
        INSERT INTO @QuantityTable (Quantity)
        SELECT CAST(value AS INT)
        FROM STRING_SPLIT(@ProductQuantities, ',')
        WHERE TRIM(value) <> '';
        
        -- Verify all products exist
        IF EXISTS (
            SELECT ProductId FROM @ProductTable 
            WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Id = ProductId)
        )
        BEGIN
            SELECT -3 AS MenuId, 'One or more products do not exist' AS Message;
            ROLLBACK;
            RETURN;
        END
        
        -- Check matching counts of product IDs and quantities
        IF (SELECT COUNT(*) FROM @ProductTable) <> (SELECT COUNT(*) FROM @QuantityTable)
        BEGIN
            SELECT -4 AS MenuId, 'Number of products and quantities must match' AS Message;
            ROLLBACK;
            RETURN;
        END
        
        -- Calculate menu price if not provided
        DECLARE @CalculatedPrice DECIMAL(6, 2) = 0;
        DECLARE @MenuDiscount DECIMAL(5, 2);
        
        -- Get discount percentage from app settings
        SELECT @MenuDiscount = 10; -- Default 10% discount
        
        -- Try to get discount from AppSettings table (if you have one)
        IF OBJECT_ID('dbo.AppSettings') IS NOT NULL
        BEGIN
            SELECT @MenuDiscount = CAST(Value AS DECIMAL(5, 2))
            FROM AppSettings
            WHERE [Key] = 'MenuDiscount';
            
            IF @MenuDiscount IS NULL
                SET @MenuDiscount = 10;
        END
        
        -- Calculate price based on products with discount
        IF @Price IS NULL
        BEGIN
            SELECT @CalculatedPrice = SUM(p.Price * qt.Quantity) * (1 - (@MenuDiscount / 100))
            FROM @ProductTable pt
            JOIN @QuantityTable qt ON pt.Position = qt.Position
            JOIN Products p ON pt.ProductId = p.Id;
            
            -- Round to 2 decimal places
            SET @CalculatedPrice = ROUND(@CalculatedPrice, 2);
        END
        ELSE
        BEGIN
            SET @CalculatedPrice = @Price;
        END
        
        -- Insert menu
        INSERT INTO [dbo].[Menus] (
            Name,
            CategoryId,
            Price
        )
        VALUES (
            @Name,
            @CategoryId,
            @CalculatedPrice
        );
        
        DECLARE @MenuId INT = SCOPE_IDENTITY();
        
        -- Insert menu details (links to products)
        INSERT INTO [dbo].[MenuDetails] (
            MenuId,
            ProductId,
            Quantity
        )
        SELECT 
            @MenuId,
            pt.ProductId,
            qt.Quantity
        FROM @ProductTable pt
        JOIN @QuantityTable qt ON pt.Position = qt.Position;
        
        COMMIT;
        
        SELECT @MenuId AS MenuId, 'Menu created successfully' AS Message, @CalculatedPrice AS CalculatedPrice;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;
            
        SELECT -999 AS MenuId, 
               ERROR_MESSAGE() AS Message;
    END CATCH
END