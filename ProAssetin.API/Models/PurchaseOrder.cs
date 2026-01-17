using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProAssetin.API.Models
{
    public class PurchaseOrder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string PONumber { get; set; } = string.Empty;

        public int? VendorId { get; set; }

        [StringLength(200)]
        public string? VendorName { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public DateTime PODate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Draft"; // Draft, Submitted, Approved, Received, Cancelled

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(100)]
        public string TenantId { get; set; } = string.Empty;

        [StringLength(450)]
        public string? CreatedByUserId { get; set; }

        [StringLength(450)]
        public string? ApprovedByUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("CreatedByUserId")]
        public virtual ApplicationUser? CreatedByUser { get; set; }

        [ForeignKey("VendorId")]
        public virtual Vendor? Vendor { get; set; }
    }
}

