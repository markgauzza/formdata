using bentley.DataAccess.Models;
using bentley.DataAccess.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace bentley.DataAccess.Repositories
{
    public class FormDataRepository(AppDbContext context) : IFormDataRepository
    {       

        public async Task<FormResponse?> CreateFormRequestAsync(string subject, string? description, bool critical, DateTime? dueDate, int? priority, string createdBy)
        {
            var id = Guid.NewGuid();
            context.FormData.Add(new FormData
            {
                Id = id,
                Subject = subject,
                Description = description,
                DueDate = dueDate,
                Priority = priority,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy,
                Critical = critical,
                Active = true
            });

            await context.SaveChangesAsync();

            var record = await context.FindAsync<FormData>(id);
            if (record == null)
                return null;

            return MapToCreateFormResponse(record);
        }

        public async Task<FormResponse?> GetFormRequestByIdAsync(Guid id)
        {
            var result = await context.FindAsync<FormData>(id);
            if (result == null)
                return null;

            return MapToCreateFormResponse(result);
        }

        public async Task<FormResponse?> UpdateFormRequestAsync(Guid id, string subject, string description, bool critical, DateTime? dueDate, int? priority, string updatedBy)
        {
            var record = await context.FindAsync<FormData>(id);
            if (record == null)
                return null;
            record.Subject = subject;
            record.Description = description;
            record.DueDate = dueDate;
            record.Priority = priority;
            record.Critical = critical;
            record.UpdatedAt = DateTime.UtcNow;
            record.UpdatedBy = updatedBy;
            await context.SaveChangesAsync();
            return MapToCreateFormResponse(record);
        }

        public async Task<bool?> DeleteFormRequestAsync(Guid id)
        {         
            var record = await context.FindAsync<FormData>(id);
            if (record == null)
                return null;
            record.Active = false;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<FormDataList> GetFormDataListAsync(int page = 1, int pageSize = 20, string? subjectFilter = null)
        {
            var paramPage = new SqlParameter("@PageNumber", page);
            var paramPageSize = new SqlParameter("@PageSize", pageSize);
            var paramSubjectFilter = new SqlParameter("@SubjectFilter", SqlDbType.VarChar, 200)
            {
                Value = (object?)subjectFilter ?? DBNull.Value
            };

            var paramTotalCount = new SqlParameter
            {
                ParameterName = "@TotalRecords",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            var records = await context.Database
                .SqlQueryRaw<FormData>(
                    "EXEC spGetFormDataList @PageNumber, @PageSize, @SubjectFilter, @TotalRecords OUTPUT",
                    paramPage, paramPageSize, paramSubjectFilter, paramTotalCount)
                .ToListAsync();

            int totalCount = paramTotalCount.Value != DBNull.Value ? (int)paramTotalCount.Value : 0;

            return new FormDataList
            {
                Results = records,
                PageNumber = page,
                PageSize = pageSize,
                TotalRecords = totalCount
            };
        }

        private static FormResponse MapToCreateFormResponse(FormData formData)
        {
            return new FormResponse(
                Id: formData.Id,
                Subject: formData.Subject,
                Description: formData.Description,
                DueDate: formData.DueDate,
                Priority: formData.Priority,
                Critical: formData.Critical,
                CreatedAt: formData.CreatedAt,
                CreatedBy: formData.CreatedBy,
                UpdatedAt: formData.UpdatedAt,
                UpdatedBy: formData.UpdatedBy
            );
        }
    }

    public record CreateFormRequest(Guid Id, string Subject, string Description, DateTime? DueDate, int? Priority, bool? Critical, DateTime CreatedAt) : IFormValidatable;
    public record UpdateFormRequest(Guid Id, string Subject, string Description, DateTime? DueDate, int? Priority, bool? Critical, DateTime? UpdatedAt) : IFormValidatable;
    public record FormResponse(Guid Id, string Subject, string Description, DateTime? DueDate, int? Priority, bool? Critical, DateTime CreatedAt, DateTime? UpdatedAt, string CreatedBy, string UpdatedBy);
    public record FormListQuery(int Page = 1, int PageSize = 20, string? SubjectFilter = null);

    public interface IFormValidatable
    {
        public string Subject { get; }
        public int? Priority { get; }
    }
}
