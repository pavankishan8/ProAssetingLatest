using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test.Models
{
    public class TicketToolModel
    {
        public class Employee
        {
            public string EmployeeID { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string Team { get; set; }
            public string ReportingManager { get; set; }
            public string Role { get; set; }
        }

        public class TicketModel
        {
            [JsonProperty("title")]
            public string TaskTitle { get; set; }
            [JsonProperty("assignedTo")]
            public string TaskAssignedToID { get; set; }
            [JsonProperty("tags")]
            public string Tags { get; set; }
            [JsonProperty("state")]
            public string TaskState { get; set; }
            [JsonProperty("description")]
            public string Description { get; set; }
            [JsonProperty("discussion")]
            public string Discussion { get; set; }
            [JsonProperty("priority")]
            public string Priority { get; set; }
            [JsonProperty("remainingWork")]
            public string RemainingWork { get; set; }
            public DateTime? TaskCompletedDate { get; set; }
            public DateTime? TaskDeletedDate { get; set; }
        }

    }
}