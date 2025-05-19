CREATE OR ALTER PROCEDURE [dbo].[AddOrder]
    @UserId INT,
    @OrderState INT,
    @ProductIds NVARCHAR(MAX),      -- Comma-separated product IDs
    @MenuIds NVARCHAR(MAX) = NULL,  -- Comma-separated menu IDs
    @Quantities NVARCHAR(MAX)       -- Comma-separated quantities
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Get current time
        DECLARE @CurrentTime DATETIME = GETDATE();
        
        -- Calculate estimated delivery time (1 hour from now)
        DECLARE @EstimatedDeliveryTime DATETIME = DATEADD(HOUR, 1, @CurrentTime);
        
        -- Create new order
        INSERT INTO [dbo].[Orders] (
            UserId,
            OrderState,
            CreatedAt,
            EstimatedDeliveryTime
        )
        VALUES (
            @UserId,
            @OrderState,
            @CurrentTime,
            @EstimatedDeliveryTime
        );
        
        DECLARE @OrderId INT = SCOPE_IDENTITY();
        
        -- Parse data
        DECLARE @ProductTable TABLE (ProductId INT, Position INT IDENTITY(1,1));
        DECLARE @MenuTable TABLE (MenuId INT, Position INT IDENTITY(1,1));
        DECLARE @QuantityTable TABLE (Quantity INT, Position INT IDENTITY(1,1));
        
        -- Parse product IDs
        IF @ProductIds IS NOT NULL AND LEN(@ProductIds) > 0
        BEGIN
            INSERT INTO @ProductTable (ProductId)
            SELECT CAST(value AS INT)
            FROM STRING_SPLIT(@ProductIds, ',')
            WHERE TRIM(value) <> '';
        END
        
        -- Parse menu IDs
        IF @MenuIds IS NOT NULL AND LEN(@MenuIds) > 0
        BEGIN
            INSERT INTO @MenuTable (MenuId)
            SELECT CAST(value AS INT)
            FROM STRING_SPLIT(@MenuIds, ',')
            WHERE TRIM(value) <> '';
        END
        
        -- Parse quantities
        INSERT INTO @QuantityTable (Quantity)
        SELECT CAST(value AS INT)
        FROM STRING_SPLIT(@Quantities, ',')
        WHERE TRIM(value) <> '';
        
        -- Validate data
        IF (SELECT COUNT(*) FROM @ProductTable) + (SELECT COUNT(*) FROM @MenuTable) <> (SELECT COUNT(*) FROM @QuantityTable)
        BEGIN
            ROLLBACK;
            RAISERROR('Number of products/menus and quantities do not match', 16, 1);
            RETURN;
        END
        
        -- Add order details for products
        INSERT INTO [dbo].[OrderDetails] (
            OrderId,
            ProductId,
            MenuId,
            Quantity
        )
        SELECT 
            @OrderId,
            pt.ProductId,
            NULL AS MenuId,
            qt.Quantity
        FROM @ProductTable pt
        JOIN @QuantityTable qt ON pt.Position = qt.Position;
        
        -- Add order details for menus (if any)
        IF EXISTS (SELECT 1 FROM @MenuTable)
        BEGIN
            -- Calculate position offset for menu quantities
            DECLARE @ProductCount INT = (SELECT COUNT(*) FROM @ProductTable);
            
            INSERT INTO [dbo].[OrderDetails] (
                OrderId,
                ProductId,
                MenuId,
                Quantity
            )
            SELECT 
                @OrderId,
                NULL AS ProductId,
                mt.MenuId,
                qt.Quantity
            FROM @MenuTable mt
            JOIN @QuantityTable qt ON mt.Position + @ProductCount = qt.Position;
        END
        
        -- Update product quantities in inventory
        UPDATE p
        SET p.Quantity = p.Quantity - od.Quantity
        FROM Products p
        JOIN OrderDetails od ON p.Id = od.ProductId
        WHERE od.OrderId = @OrderId;
        
        COMMIT;
        
        -- Return the order ID, message, and estimated delivery time
        SELECT @OrderId AS OrderId, 
               'Order created successfully' AS Message, 
               @EstimatedDeliveryTime AS EstimatedDeliveryTime;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;
            
        SELECT -1 AS OrderId, ERROR_MESSAGE() AS Message, NULL AS EstimatedDeliveryTime;
    END CATCH
END