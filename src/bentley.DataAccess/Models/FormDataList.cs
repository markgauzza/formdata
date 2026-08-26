namespace bentley.DataAccess.Models
{
    public class FormDataList
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; } = 0;
        public List<FormData> Results { get; set; } = [];
    }
}
