using bentley.api.Data;
using bentley.api.Models;
using bentley.api.Repositories.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace bentley.api.Repositories
{
    public class FormDataRepository(AppDbContext context) : IFormDataRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<FormResponse?> CreateFormRequestAsync(string subject, string? description, bool critical, DateTime? dueDate, int? priority, string createdBy)
        {
            var id = Guid.NewGuid();
            _context.FormData.Add(new FormData
            {
                Id = id,
                Subject = subject,
                Description = description,
                DueDate = dueDate,
                Priority = priority,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy,
                Critical = critical
            });

            await _context.SaveChangesAsync();

            var record = await _context.FindAsync<FormData>(id);
            if (record == null)
                return null;

            return MapToCreateFormResponse(record);
        }

        public async Task<FormResponse?> GetFormRequestByIdAsync(Guid id)
        {
            var result = await _context.FindAsync<FormData>(id);
            if (result == null)
                return null;

            return MapToCreateFormResponse(result);
        }

        public async Task<FormResponse?> UpdateFormRequestAsync(Guid id, string subject, string description, bool critical, DateTime? dueDate, int? priority, string updatedBy)
        {
            var record = await _context.FindAsync<FormData>(id);
            if (record == null)
                return null;
            record.Subject = subject;
            record.Description = description;
            record.DueDate = dueDate;
            record.Priority = priority;
            record.Critical = critical;
            record.UpdatedAt = DateTime.UtcNow;
            record.UpdatedBy = updatedBy;
            await _context.SaveChangesAsync();
            return MapToCreateFormResponse(record);
        }

        public async Task<int?> DeleteFormRequestAsync(Guid id)
        {
            var record = await _context.FindAsync<FormData>(id);
            if (record == null)
                return null;
            _context.FormData.Remove(record);
            return await _context.SaveChangesAsync();

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

    public record CreateFormRequest(Guid Id, string Subject, string Description, DateTime? DueDate, int? Priority, bool? Critical, DateTime CreatedAt, string CreatedBy) : IFormValidatable;
    public record UpdateFormRequest(Guid Id, string Subject, string Description, DateTime? DueDate, int? Priority, bool? Critical, DateTime CreatedAt, DateTime? UpdatedAt, string UpdatedBy) : IFormValidatable;
    public record FormResponse(Guid Id, string Subject, string Description, DateTime? DueDate, int? Priority, bool? Critical, DateTime CreatedAt, DateTime? UpdatedAt, string CreatedBy, string UpdatedBy);

    public interface IFormValidatable
    {
        public string Subject { get; }
        public int? Priority { get; }
    }
}
