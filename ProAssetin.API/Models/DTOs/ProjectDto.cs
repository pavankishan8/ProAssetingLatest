namespace ProAssetin.API.Models.DTOs
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string ProjectReference { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ProjectManagerName { get; set; }
        public string? DepartmentOrClient { get; set; }
        public string? Notes { get; set; }
        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateProjectDto
    {
        public string ProjectReference { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = "Planning";
        public string Priority { get; set; } = "Medium";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ProjectManagerName { get; set; }
        public string? DepartmentOrClient { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateProjectDto
    {
        public string? ProjectReference { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ProjectManagerName { get; set; }
        public string? DepartmentOrClient { get; set; }
        public string? Notes { get; set; }
    }

    public class ProjectQueryDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
    }
}
