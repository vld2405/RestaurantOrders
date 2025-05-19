CREATE PROCEDURE [dbo].[GetAllCategories]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name
    FROM 
        [dbo].[Categories]
    ORDER BY 
        Name;
END