using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProAssetin.API.Models
{
    public class EmployeeConfiguration
    {
        [Key]
        public int ConfigurationID { get; set; }

        [Required]
        [StringLength(450)]
        public string EmployeeID { get; set; } = string.Empty;

        [StringLength(50)]
        public string? PreDefinedAssetID { get; set; }

        [StringLength(50)]
        public string? GSTNumber { get; set; }

        [Column(TypeName = "varbinary(max)")]
        public byte[]? Image { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        [ForeignKey("EmployeeID")]
        public virtual ApplicationUser? Employee { get; set; }
    }
}

