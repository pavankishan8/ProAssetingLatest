using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test.Models
{
    public class PurchaseModel
    {
        public class PurchaseOrderModel
        {
            public string CompanyName { get; set; }
            public string CompanyAddress { get; set; }
            public string PurchaseOrderNo { get; set; }
            public DateTime OrderDate { get; set; }
            public DateTime DueDate { get; set; }
            public string Notes { get; set; }
            public string TermsAndConditions { get; set; }
            public decimal? TotalAmount { get; set; }
            public string Items { get; set; }
            public DateTime CreatedDate { get; set; }
        }
    }
}