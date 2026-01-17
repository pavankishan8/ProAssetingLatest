using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProAssetin.API.Models
{
    public class InventoryLog
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int AssetId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Action { get; set; } = string.Empty; // Added, Removed, Updated, Audited
        
        [StringLength(450)]
        public string? PerformedByUserId { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Quantity { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [Required]
        [StringLength(100)]
        public string TenantId { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        [ForeignKey("AssetId")]
        public virtual Asset Asset { get; set; } = null!;
        
        [ForeignKey("PerformedByUserId")]
        public virtual ApplicationUser? PerformedByUser { get; set; }
    }
}

