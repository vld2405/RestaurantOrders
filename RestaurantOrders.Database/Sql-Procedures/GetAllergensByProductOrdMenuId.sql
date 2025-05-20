-- SQL procedures to retrieve allergens for products and menus
-- These procedures work with the existing AllergenProduct entity

-- Add stored procedure to retrieve allergens for a product
IF OBJECT_ID('dbo.GetAllergensByProductId', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[GetAllergensByProductId];
GO

CREATE PROCEDURE [dbo].[GetAllergensByProductId]
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
GO

-- Add stored procedure to retrieve allergens for a menu (combining allergens from all products in the menu)
IF OBJECT_ID('dbo.GetAllergensByMenuId', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[GetAllergensByMenuId];
GO

CREATE PROCEDURE [dbo].[GetAllergensByMenuId]
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
        [dbo].[AllergenProduct] ap ON a.Id = ap.AllergenId
    INNER JOIN 
        [dbo].[Products] p ON ap.ProductId = p.Id
    INNER JOIN 
        [dbo].[MenuDetails] md ON p.Id = md.ProductId
    WHERE 
        md.MenuId = @MenuId
    ORDER BY 
        a.Name;
END
GO