namespace TaskManagement.Application.DTOs
{
    public class TaskStatusDto
    {
        public string Pending { get; set; } = string.Empty;
        public string InProgress { get; set; } = string.Empty;
        public string Completed { get; set; } = string.Empty;
    }
}
