namespace bentley.api.Repositories.Interfaces
{
    public interface IFormDataRepository
    {        
        Task<FormResponse?> CreateFormRequestAsync(string subject, string? description, bool critical, DateTime? dueDate, int? priority, string createdBy);
        Task<int?> DeleteFormRequestAsync(Guid id);
        Task<FormResponse?> GetFormRequestByIdAsync(Guid id);
        Task<FormResponse?> UpdateFormRequestAsync(Guid id, string subject, string description, bool critical, DateTime? dueDate, int? priority, string updatedBy);
    }
}
