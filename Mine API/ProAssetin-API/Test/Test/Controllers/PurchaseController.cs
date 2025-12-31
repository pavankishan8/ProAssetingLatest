using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using static Test.Models.PurchaseModel;

namespace Test.Controllers
{

    [EnableCors("*", "*", "*", "*")]
    public class PurchaseController : ApiController
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["Test"].ConnectionString;

        [HttpPost]
        [Route("api/purchaseorders")]
        public IHttpActionResult InsertPurchaseOrder([FromBody] PurchaseOrderModel model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest("Invalid request body.");
                }
                
                if (string.IsNullOrEmpty(model.CompanyName) ||
                    string.IsNullOrEmpty(model.CompanyAddress) ||
                    string.IsNullOrEmpty(model.PurchaseOrderNo) ||
                    model.OrderDate == default(DateTime) ||
                    model.DueDate == default(DateTime))
                {
                    return BadRequest("One or more required properties are missing or invalid.");
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"INSERT INTO PurchaseOrders 
                                (CompanyName, CompanyAddress, PurchaseOrderNo, OrderDate, DueDate, Notes, TermsAndConditions, TotalAmount, Items, CreatedDate) 
                                VALUES 
                                (@CompanyName, @CompanyAddress, @PurchaseOrderNo, @OrderDate, @DueDate, @Notes, @TermsAndConditions, @TotalAmount, @Items, @CreatedDate)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CompanyName", model.CompanyName);
                        command.Parameters.AddWithValue("@CompanyAddress", model.CompanyAddress);
                        command.Parameters.AddWithValue("@PurchaseOrderNo", model.PurchaseOrderNo);
                        command.Parameters.AddWithValue("@OrderDate", model.OrderDate);
                        command.Parameters.AddWithValue("@DueDate", model.DueDate);
                        command.Parameters.AddWithValue("@Notes", model.Notes);
                        command.Parameters.AddWithValue("@TermsAndConditions", model.TermsAndConditions);
                        command.Parameters.AddWithValue("@TotalAmount", model.TotalAmount ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Items", model.Items);
                        command.Parameters.AddWithValue("@CreatedDate", model.CreatedDate);

                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Purchase order inserted successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
