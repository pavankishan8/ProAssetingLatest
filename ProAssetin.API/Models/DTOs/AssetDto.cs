namespace ProAssetin.API.Models.DTOs
{
    public class AssetDto
    {
        public int Id { get; set; }
        public string AssetId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Make { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public decimal? PurchasePrice { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? WarrantyExpiryDate { get; set; }
        public string? Description { get; set; }
        public string? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }
        public int? VendorId { get; set; }
        public string? VendorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    
    public class CreateAssetDto
    {
        public string AssetId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string Status { get; set; } = "Available";
        public string? Make { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public decimal? PurchasePrice { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? WarrantyExpiryDate { get; set; }
        public string? Description { get; set; }
        public string? AssignedToUserId { get; set; }
        public int? VendorId { get; set; }
    }
    
    public class UpdateAssetDto
    {
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? Location { get; set; }
        public string? Status { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public decimal? PurchasePrice { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? WarrantyExpiryDate { get; set; }
        public string? Description { get; set; }
        public string? AssignedToUserId { get; set; }
        public int? VendorId { get; set; }
    }
    
    public class AssetQueryDto
    {
        public string? SearchTerm { get; set; }
        public string? Category { get; set; }
        public string? Status { get; set; }
        public string? Location { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "Name";
        public bool SortDescending { get; set; } = false;
    }

    public class AllocateAssetDto
    {
        public string UserId { get; set; } = string.Empty;
    }
}

