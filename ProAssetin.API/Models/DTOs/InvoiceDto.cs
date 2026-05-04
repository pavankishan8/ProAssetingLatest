namespace ProAssetin.API.Models.DTOs
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string? VendorName { get; set; }
        public decimal Amount { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string Status { get; set; } = string.Empty; // Pending, Paid, Overdue, Cancelled
        public string? Description { get; set; }
        public string? PurchaseOrderNumber { get; set; }
        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateInvoiceDto
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public string? VendorName { get; set; }
        public decimal Amount { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string Status { get; set; } = "Pending";
        public string? Description { get; set; }
        public string? PurchaseOrderNumber { get; set; }
    }

    public class UpdateInvoiceDto
    {
        public string? InvoiceNumber { get; set; }
        public string? VendorName { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
        public string? PurchaseOrderNumber { get; set; }
    }

    public class InvoiceQueryDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? Status { get; set; }
        public string? VendorName { get; set; }
        public DateTime? InvoiceDateFrom { get; set; }
        public DateTime? InvoiceDateTo { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
    }
}

