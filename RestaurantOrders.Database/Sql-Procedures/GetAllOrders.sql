CREATE OR ALTER PROCEDURE [dbo].[GetAllOrders]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        o.Id AS OrderId,
        o.UserId,
        u.FirstName,
        u.LastName,
        u.Email,
        u.PhoneNo,
        u.Address,
        o.OrderState,
        o.CreatedAt,
        o.EstimatedDeliveryTime,
        -- Calculate total order value
        (
            SELECT SUM(
                CASE 
                    WHEN od.ProductId IS NOT NULL THEN p.Price * od.Quantity
                    WHEN od.MenuId IS NOT NULL THEN m.Price * od.Quantity
                    ELSE 0
                END
            )
            FROM OrderDetails od
            LEFT JOIN Products p ON od.ProductId = p.Id
            LEFT JOIN Menus m ON od.MenuId = m.Id
            WHERE od.OrderId = o.Id
        ) AS TotalOrderValue
    FROM 
        Orders o
    INNER JOIN 
        Users u ON o.UserId = u.Id
    ORDER BY 
        o.CreatedAt DESC;
END