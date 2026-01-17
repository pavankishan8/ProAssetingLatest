using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProAssetin.API.Models
{
    public class Asset
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string AssetId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string? Location { get; set; }
        
        [StringLength(50)]
        public string Status { get; set; } = "In-Stock"; // In-Stock, Repair, Sold, Damaged, E-Waste
        
        [StringLength(100)]
        public string? Make { get; set; }
        
        [StringLength(100)]
        public string? Model { get; set; }
        
        [StringLength(100)]
        public string? SerialNumber { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PurchasePrice { get; set; }
        
        public DateTime? PurchaseDate { get; set; }
        
        public DateTime? WarrantyExpiryDate { get; set; }
        
        [StringLength(200)]
        public string? Description { get; set; }
        
        [StringLength(450)]
        public string? AssignedToUserId { get; set; }
        
        public int? VendorId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string TenantId { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        [ForeignKey("AssignedToUserId")]
        public virtual ApplicationUser? AssignedToUser { get; set; }
        
        [ForeignKey("VendorId")]
        public virtual Vendor? Vendor { get; set; }
    }
}

