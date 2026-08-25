namespace bentley.api.Models.Request
{
    public class CreateForm
    {        
        public required string Subject { get; set; }
        public string? Description { get; set; }
        public bool? Critical { get; set; }
        public DateTime? DueDate { get; set; }
        public int? Priority { get; set; }  // Must be between 1 and 10        
        public string CreatedBy { get; set; } = string.Empty;


    }
}
