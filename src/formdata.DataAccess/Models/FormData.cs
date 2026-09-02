using System.ComponentModel.DataAnnotations.Schema;
namespace formdata.DataAccess.Models
{
    public class FormData
    {
        [Column("FormDataId")]       
        public Guid Id { get; set; }
        public required string Subject { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public int? Priority { get; set; }  // Must be between 1 and 10
        public bool? Critical { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? UpdatedBy { get; set;}
        public bool Active { get; set; } 
    }
}
