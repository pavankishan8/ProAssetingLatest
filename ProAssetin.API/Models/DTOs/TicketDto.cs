namespace ProAssetin.API.Models.DTOs
{
    public class TicketDto
    {
        public int Id { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public string? TaskAssignedToName { get; set; }
        public string TaskState { get; set; } = "Open";
        public string? Priority { get; set; }
        public string? Description { get; set; }
        public string? Resolution { get; set; }
        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }

    public class CreateTicketDto
    {
        public string TaskTitle { get; set; } = string.Empty;
        public string? TaskAssignedToName { get; set; }
        public string TaskState { get; set; } = "Open";
        public string? Priority { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateTicketDto
    {
        public string? TaskTitle { get; set; }
        public string? TaskAssignedToName { get; set; }
        public string? TaskState { get; set; }
        public string? Priority { get; set; }
        public string? Description { get; set; }
        public string? Resolution { get; set; }
    }

    public class TicketQueryDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? TaskState { get; set; }
        public string? Priority { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
    }
}
