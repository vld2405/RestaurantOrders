CREATE OR ALTER PROCEDURE [dbo].[GetAllergensByProductId]
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        a.Id,
        a.Name
    FROM 
        [dbo].[Allergens] a
    INNER JOIN 
        [dbo].[AllergenProduct] ap ON a.Id = ap.AllergensId
    WHERE 
        ap.ProductsId = @ProductId
    ORDER BY 
        a.Name;
END