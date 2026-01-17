using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProAssetin.API.Models
{
    public class CompanySettings
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string TenantId { get; set; } = string.Empty;

        // Company Logo - stored as Base64 string
        [Column(TypeName = "nvarchar(max)")]
        public string? CompanyLogo { get; set; } // Base64 encoded logo

        [StringLength(50)]
        public string? CompanyLogoMimeType { get; set; } // image/png, image/jpeg, etc.

        // Company Information
        [Required]
        [StringLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? PhoneNumber { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(100)]
        public string? Industry { get; set; }

        [StringLength(200)]
        public string? SPOCInformation { get; set; } // Single Point of Contact

        [StringLength(50)]
        public string? GSTNumber { get; set; }

        [StringLength(200)]
        public string? Website { get; set; }

        // Additional Configurations
        [StringLength(50)]
        public string? Currency { get; set; } = "USD";

        [StringLength(50)]
        public string? TimeZone { get; set; } = "UTC";

        [StringLength(10)]
        public string? DateFormat { get; set; } = "MM/dd/yyyy";

        [StringLength(10)]
        public string? TimeFormat { get; set; } = "12h"; // 12h or 24h

        // System Settings
        public int DefaultPageSize { get; set; } = 10;

        public bool EnableEmailNotifications { get; set; } = true;

        public bool EnableSMSNotifications { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

