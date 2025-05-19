CREATE PROCEDURE [dbo].[GetAllAllergens]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name
    FROM 
        [dbo].[Allergens]
    ORDER BY 
        Name;
END