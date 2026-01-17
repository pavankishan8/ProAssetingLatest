using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProAssetin.API.Models
{
    public class Software
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string SoftwareName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Version { get; set; }

        [StringLength(100)]
        public string? LicenseType { get; set; } // Perpetual, Subscription, OEM, Open Source, Freeware

        [StringLength(100)]
        public string? LicenseKey { get; set; }

        public int? VendorId { get; set; }

        [StringLength(200)]
        public string? VendorName { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PurchasePrice { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public DateTime? LicenseExpiryDate { get; set; }

        public int? TotalLicenses { get; set; }

        public int? UsedLicenses { get; set; }

        public int? AvailableLicenses { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(200)]
        public string? InstallationPath { get; set; }

        [StringLength(100)]
        public string? Category { get; set; } // Operating System, Office Suite, Development Tools, Security, etc.

        [StringLength(50)]
        public string Status { get; set; } = "Active"; // Active, Expired, Inactive, Renewal Pending

        [Required]
        [StringLength(100)]
        public string TenantId { get; set; } = string.Empty;

        [StringLength(450)]
        public string? PurchasedByUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("VendorId")]
        public virtual Vendor? Vendor { get; set; }

        [ForeignKey("PurchasedByUserId")]
        public virtual ApplicationUser? PurchasedByUser { get; set; }
    }
}

