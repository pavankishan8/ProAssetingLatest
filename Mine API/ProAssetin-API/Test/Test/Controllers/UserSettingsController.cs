using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;
using System.Web.Http.Cors;
using System.DirectoryServices;
using Test.Utils;
using static Test.Models.UserSettingsModel;
using System.IO;
using System.Web;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace Test.Controllers
{
    [EnableCors("*", "*", "*", "*")]
    public class UserSettingsController : ApiController
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["Test"].ConnectionString;
        private const string ApiKey = "720236d913a58e70ae19eff350131ffb";
        private const string ExchangeRateApiUrl = "http://api.exchangeratesapi.io/v1/latest?access_key=720236d913a58e70ae19eff350131ffb&format=1";
        UserSettingsUtil userSetUtil = new UserSettingsUtil();

        [HttpPost]
        [Route("api/email")]
        public IHttpActionResult SendEmail(EmailModel email)
        {
            try
            {
                using (SmtpClient smtpClient = new SmtpClient("smtp.ethereal.email"))
                {
                    smtpClient.Credentials = new NetworkCredential("adan20@ethereal.email", "kfDRTArRaR7DwfnGte");
                    smtpClient.Port = 587; // Ethereal's SMTP port
                    smtpClient.EnableSsl = true;

                    using (MailMessage mail = new MailMessage())
                    {
                        mail.From = new MailAddress("adan20@ethereal.email");
                        mail.To.Add(email.To);
                        mail.Subject = email.Subject;
                        mail.Body = email.Body;

                        smtpClient.Send(mail);
                    }
                }
                return Ok("Email sent successfully!");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/exchangerates")]
        public IHttpActionResult GetExchangeRates()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("apikey", ApiKey);

                try
                {
                    var response = httpClient.GetAsync(ExchangeRateApiUrl).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = response.Content.ReadAsStringAsync().Result;

                        var exchangeRateResponse = JsonConvert.DeserializeObject<ExchangeRateResponse>(jsonResponse);
                        return Ok(exchangeRateResponse);
                    }

                    return BadRequest("Failed to fetch exchange rates.");
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
        }

        [HttpPost]
        [Route("api/employee/config-update")]
        public IHttpActionResult UpdateEmployeeConfiguration(UpdateEmployeeConfigurationModel model)
        {
            // Check if the provided EmployeeID exists in the Employees table
            if (!userSetUtil.EmployeeExists(model.EmployeeID))
            {
                return BadRequest("Invalid EmployeeID"); // Return a 400 Bad Request status if the EmployeeID is not found
            }

            // Check if the user has the SuperAdmin role based on the role sent from Angular
            if (model.Role != "SuperAdmin")
            {
                return BadRequest("Only SuperAdmins can update PreDefinedAssetID");
            }

            // Retrieve the ConfigurationID associated with the provided EmployeeID
            string configurationId = userSetUtil.GetConfigurationId(model.EmployeeID);

            if (string.IsNullOrEmpty(configurationId))
            {
                return BadRequest("ConfigurationID not found"); // Handle the case where ConfigurationID is not found
            }

            // Update the Employee_Configuration table with the new PreDefinedAssetID
            bool updateSuccessful = userSetUtil.UpdatePreDefinedAssetID(configurationId, model.PreDefinedAssetID);

            if (updateSuccessful)
            {
                return Ok("PreDefinedAssetID updated successfully");
            }
            else
            {
                return BadRequest("Failed to update PreDefinedAssetID");
            }
        }

        [HttpGet]
        [Route("api/employee/predefined-asset")]
        public IHttpActionResult GetPredefinedAssetID(string employeeId)
        {
            // Check if the provided EmployeeID exists in the Employees table
            if (!userSetUtil.EmployeeExists(employeeId))
            {
                return BadRequest("Invalid EmployeeID");
            }

            // Retrieve the ConfigurationID associated with the provided EmployeeID
            string configurationId = userSetUtil.GetConfigurationId(employeeId);

            if (string.IsNullOrEmpty(configurationId))
            {
                return BadRequest("ConfigurationID not found");
            }

            // Retrieve the PreDefinedAssetID from the Employee_Configuration table
            string predefinedAssetID = userSetUtil.GetPredefinedAssetID(configurationId);

            if (predefinedAssetID != null)
            {
                return Ok(predefinedAssetID);
            }
            else
            {
                return BadRequest("PreDefinedAssetID not found");
            }
        }

        [HttpGet]
        [Route("api/ad/domain")]
        public string GetDomainName()
        {
            try
            {
                using (DirectoryEntry root = new DirectoryEntry("LDAP://RootDSE"))
                {
                    string domainName = root.Properties["defaultNamingContext"].Value.ToString();
                    return domainName;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions, such as directory access issues.
                return "Error: " + ex.Message;
            }
        }

        [HttpPost]
        [Route("api/employee/uploadImage")]
        public IHttpActionResult UploadEmployeeImage()
        {
            HttpRequest request = HttpContext.Current.Request;

            string employeeID = request.Form["employeeID"];
            string role = request.Form["role"];
            HttpPostedFile image = request.Files["image"];

            if (string.IsNullOrEmpty(employeeID) || string.IsNullOrEmpty(role) || image == null || image.ContentLength == 0)
            {
                return BadRequest("Invalid input data");
            }

            // Check if the employeeID is valid
            if (!userSetUtil.EmployeeExists(employeeID))
            {
                return BadRequest("Invalid EmployeeID");
            }

            // Check if the user has the SuperAdmin role
            if (role != "SuperAdmin")
            {
                return BadRequest("Only SuperAdmins can upload images");
            }

            // Retrieve the ConfigurationID associated with the provided EmployeeID
            string configurationId = userSetUtil.GetConfigurationId(employeeID);

            if (string.IsNullOrEmpty(configurationId))
            {
                return BadRequest("ConfigurationID not found");
            }

            byte[] imageData;
            using (var binaryReader = new BinaryReader(image.InputStream))
            {
                imageData = binaryReader.ReadBytes(image.ContentLength);
            }

            bool uploadSuccessful = userSetUtil.UploadEmployeeImageToDatabase(configurationId, imageData);

            if (uploadSuccessful)
            {
                return Ok("Image uploaded successfully");
            }
            else
            {
                return BadRequest("Failed to upload image");
            }
        }

        [HttpGet]
        [Route("api/employee/getImage")]
        public IHttpActionResult GetEmployeeImage(string employeeID)
        {
            // Check if the employeeID is valid
            if (!userSetUtil.EmployeeExists(employeeID))
            {
                return BadRequest("Invalid EmployeeID");
            }

            // Retrieve the ConfigurationID associated with the provided EmployeeID
            string configurationId = userSetUtil.GetConfigurationId(employeeID);

            if (string.IsNullOrEmpty(configurationId))
            {
                return BadRequest("ConfigurationID not found");
            }

            // Retrieve the image data from the database based on the configurationId
            byte[] imageData = userSetUtil.GetEmployeeImageData(configurationId);

            if (imageData == null)
            {
                return BadRequest("Image not found");
            }

            //string base64Image = Convert.ToBase64String(imageData);

            string base64Image = Convert.ToBase64String(imageData);

            return Ok(new { image = base64Image });
        }

        [HttpPost]
        [Route("api/check-existing-users")]
        public IHttpActionResult CheckExistingUsers([FromBody] UserCheckModel userCheckModel)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (userSetUtil.EmailExistsInEmployees(userCheckModel.EEmail) &&
                        userSetUtil.UsernameExistsInEmployees(userCheckModel.UName))
                    {
                        //return Ok("User already exists");
                        return Ok(new { success = true });
                    }

                    return Ok(new { success = false });
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/Register")]
        public IHttpActionResult RegisterCompanyAndEmployee([FromBody] RegistrationData registrationData)
        {
            if (registrationData == null || registrationData.CompanyModel == null || registrationData.EmployeeModel == null)
            {
                return BadRequest("Invalid company or employee data");
            }

            CompanyModel companyModel = registrationData.CompanyModel;
            EmployeeModel employeeModel = registrationData.EmployeeModel;
            string userRole = registrationData.UserRole;

            if (userRole != "SuperAdmin")
            {
                return BadRequest("Only SuperAdmins can perform this action");
            }

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            //using (var commandCompany = new SqlCommand())
                            //{
                            //    commandCompany.Connection = connection;
                            //    commandCompany.Transaction = transaction;
                            //    commandCompany.CommandText = @"INSERT INTO Company (CompanyID, CompanyName, Address, PhoneNumber, Industry, SPOCInformation, EmailAccount)
                            //                           VALUES (@CompanyID, @CompanyName, @Address, @PhoneNumber, @Industry, @SPOCInformation, @EmailAccount)";
                                
                            //    commandCompany.Parameters.AddWithValue("@CompanyID", companyModel.CompanyID);
                            //    commandCompany.Parameters.AddWithValue("@CompanyName", companyModel.CompanyName);
                            //    commandCompany.Parameters.AddWithValue("@Address", companyModel.Address);
                            //    commandCompany.Parameters.AddWithValue("@PhoneNumber", companyModel.PhoneNumber);
                            //    commandCompany.Parameters.AddWithValue("@Industry", companyModel.Industry);
                            //    commandCompany.Parameters.AddWithValue("@SPOCInformation", companyModel.SPOCInformation);
                            //    commandCompany.Parameters.AddWithValue("@EmailAccount", companyModel.EmailAccount);
                            //    commandCompany.ExecuteNonQuery();
                            //}

                            // Generate ConfigurationID (assuming it's an integer)
                            //int configurationID;
                            //using (var commandGenerateConfigID = new SqlCommand("SELECT ISNULL(MAX(ConfigurationID), 100000) + 1 FROM Employees", connection, transaction))
                            //{
                            //    configurationID = Convert.ToInt32(commandGenerateConfigID.ExecuteScalar());
                            //}

                            // Insert into Employees table
                            using (var commandEmployee = new SqlCommand())
                            {
                                commandEmployee.Connection = connection;
                                commandEmployee.Transaction = transaction;
                                commandEmployee.CommandText = @"INSERT INTO Employees (EmployeeID, DomainAccount, EmployeeType, Username, FirstName, LastName, Email, Password, RegisterDate, Role, PhoneNumber, Location, ProjectName, Team, CustomerName, WorkType, ReportingManager, CompanyID)
                                                        OUTPUT INSERTED.ConfigurationID
                                                        VALUES (@EmployeeID, @DomainAccount, @EmployeeType, @Username, @FirstName, @LastName, @Email, @Password, @RegisterDate, @Role, @PhoneNumber, @Location, @ProjectName, @Team, @CustomerName, @WorkType, @ReportingManager, @CompanyID)";

                                // Add parameters to prevent SQL injection
                                commandEmployee.Parameters.AddWithValue("@EmployeeID", employeeModel.EmployeeID);
                                commandEmployee.Parameters.AddWithValue("@DomainAccount", employeeModel.DomainAccount);
                                commandEmployee.Parameters.AddWithValue("@EmployeeType", employeeModel.EmployeeType);
                                commandEmployee.Parameters.AddWithValue("@Username", employeeModel.Username);
                                commandEmployee.Parameters.AddWithValue("@FirstName", employeeModel.FirstName);
                                commandEmployee.Parameters.AddWithValue("@LastName", employeeModel.LastName);
                                commandEmployee.Parameters.AddWithValue("@Email", employeeModel.Email);
                                commandEmployee.Parameters.AddWithValue("@Password", employeeModel.Password);
                                commandEmployee.Parameters.AddWithValue("@RegisterDate", employeeModel.RegisterDate);
                                commandEmployee.Parameters.AddWithValue("@Role", employeeModel.Role);
                                commandEmployee.Parameters.AddWithValue("@PhoneNumber", employeeModel.PhoneNumber);
                                commandEmployee.Parameters.AddWithValue("@Location", employeeModel.Location);
                                commandEmployee.Parameters.AddWithValue("@ProjectName", employeeModel.ProjectName);
                                commandEmployee.Parameters.AddWithValue("@Team", employeeModel.Team);
                                commandEmployee.Parameters.AddWithValue("@CustomerName", employeeModel.CustomerName);
                                commandEmployee.Parameters.AddWithValue("@WorkType", employeeModel.WorkType);
                                commandEmployee.Parameters.AddWithValue("@ReportingManager", employeeModel.ReportingManager);

                                // Link the employee to the company by using the same CompanyID
                                commandEmployee.Parameters.AddWithValue("@CompanyID", companyModel.CompanyID);
                                int generatedConfigurationID = (int)commandEmployee.ExecuteScalar();

                                registrationData.EmployeeConfigModel = new EmployeeConfigModel();

                                registrationData.EmployeeConfigModel.ConfigurationID = generatedConfigurationID;
                            }

                            using (var commandEmployeeConfig = new SqlCommand())
                            {
                                commandEmployeeConfig.Connection = connection;
                                commandEmployeeConfig.Transaction = transaction;
                                commandEmployeeConfig.CommandText = @"INSERT INTO Employee_Configuration (ConfigurationID, PreDefinedAssetID, GSTNumber, Image)
                                                              VALUES (@ConfigurationID, @PreDefinedAssetID, @GSTNumber, @Image)";

                                // Add parameters to prevent SQL injection
                                commandEmployeeConfig.Parameters.AddWithValue("@ConfigurationID", registrationData.EmployeeConfigModel.ConfigurationID); 
                                commandEmployeeConfig.Parameters.AddWithValue("@PreDefinedAssetID", registrationData.EmployeeConfigModel.PreDefinedAssetID ?? (object)DBNull.Value);
                                commandEmployeeConfig.Parameters.AddWithValue("@GSTNumber", registrationData.EmployeeConfigModel.GSTNumber ?? (object)DBNull.Value);

                                SqlParameter imageParameter = new SqlParameter("@Image", SqlDbType.VarBinary);
                                imageParameter.Value = (object)registrationData.EmployeeConfigModel.Image ?? DBNull.Value;
                                commandEmployeeConfig.Parameters.Add(imageParameter);

                                // Execute the SQL command
                                commandEmployeeConfig.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return Ok("Company and employee registered successfully");
                        }
                        catch (Exception ex)
                        {
                            // Rollback the transaction if an exception occurs
                            transaction.Rollback();

                            // Log the exception or handle it as needed
                            return InternalServerError(ex);
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
        [Route("api/GetEmployeeDetails")]
        public IHttpActionResult GetEmployeeDetails()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlQuery = @"SELECT EmployeeID, Username, Email, Team, ReportingManager, Role 
                                FROM Employees
                                WHERE Role != 'EndUser'";

                    using (var command = new SqlCommand(sqlQuery, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            List<EmployeeDetailsModel> employeeDetailsList = new List<EmployeeDetailsModel>();

                            while (reader.Read())
                            {
                                EmployeeDetailsModel employeeDetails = new EmployeeDetailsModel
                                {
                                    EmployeeID = reader["EmployeeID"].ToString(),
                                    Username = reader["Username"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Team = reader["Team"].ToString(),
                                    ReportingManager = reader["ReportingManager"].ToString(),
                                    Role = reader["Role"].ToString()
                                };

                                employeeDetailsList.Add(employeeDetails);
                            }

                            return Ok(employeeDetailsList);
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
        [Route("api/GetEndUsers")]
        public IHttpActionResult GetEndUsers()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Query to retrieve specific fields from the Employees table for users with Role = 'EndUser'
                    string sqlQuery = @"SELECT EmployeeID, Username, Email, Team, ReportingManager, Role 
                                FROM Employees
                                WHERE Role = 'EndUser'";

                    using (var command = new SqlCommand(sqlQuery, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            List<EmployeeDetailsModel> endUserList = new List<EmployeeDetailsModel>();

                            while (reader.Read())
                            {
                                EmployeeDetailsModel endUserDetails = new EmployeeDetailsModel
                                {
                                    EmployeeID = reader["EmployeeID"].ToString(),
                                    Username = reader["Username"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Team = reader["Team"].ToString(),
                                    ReportingManager = reader["ReportingManager"].ToString(),
                                    Role = reader["Role"].ToString()
                                };

                                endUserList.Add(endUserDetails);
                            }

                            return Ok(endUserList);
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


        [HttpPut]
        [Route("api/employee/updateRole")]
        public IHttpActionResult UpdateEmployeeRole([FromBody] EmployeeUpdate model)
        {
            string userRole = model.UserRole;

            if (model == null || model.EmployeeID == null || string.IsNullOrEmpty(model.Role))
            {
                return BadRequest("Invalid data");
            }

            if (userRole != "SuperAdmin")
            {
                return BadRequest("Only SuperAdmins can perform this action");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string updateQuery = "UPDATE Employees SET Role = @Role WHERE EmployeeID = @EmployeeID";

                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Role", model.Role);
                    command.Parameters.AddWithValue("@EmployeeID", model.EmployeeID);

                    try
                    {
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok("Role updated successfully");
                        }
                        else
                        {
                            return BadRequest($"Employee with ID {model.EmployeeID} not found");
                        }
                    }
                    catch (Exception ex)
                    {
                        return InternalServerError(ex);
                    }
                }
            }
        }

     }
}
