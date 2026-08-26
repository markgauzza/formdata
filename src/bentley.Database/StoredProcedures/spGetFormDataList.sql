-- =============================================
-- Author:     Mark Gauzza
-- Create Date: 8/26/2026
-- Description: Retrieves a paginated list of form data records
-- =============================================
CREATE PROCEDURE [dbo].[spGetFormDataList]
(
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SubjectFilter nvarchar(200) = null,
    @TotalRecords INT OUTPUT
)
AS
BEGIN 
  
    SET NOCOUNT ON;
    
    SELECT @TotalRecords = COUNT(*) 
    FROM dbo.FormData
        WHERE @SubjectFilter IS NULL OR Subject LIKE '%' + @SubjectFilter + '%';
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT [FormDataId]
      ,[Subject]
      ,[Description]
      ,[DueDate]
      ,[Priority]
      ,[Critical]
      ,[CreatedAt]
      ,[CreatedBy]
      ,[UpdatedAt]
      ,[UpdatedBy]
      ,[Active]
    FROM [dbo].[FormData]
        WHERE Active = 1 
        AND (@SubjectFilter IS NULL OR Subject LIKE '%' + @SubjectFilter + '%')
    ORDER BY 
        CreatedAt
        OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO


