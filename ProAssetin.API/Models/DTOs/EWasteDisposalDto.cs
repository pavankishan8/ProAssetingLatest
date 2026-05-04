namespace ProAssetin.API.Models.DTOs
{
    public class EWasteDisposalDto
    {
        public int Id { get; set; }
        public string DisposalReference { get; set; } = string.Empty;
        public int? AssetId { get; set; }
        public string? AssetTag { get; set; }
        public string ItemDescription { get; set; } = string.Empty;
        public string? Category { get; set; }
        public int Quantity { get; set; }
        public decimal? EstimatedWeightKg { get; set; }
        public string? RecyclerName { get; set; }
        public DateTime? PickupDate { get; set; }
        public DateTime? DisposalDate { get; set; }
        public string? CertificateReference { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateEWasteDisposalDto
    {
        public string DisposalReference { get; set; } = string.Empty;
        public int? AssetId { get; set; }
        public string ItemDescription { get; set; } = string.Empty;
        public string? Category { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal? EstimatedWeightKg { get; set; }
        public string? RecyclerName { get; set; }
        public DateTime? PickupDate { get; set; }
        public DateTime? DisposalDate { get; set; }
        public string? CertificateReference { get; set; }
        public string Status { get; set; } = "Scheduled";
        public string? Notes { get; set; }
    }

    public class UpdateEWasteDisposalDto
    {
        public string? DisposalReference { get; set; }
        public int? AssetId { get; set; }
        public string? ItemDescription { get; set; }
        public string? Category { get; set; }
        public int? Quantity { get; set; }
        public decimal? EstimatedWeightKg { get; set; }
        public string? RecyclerName { get; set; }
        public DateTime? PickupDate { get; set; }
        public DateTime? DisposalDate { get; set; }
        public string? CertificateReference { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
    }

    public class EWasteDisposalQueryDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? Status { get; set; }
        public DateTime? DisposalDateFrom { get; set; }
        public DateTime? DisposalDateTo { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
    }
}
