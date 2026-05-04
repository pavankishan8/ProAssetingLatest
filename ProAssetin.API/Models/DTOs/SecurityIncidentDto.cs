namespace ProAssetin.API.Models.DTOs
{
    public class SecurityIncidentDto
    {
        public int Id { get; set; }
        public string IncidentReference { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string Severity { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime ReportedDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string? AffectedSystem { get; set; }
        public string? AssignedToName { get; set; }
        public string? Notes { get; set; }
        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateSecurityIncidentDto
    {
        public string IncidentReference { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string Severity { get; set; } = "Medium";
        public string Status { get; set; } = "Open";
        public DateTime ReportedDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string? AffectedSystem { get; set; }
        public string? AssignedToName { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateSecurityIncidentDto
    {
        public string? IncidentReference { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Severity { get; set; }
        public string? Status { get; set; }
        public DateTime? ReportedDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string? AffectedSystem { get; set; }
        public string? AssignedToName { get; set; }
        public string? Notes { get; set; }
    }

    public class SecurityIncidentQueryDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? Status { get; set; }
        public string? Severity { get; set; }
        public DateTime? ReportedDateFrom { get; set; }
        public DateTime? ReportedDateTo { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
    }
}
