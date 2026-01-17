using Microsoft.AspNetCore.Identity;

namespace ProAssetin.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? TenantId { get; set; }
        public string? CompanyID { get; set; }
        public string? DomainAccount { get; set; }
        public string? EmployeeType { get; set; }
        public string? Location { get; set; }
        public string? ProjectName { get; set; }
        public string? Team { get; set; }
        public string? CustomerName { get; set; }
        public string? WorkType { get; set; }
        public string? ReportingManager { get; set; }
        public DateTime RegisterDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

