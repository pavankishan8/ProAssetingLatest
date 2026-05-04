namespace ProAssetin.API.Models.DTOs
{
    public class BudgetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int FiscalYear { get; set; }
        public string? Category { get; set; }
        public decimal AllocatedAmount { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateBudgetDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int FiscalYear { get; set; }
        public string? Category { get; set; }
        public decimal AllocatedAmount { get; set; }
        public decimal SpentAmount { get; set; }
        public string Status { get; set; } = "Active";
    }

    public class UpdateBudgetDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? FiscalYear { get; set; }
        public string? Category { get; set; }
        public decimal? AllocatedAmount { get; set; }
        public decimal? SpentAmount { get; set; }
        public string? Status { get; set; }
    }

    public class BudgetQueryDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? Status { get; set; }
        public int? FiscalYear { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
    }
}
