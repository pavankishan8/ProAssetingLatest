using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace Test.Utils
{
    public class UserSettingsUtil
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["Test"].ConnectionString;

        public bool EmployeeExists(string employeeId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM Employees WHERE EmployeeID = @EmployeeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", employeeId);

                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }

        public string GetConfigurationId(string employeeId)
        {
            string configurationId = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT ConfigurationID FROM Employees WHERE EmployeeID = @EmployeeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", employeeId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            configurationId = reader["ConfigurationID"].ToString();
                        }
                    }
                }
            }

            return configurationId;
        }

        public bool UpdatePreDefinedAssetID(string configurationId, string newPreDefinedAssetID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE Employee_Configuration SET PreDefinedAssetID = @PreDefinedAssetID WHERE ConfigurationID = @ConfigurationID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PreDefinedAssetID", newPreDefinedAssetID);
                    command.Parameters.AddWithValue("@ConfigurationID", configurationId);

                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }
        
        public string GetPredefinedAssetID(string configurationId)
        {
            string predefinedAssetID = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT PreDefinedAssetID FROM Employee_Configuration WHERE ConfigurationID = @ConfigurationID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ConfigurationID", configurationId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            predefinedAssetID = reader["PreDefinedAssetID"].ToString();
                        }
                    }
                }
            }

            return predefinedAssetID;
        }
        
        public bool UploadEmployeeImageToDatabase(string configurationId, byte[] image)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE Employee_Configuration SET Image = @Image WHERE ConfigurationID = @ConfigurationID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Image", image);
                    command.Parameters.AddWithValue("@ConfigurationID", configurationId);

                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        public byte[] GetEmployeeImageData(string configurationId)
        {
            byte[] imageData = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT Image FROM Employee_Configuration WHERE ConfigurationID = @ConfigurationID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ConfigurationID", configurationId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("Image")))
                            {
                                imageData = (byte[])reader["Image"];
                            }
                        }
                    }
                }
            }

            return imageData;
        }

        public string InferImageContentType(byte[] imageData)
        {
            // Check the first few bytes of the image data to determine the format
            if (imageData.Length >= 2 && imageData[0] == 0xFF && imageData[1] == 0xD8)
            {
                return "image/jpeg";
            }
            else if (imageData.Length >= 8 && Encoding.ASCII.GetString(imageData, 0, 8).Contains("PNG"))
            {
                return "image/png";
            }
            // Add more checks for other image formats as needed

            // Default to "application/octet-stream" if the format is not recognized
            return "application/octet-stream";
        }

        public bool EmailExistsInCompany(string email)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open(); // Open the connection

                using (var command = new SqlCommand("SELECT COUNT(*) FROM Company WHERE EmailAccount = @Email", connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    return (int)command.ExecuteScalar() > 0;
                }
            }
        }

        public bool EmailExistsInEmployees(string email)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open(); // Open the connection

                using (var command = new SqlCommand("SELECT COUNT(*) FROM Employees WHERE Email = @Email", connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    return (int)command.ExecuteScalar() > 0;
                }
            }
        }

        public bool UsernameExistsInEmployees(string username)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open(); // Open the connection

                using (var command = new SqlCommand("SELECT COUNT(*) FROM Employees WHERE Username = @Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    return (int)command.ExecuteScalar() > 0;
                }
            }
        }
    }
}