CREATE OR ALTER PROCEDURE [dbo].[GetAllergensByMenuId]
    @MenuId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT DISTINCT
        a.Id,
        a.Name
    FROM 
        [dbo].[Allergens] a
    INNER JOIN 
        [dbo].[AllergenProduct] ap ON a.Id = ap.AllergensId
    INNER JOIN 
        [dbo].[Products] p ON ap.ProductsId = p.Id
    INNER JOIN 
        [dbo].[MenuDetails] md ON p.Id = md.ProductId
    WHERE 
        md.MenuId = @MenuId
    ORDER BY 
        a.Name;
END