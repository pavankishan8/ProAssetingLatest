namespace ProAssetin.API.Models.DTOs
{
    public class SoftwareDto
    {
        public int Id { get; set; }
        public string SoftwareName { get; set; } = string.Empty;
        public string? Version { get; set; }
        public string? LicenseType { get; set; }
        public string? LicenseKey { get; set; }
        public int? VendorId { get; set; }
        public string? VendorName { get; set; }
        public decimal? PurchasePrice { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
        public int? TotalLicenses { get; set; }
        public int? UsedLicenses { get; set; }
        public int? AvailableLicenses { get; set; }
        public string? Description { get; set; }
        public string? InstallationPath { get; set; }
        public string? Category { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? PurchasedByUserId { get; set; }
        public string? PurchasedByUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateSoftwareDto
    {
        public string SoftwareName { get; set; } = string.Empty;
        public string? Version { get; set; }
        public string? LicenseType { get; set; }
        public string? LicenseKey { get; set; }
        public int? VendorId { get; set; }
        public decimal? PurchasePrice { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
        public int? TotalLicenses { get; set; }
        public int? UsedLicenses { get; set; }
        public string? Description { get; set; }
        public string? InstallationPath { get; set; }
        public string? Category { get; set; }
        public string Status { get; set; } = "Active";
    }

    public class UpdateSoftwareDto
    {
        public string? SoftwareName { get; set; }
        public string? Version { get; set; }
        public string? LicenseType { get; set; }
        public string? LicenseKey { get; set; }
        public int? VendorId { get; set; }
        public decimal? PurchasePrice { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
        public int? TotalLicenses { get; set; }
        public int? UsedLicenses { get; set; }
        public string? Description { get; set; }
        public string? InstallationPath { get; set; }
        public string? Category { get; set; }
        public string? Status { get; set; }
    }

    public class SoftwareQueryDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? Status { get; set; }
        public string? Category { get; set; }
        public string? LicenseType { get; set; }
        public int? VendorId { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
    }
}

