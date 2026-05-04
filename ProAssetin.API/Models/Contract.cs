using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProAssetin.API.Models
{
    /// <summary>
    /// Vendor or customer contract record for renewal and compliance tracking.
    /// </summary>
    public class Contract
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ContractReference { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(200)]
        public string? CounterpartyName { get; set; }

        [StringLength(100)]
        public string? ContractType { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RenewalReminderDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ContractValue { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Draft";

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
