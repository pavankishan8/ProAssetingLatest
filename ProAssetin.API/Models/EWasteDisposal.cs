using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProAssetin.API.Models
{
    /// <summary>
    /// Records electronic waste pickup / disposal and certification tracking.
    /// </summary>
    public class EWasteDisposal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string DisposalReference { get; set; } = string.Empty;

        /// <summary>Optional link to ProAssetinAssets.Id when retiring a tracked asset.</summary>
        public int? AssetId { get; set; }

        [Required]
        [StringLength(500)]
        public string ItemDescription { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Category { get; set; }

        public int Quantity { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? EstimatedWeightKg { get; set; }

        [StringLength(200)]
        public string? RecyclerName { get; set; }

        public DateTime? PickupDate { get; set; }
        public DateTime? DisposalDate { get; set; }

        [StringLength(200)]
        public string? CertificateReference { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Scheduled";

        [StringLength(1000)]
        public string? Notes { get; set; }

        [Required]
        [StringLength(100)]
        public string TenantId { get; set; } = string.Empty;

        [StringLength(450)]
        public string? CreatedByUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("AssetId")]
        public virtual Asset? Asset { get; set; }
    }
}
