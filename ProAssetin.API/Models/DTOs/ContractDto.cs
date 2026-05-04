namespace ProAssetin.API.Models.DTOs
{
    public class ContractDto
    {
        public int Id { get; set; }
        public string ContractReference { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? CounterpartyName { get; set; }
        public string? ContractType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RenewalReminderDate { get; set; }
        public decimal? ContractValue { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateContractDto
    {
        public string ContractReference { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? CounterpartyName { get; set; }
        public string? ContractType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RenewalReminderDate { get; set; }
        public decimal? ContractValue { get; set; }
        public string Status { get; set; } = "Draft";
        public string? Notes { get; set; }
    }

    public class UpdateContractDto
    {
        public string? ContractReference { get; set; }
        public string? Title { get; set; }
        public string? CounterpartyName { get; set; }
        public string? ContractType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RenewalReminderDate { get; set; }
        public decimal? ContractValue { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
    }

    public class ContractQueryDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? Status { get; set; }
        public string? ContractType { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
    }
}
