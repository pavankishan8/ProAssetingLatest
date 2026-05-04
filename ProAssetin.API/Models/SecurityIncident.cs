using System.ComponentModel.DataAnnotations;

namespace ProAssetin.API.Models
{
    /// <summary>
    /// Security incident / finding log for governance and response tracking.
    /// </summary>
    public class SecurityIncident
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string IncidentReference { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? Category { get; set; }

        [StringLength(50)]
        public string Severity { get; set; } = "Medium";

        [StringLength(50)]
        public string Status { get; set; } = "Open";

        public DateTime ReportedDate { get; set; }
        public DateTime? ResolvedDate { get; set; }

        [StringLength(300)]
        public string? AffectedSystem { get; set; }

        [StringLength(200)]
        public string? AssignedToName { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [Required]
        [StringLength(100)]
        public string TenantId { get; set; } = string.Empty;

        [StringLength(450)]
        public string? CreatedByUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
