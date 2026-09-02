using formdata.DataAccess.Models;

namespace formdata.DataAccess.Repositories.Interfaces
{
    public interface IFormDataRepository
    {        
        Task<FormResponse?> CreateFormRequestAsync(string subject, string? description, bool critical, DateTime? dueDate, int? priority, string createdBy);
        Task<bool?> DeleteFormRequestAsync(Guid id);
        Task<FormDataList> GetFormDataListAsync(int page = 1, int pageSize = 20, string? subjectFilter = null);
        Task<FormResponse?> GetFormRequestByIdAsync(Guid id);
        Task<FormResponse?> UpdateFormRequestAsync(Guid id, string subject, string description, bool critical, DateTime? dueDate, int? priority, string updatedBy);
    }
}
