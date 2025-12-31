using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Test.Models;
using static Test.Models.LoginModel;

namespace Test.Controllers
{
    [EnableCors("*", "*", "*", "*")]
    public class LoginController : ApiController
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["Test"].ConnectionString;

        [HttpPost]
        [Route("api/login")]
        public IHttpActionResult LoginUser(UserLoginModel model)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Define the SQL query to retrieve user information based on email
                    string sql = "SELECT E.EmployeeID, E.DomainAccount, E.EmployeeType, E.Username, E.FirstName, E.LastName, E.Email, E.Password, " +
                    "E.RegisterDate, E.LastLoginDate, E.Role, E.PhoneNumber, E.Location, E.ProjectName, E.Team, E.CustomerName, " +
                    "E.WorkType, E.ReportingManager, C.CompanyID, C.CompanyName, C.Address AS CompanyAddress, C.PhoneNumber AS CompanyPhoneNumber, C.Industry " +
                    "FROM Employees E " +
                    "LEFT JOIN Company C ON E.CompanyID = C.CompanyID " +
                    "WHERE E.Email = @Email";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Email", model.Email);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedPlainTextPassword = reader.GetString(7); // Assumes plain text password
                                if (model.Password == storedPlainTextPassword)
                                {
                                    // Passwords match, user is authenticated

                                    // Update LastLoginDate in the database
                                    //DateTime loginTime = DateTime.Now;
                                    //string employeeId = reader.GetString(0);

                                    var userData = new
                                    {
                                        EmployeeID = reader.GetString(0), // Retrieve EmployeeID from Employees table as a string
                                        DomainAccount = reader.GetString(1),
                                        EmployeeType = reader.GetString(2),
                                        CompanyID = reader.GetString(18), // Retrieve CompanyID from Employees table as a string
                                        Username = reader.GetString(3), // Retrieve Username as a string
                                        FirstName = reader.GetString(4), // Retrieve FirstName as a string
                                        LastName = reader.GetString(5), // Retrieve LastName as a strin
                                        Email = reader.GetString(6), // Retrieve Email as a string
                                        Role = reader.GetString(10), // Retrieve Role as a string
                                        PhoneNumber = reader.GetString(11), // Retrieve PhoneNumber as a string
                                        Location = reader.GetString(12), // Retrieve Location as a string
                                        ProjectName = reader.GetString(13), // Retrieve ProjectName as a string
                                        Team = reader.GetString(14), // Retrieve Team as a string
                                        CustomerName = reader.GetString(15), // Retrieve CustomerName as a string
                                        WorkType = reader.GetString(16), // Retrieve WorkType as a string
                                        ReportingManager = reader.GetString(17), // Retrieve ReportingManager as a string
                                        CompanyName = reader.GetString(19), // Retrieve CompanyName from Company table as a string
                                        CompanyAddress = reader.GetString(20), // Retrieve CompanyAddress from Company table as a string
                                        CompanyPhoneNumber = reader.GetString(21), // Retrieve CompanyPhoneNumber from Company table as a string
                                        Industry = reader.GetString(22) // Retrieve Industry from Company table as a string
                                    };

                                    reader.Close();

                                    DateTime loginTime = DateTime.Now;
                                    string employeeId = userData.EmployeeID;

                                    using (SqlConnection updateConnection = new SqlConnection(connectionString))
                                    {
                                        updateConnection.Open();
                                        string updateLastLoginSql = "UPDATE Employees SET LastLoginDate = @LoginTime WHERE EmployeeID = @EmployeeID";
                                        using (SqlCommand updateCommand = new SqlCommand(updateLastLoginSql, updateConnection))
                                        {
                                            updateCommand.Parameters.AddWithValue("@LoginTime", loginTime);
                                            updateCommand.Parameters.AddWithValue("@EmployeeID", employeeId);

                                            // Execute the update command
                                            updateCommand.ExecuteNonQuery();
                                        }
                                    }

                                    // Return the success status and user data to the client
                                    return Ok(new { success = true, user = userData });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                // For now, just returning a simple error message
                return InternalServerError(ex);
            }

            // Authentication failed
            return Ok(new { success = false });
        }
    }
}
