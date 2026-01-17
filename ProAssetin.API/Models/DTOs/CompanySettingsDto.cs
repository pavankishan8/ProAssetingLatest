namespace ProAssetin.API.Models.DTOs
{
    public class CompanySettingsDto
    {
        public int Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public string? CompanyLogo { get; set; } // Base64 encoded logo (data:image/png;base64,...)
        public string? CompanyLogoMimeType { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Industry { get; set; }
        public string? SPOCInformation { get; set; }
        public string? GSTNumber { get; set; }
        public string? Website { get; set; }
        public string? Currency { get; set; }
        public string? TimeZone { get; set; }
        public string? DateFormat { get; set; }
        public string? TimeFormat { get; set; }
        public int DefaultPageSize { get; set; }
        public bool EnableEmailNotifications { get; set; }
        public bool EnableSMSNotifications { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UpdateCompanySettingsDto
    {
        public string? CompanyName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Industry { get; set; }
        public string? SPOCInformation { get; set; }
        public string? GSTNumber { get; set; }
        public string? Website { get; set; }
        public string? Currency { get; set; }
        public string? TimeZone { get; set; }
        public string? DateFormat { get; set; }
        public string? TimeFormat { get; set; }
        public int? DefaultPageSize { get; set; }
        public bool? EnableEmailNotifications { get; set; }
        public bool? EnableSMSNotifications { get; set; }
    }
}

