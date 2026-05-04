using System.ComponentModel.DataAnnotations;

namespace ProAssetin.API.Models
{
    /// <summary>
    /// Tenant project record for project management tracking.
    /// </summary>
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ProjectReference { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Planning";

        [StringLength(50)]
        public string Priority { get; set; } = "Medium";

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [StringLength(200)]
        public string? ProjectManagerName { get; set; }

        [StringLength(200)]
        public string? DepartmentOrClient { get; set; }

        [StringLength(2000)]
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
