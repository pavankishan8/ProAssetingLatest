namespace ProAssetin.API.Models.DTOs
{
    public class PurchaseOrderDto
    {
        public int Id { get; set; }
        public string PONumber { get; set; } = string.Empty;
        public int? VendorId { get; set; }
        public string? VendorName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PODate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public string? ApprovedByUserId { get; set; }
        public string? ApprovedByUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreatePurchaseOrderDto
    {
        public string PONumber { get; set; } = string.Empty;
        public int? VendorId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PODate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string Status { get; set; } = "Draft";
        public string? Description { get; set; }
    }

    public class UpdatePurchaseOrderDto
    {
        public string? PONumber { get; set; }
        public int? VendorId { get; set; }
        public decimal? TotalAmount { get; set; }
        public DateTime? PODate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
        public string? ApprovedByUserId { get; set; }
    }

    public class PurchaseOrderQueryDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? Status { get; set; }
        public int? VendorId { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
    }
}

