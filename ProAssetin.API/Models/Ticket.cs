using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProAssetin.API.Models
{
    public class Ticket
    {
        [Key]
        public int TaskID { get; set; }

        [Required]
        [StringLength(500)]
        public string TaskTitle { get; set; } = string.Empty;

        [StringLength(450)]
        public string? TaskAssignedToID { get; set; }

        [StringLength(200)]
        public string? TaskAssignedToName { get; set; }

        [StringLength(50)]
        public string TaskState { get; set; } = "Open"; // Open, In Progress, Resolved, Closed, Cancelled

        [StringLength(50)]
        public string? Priority { get; set; } // Low, Medium, High, Critical

        [StringLength(2000)]
        public string? Description { get; set; }

        [StringLength(2000)]
        public string? Resolution { get; set; }

        [Required]
        [StringLength(100)]
        public string TenantId { get; set; } = string.Empty;

        [StringLength(450)]
        public string? CreatedByUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }

        // Navigation properties
        [ForeignKey("TaskAssignedToID")]
        public virtual ApplicationUser? AssignedToUser { get; set; }
    }
}

