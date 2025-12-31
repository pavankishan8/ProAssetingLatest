using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test.Models
{
    public class UserSettingsModel
    {
        public class EmailModel
        {
            public string To { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
        }

        public class ExchangeRateResponse
        {
            public bool Success { get; set; }
            public long Timestamp { get; set; }
            public string Base { get; set; }
            public string Date { get; set; }
            public Dictionary<string, decimal> Rates { get; set; }
        }

        public class UpdateEmployeeConfigurationModel
        {
            public string EmployeeID { get; set; }
            public string PreDefinedAssetID { get; set; }
            public string Role { get; set; }
        }

        public class UploadEmployeeImageModel
        {
            public string EmployeeID { get; set; }
            public byte[] Image { get; set; }
            public string Role { get; set; }
        }

        public class CompanyModel
        {
            public string CompanyID { get; set; }
            public string CompanyName { get; set; }
            public string Address { get; set; }
            public string PhoneNumber { get; set; }
            public string Industry { get; set; }
            public string SPOCInformation { get; set; }
            public string EmailAccount { get; set; }
        }

        public class EmployeeModel
        {
            public string EmployeeID { get; set; }
            public string DomainAccount { get; set; }
            public string EmployeeType { get; set; }
            public string Username { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string RegisterDate { get; set; }
            public string Role { get; set; }
            public string PhoneNumber { get; set; }
            public string Location { get; set; }
            public string ProjectName { get; set; }
            public string Team { get; set; }
            public string CustomerName { get; set; }
            public string WorkType { get; set; }
            public string ReportingManager { get; set; }
            public string CompanyID { get; set; }
            public string ConfigurationID { get; set; }
        }

        public class EmployeeConfigModel
        {
            public int ConfigurationID { get; set; }
            public string PreDefinedAssetID { get; set; }
            public string GSTNumber { get; set; }
            public byte[] Image { get; set; }
        }

        public class RegistrationData
        {
            public CompanyModel CompanyModel { get; set; }
            public EmployeeModel EmployeeModel { get; set; }
            public EmployeeConfigModel EmployeeConfigModel { get; set; }
            public string UserRole { get; set; }
        }

        public class UserCheckModel
        {
            public string CEmail { get; set; }
            public string EEmail { get; set; }
            public string UName { get; set; }
        }

        public class EmployeeDetailsModel
        {
            public string EmployeeID { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string Team { get; set; }
            public string ReportingManager { get; set; }
            public string Role { get; set; }
        }

        public class EmployeeUpdate
        {
            public string EmployeeID { get; set; }
            public string Role { get; set; }
            public string UserRole { get; set; }
        }
    }
}