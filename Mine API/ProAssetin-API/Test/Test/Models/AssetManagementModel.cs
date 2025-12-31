using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Test.Models
{
    public class AssetManagementModel
    {
        public class LoginModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
        public class Asset
        {
            public string AssetID { get; set; }
            public string AssetType { get; set; }
            public string Make { get; set; }
            public string Model { get; set; }
            public string Status { get; set; }
            public string RAM { get; set; }
            public string Processor { get; set; }
            public string OS { get; set; }
            public string Serial_Number { get; set; }
            public string IMEI_Number { get; set; }
            public decimal Purchase_Cost { get; set; }
            public DateTime Purchase_Year { get; set; }
            public string MonthsInUse { get; set; }
            public DateTime NextRecycleDate { get; set; }
            public string AssignedToUserID { get; set; }
            public string Vendor { get; set; } // Add the Vendor field
            public DateTime? SentDate { get; set; } // Add the SentDate field
            public DateTime? ReceiveDate { get; set; } // Add the ReceiveDate field
            public decimal Repair_Cost { get; set; } // Add the Repair_Cost field
            public string RepairStatus { get; set; } // Add the RepairStatus field
            public string Tracking { get; set; } // Add the Tracking field
            public string RepairNotes { get; set; } // Add the RepairNotes field
            public string DeliveredBy { get; set; } // Add the DeliveredBy field
            public string DamagedNotes { get; set; }
            public decimal Sold_Cost { get; set; }
            public DateTime? Sold_Year { get; set; }
            public string SoldNotes { get; set; }
            public string SoldTo { get; set; }
            public string Approvals { get; set; }
            public string EWaste_Vendor { get; set; }
            public string EWasteNotes { get; set; }
            public string EWasteApprovals { get; set; }

            public string Email { get; set; }
            public string Username { get; set; }
            public string PhoneNumber { get; set; }
            public string Location { get; set; }
            public string ReportingManager { get; set; }

            public string AssignedToUserIDString { get; set; }
            public bool expanded { get; set; }

        }


        public class AssetList
        {
            public List<Asset> Assets { get; set; }
        }

        public class InStockItem
        {
            public string AssetID { get; set; }
            public string AssetType { get; set; }
            public string Make { get; set; }
            public string Model { get; set; }
            public string Status { get; set; }
            public string RAM { get; set; }
            public string Processor { get; set; }
            public string OS { get; set; }
            public string Serial_Number { get; set; }
            public string IMEI_Number { get; set; }
            public decimal Purchase_Cost { get; set; }
            public int Purchase_Year { get; set; }
            public int MonthsInUse { get; set; }
            public DateTime NextRecycleDate { get; set; }
            public string AssignedToUserID { get; set; }
        }

        public class RepairItem
        {
            public string AssetID { get; set; }
            public string AssetType { get; set; }
            public string Make { get; set; }
            public string Model { get; set; }
            public string Status { get; set; }
            public string Vendor { get; set; } // Add the Vendor field
            public DateTime? SentDate { get; set; } // Add the SentDate field
            public DateTime? ReceiveDate { get; set; } // Add the ReceiveDate field
            public string Repair_Cost { get; set; } // Add the Repair_Cost field
            public string RepairStatus { get; set; } // Add the RepairStatus field
            public string Tracking { get; set; } // Add the Tracking field
            public string RepairNotes { get; set; } // Add the RepairNotes field
            public string DeliveredBy { get; set; } // Add the DeliveredBy field
        }

        public class SoldItem
        {
            public string AssetID { get; set; }
            public string AssetType { get; set; }
            public string Make { get; set; }
            public string Processor { get; set; }
            public string Status { get; set; }
            public string Serial_Number { get; set; }
            public string Purchase_Cost { get; set; }
            public string Sold_Cost { get; set; }
            public DateTime? Purchase_Year { get; set; }
            public DateTime? Sold_Year { get; set; }
            public string SoldNotes { get; set; }
            public string SoldTo { get; set; }
            public string Approvals { get; set; }

        }

        public class DamagedItem
        {
            public string AssetID { get; set; }
            public string AssetType { get; set; }
            public string Make { get; set; }
            public string Status { get; set; }
            public string Serial_Number { get; set; }
            public string Purchase_Cost { get; set; }
            public DateTime? Purchase_Year { get; set; }
            public string DamagedNotes { get; set; }

        }

        public class EWasteItem
        {
            public string AssetID { get; set; }
            public string AssetType { get; set; }
            public string Make { get; set; }
            public string Processor { get; set; }
            public string Status { get; set; }
            public string Serial_Number { get; set; }
            public DateTime? Purchase_Year { get; set; }
            public string EWaste_Vendor { get; set; }
            public string EWasteNotes { get; set; }
            public string EWasteApprovals { get; set; }

        }

        public class QueryDefinition
        {
            public string Sql { get; set; }
            public Func<SqlDataReader, Asset> Map { get; set; }
        }

        public class EmployeeDetails
        {
            public string EmployeeID { get; set; }
            public string DomainAccount { get; set; }
            public string EmployeeType { get; set; }
            public string Username { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public DateTime? RegisterDate { get; set; }
            public string LastLoginDate { get; set; }
            public string Role { get; set; }
            public string PhoneNumber { get; set; }
            public string Location { get; set; }
            public string ProjectName { get; set; }
            public string Team { get; set; }
            public string WorkType { get; set; }
            public string ReportingManager { get; set; }
            public string CompanyID { get; set; }
            public string ConfigurationID { get; set; }
        }

        public class AllocateAssetRequest
        {
            public string AssetID { get; set; }
            public string AssetType { get; set; }
            public string Make { get; set; }
            public string Processor { get; set; }
            public string RAM { get; set; }
            public string Status { get; set; }
            public string Serial_Number { get; set; }
            public string AssignedToUserID { get; set; }
            public string Email { get; set; }
            public string Username { get; set; }
            public string PhoneNumber { get; set; }
            public string Location { get; set; }
            public string ReportingManager { get; set; }
        }

    }
}