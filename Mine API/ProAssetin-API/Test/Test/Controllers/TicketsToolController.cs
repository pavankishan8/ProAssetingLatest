using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static Test.Models.TicketToolModel;

namespace Test.Controllers
{
    public class TicketsToolController : ApiController
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["Test"].ConnectionString;

        [HttpGet]
        [Route("api/getTicketsCount")]
        public IHttpActionResult GetTicketsSummary()
        {
            Dictionary<string, int> stateCounts = new Dictionary<string, int>();
            int totalRecords = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand totalRecordsCommand = new SqlCommand("SELECT COUNT(*) FROM TicketsTable", connection);
                totalRecords = (int)totalRecordsCommand.ExecuteScalar();

                SqlCommand stateCountsCommand = new SqlCommand("SELECT TaskState, COUNT(*) FROM TicketsTable GROUP BY TaskState", connection);
                SqlDataReader reader = stateCountsCommand.ExecuteReader();
                while (reader.Read())
                {
                    string state = reader.GetString(0);
                    int count = reader.GetInt32(1);
                    stateCounts[state] = count;
                }
                reader.Close();
            }

            return Ok(new { TotalRecords = totalRecords, StateCounts = stateCounts });
        }

        [HttpGet]
        [Route("api/getTicketsData")]
        public IHttpActionResult GetTicketsByEmployeeID(string employeeID)
        {
            List<object> tickets = new List<object>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT * FROM TicketsTable WHERE TaskAssignedToID = @employeeID", connection);
                command.Parameters.AddWithValue("@employeeID", employeeID);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int taskId = reader.GetInt32(0);
                    string taskTitle = reader.GetString(1);
                    string taskState = reader.GetString(5);
                    string tastPriority = reader.GetString(8);
                    string assignedToID = reader.GetString(2);
                    string assignedToName = reader.GetString(3);

                    tickets.Add(new { TaskID = taskId, TaskTitle = taskTitle, TaskState = taskState, Priority = tastPriority,  AssignedToID = assignedToID, AssignedToName = assignedToName });
                }
            }

            return Ok(tickets);
        }

        [HttpGet]
        [Route("api/getAllEmployees")]
        public IHttpActionResult GetEmployees()
        {
            List<Employee> employees = new List<Employee>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();


                string sqlQuery = @"SELECT EmployeeID, Username, Email, Team, ReportingManager, Role 
                                    FROM Employees";

                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Employee employee = new Employee
                            {
                                EmployeeID = reader["EmployeeID"].ToString(),
                                Username = reader["Username"].ToString(),
                                Email = reader["Email"].ToString(),
                                Team = reader["Team"].ToString(),
                                ReportingManager = reader["ReportingManager"].ToString(),
                                Role = reader["Role"].ToString()
                            };

                            employees.Add(employee);
                        }
                    }
                }
            }

            return Ok(employees);
        }

        [HttpPost]
        [Route("api/AddTicket")]
        public IHttpActionResult AddTicket(TicketModel model)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlQuery = $@"
                        INSERT INTO TicketsTable (TaskTitle, TaskAssignedToID, TaskAssignedToName, Tags, TaskState, Description, Discussion, Priority, RemainingWork, TaskCompletedDate, TaskDeletedDate)
                        VALUES (
                            '{model.TaskTitle}',
                            '{model.TaskAssignedToID}',
                            (SELECT FirstName + ' ' + LastName FROM Employees WHERE EmployeeID = '{model.TaskAssignedToID}'),
                            '{model.Tags}',
                            '{model.TaskState}',
                            '{model.Description}',
                            '{model.Discussion}',
                            '{model.Priority}',
                            '{model.RemainingWork}',
                            {(model.TaskCompletedDate != null ? $"'{model.TaskCompletedDate.Value.ToString("yyyy-MM-dd HH:mm:ss")}'" : "NULL")},
                            {(model.TaskDeletedDate != null ? $"'{model.TaskDeletedDate.Value.ToString("yyyy-MM-dd HH:mm:ss")}'" : "NULL")}
                        )";

                    using (var command = new SqlCommand(sqlQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    return Ok("Ticket added successfully.");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/TaskAssignedToNameCounts")]
        public IHttpActionResult GetTaskAssignedToNameCounts()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["Test"].ConnectionString;

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlQuery = @"
                SELECT TaskAssignedToName, COUNT(*) AS TaskCount
                FROM TicketsTable
                WHERE TaskState != 'Done' -- Filter out tasks with TaskState 'Done'
                GROUP BY TaskAssignedToName
                ORDER BY TaskCount DESC";

                    using (var command = new SqlCommand(sqlQuery, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            List<object> taskCounts = new List<object>();

                            while (reader.Read())
                            {
                                var taskCount = new
                                {
                                    label = reader["TaskAssignedToName"].ToString(),
                                    data = Convert.ToInt32(reader["TaskCount"])
                                };
                                taskCounts.Add(taskCount);
                            }

                            return Ok(taskCounts);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/tickets")]
        public IHttpActionResult GetAllTickets(string taskId)
        {
            try
            {

                List<object> tickets = new List<object>();

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    
                    SqlCommand command = new SqlCommand("SELECT * FROM TicketsTable WHERE TaskID = @taskId", connection);
                    command.Parameters.AddWithValue("@taskId", taskId);
                    SqlDataReader reader = command.ExecuteReader();
                    
                            while (reader.Read())
                            {
                                var ticket = new
                                {
                                    TaskID = reader["TaskID"],
                                    TaskTitle = reader["TaskTitle"],
                                    TaskAssignedToID = reader["TaskAssignedToID"],
                                    TaskAssignedToName = reader["TaskAssignedToName"],
                                    Tags = reader["Tags"],
                                    TaskState = reader["TaskState"],
                                    Description = reader["Description"],
                                    Discussion = reader["Discussion"],
                                    Priority = reader["Priority"],
                                    RemainingWork = reader["RemainingWork"],
                                    TaskCreatedDate = reader["TaskCreatedDate"],
                                    TaskModifiedDate = reader["TaskModifiedDate"],
                                    TaskCompletedDate = reader["TaskCompletedDate"],
                                    TaskDeletedDate = reader["TaskDeletedDate"],
                                };
                                tickets.Add(ticket);
                            }

                            return Ok(tickets);
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/getAllTickets")]
        public IHttpActionResult GetAllTicketsFull()
        {
            try
            {
                List<object> tickets = new List<object>();

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("SELECT * FROM TicketsTable", connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            var ticket = new
                            {
                                TaskID = reader["TaskID"],
                                TaskTitle = reader["TaskTitle"],
                                TaskAssignedToID = reader["TaskAssignedToID"],
                                TaskAssignedToName = reader["TaskAssignedToName"],
                                Tags = reader["Tags"],
                                TaskState = reader["TaskState"],
                                Description = reader["Description"],
                                Discussion = reader["Discussion"],
                                Priority = reader["Priority"],
                                RemainingWork = reader["RemainingWork"],
                                TaskCreatedDate = reader["TaskCreatedDate"],
                                TaskModifiedDate = reader["TaskModifiedDate"],
                                TaskCompletedDate = reader["TaskCompletedDate"],
                                TaskDeletedDate = reader["TaskDeletedDate"],
                            };
                            tickets.Add(ticket);
                        }
                    }

                }

                return Ok(tickets);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/UpdateTicket")]
        public IHttpActionResult UpdateTicket(string taskId, TicketModel model)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string isClosed = model.TaskState.Equals("Done", StringComparison.OrdinalIgnoreCase) ? "1" : "0";

                    string completedDate = isClosed == "1" ? $"GETDATE()" : "NULL";

                    string sqlQuery = $@"
                    UPDATE TicketsTable
                    SET 
                    TaskTitle = '{model.TaskTitle}',
                    TaskAssignedToID = '{model.TaskAssignedToID}',
                    TaskAssignedToName = (SELECT FirstName + ' ' + LastName FROM Employees WHERE EmployeeID = '{model.TaskAssignedToID}'),
                    Tags = '{model.Tags}',
                    TaskState = '{model.TaskState}',
                    Description = '{model.Description}',
                    Discussion = '{model.Discussion}',
                    Priority = '{model.Priority}',
                    RemainingWork = '{model.RemainingWork}',
                    TaskCompletedDate = {(model.TaskCompletedDate != null ? $"'{model.TaskCompletedDate.Value.ToString("yyyy-MM-dd HH:mm:ss")}'" : completedDate)},
                    TaskDeletedDate = {(model.TaskDeletedDate != null ? $"'{model.TaskDeletedDate.Value.ToString("yyyy-MM-dd HH:mm:ss")}'" : "NULL")},
                    TaskModifiedDate = GETDATE(),
                    IsClosed = {isClosed}
                    WHERE TaskID = '{taskId}'";

                    using (var command = new SqlCommand(sqlQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    return Ok("Ticket updated successfully.");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}