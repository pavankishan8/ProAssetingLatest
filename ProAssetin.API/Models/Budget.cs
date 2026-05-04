using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProAssetin.API.Models
{
    public class Budget
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public int FiscalYear { get; set; }

        [StringLength(100)]
        public string? Category { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AllocatedAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SpentAmount { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Active";

        [Required]
        [StringLength(100)]
        public string TenantId { get; set; } = string.Empty;

        [StringLength(450)]
        public string? CreatedByUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("CreatedByUserId")]
        public virtual ApplicationUser? CreatedByUser { get; set; }
    }
}
