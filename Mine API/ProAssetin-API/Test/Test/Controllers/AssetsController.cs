using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Test.Models;
using Test.Utils;
using static Test.Models.AssetManagementModel;

namespace Test.Controllers
{
    [EnableCors("*", "*", "*", "*")]
    public class AssetsController : ApiController
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["Test"].ConnectionString;
        //private int nextAssetID = 10000001;
        AssetManagementUtil assetUtil = new AssetManagementUtil();

        //[HttpPost]
        //[Route("api/login")]
        //public IHttpActionResult LoginUser(LoginModel model)
        //{
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            connection.Open();

        //            // Define the SQL query to retrieve user information based on email
        //            string sql = "SELECT E.EmployeeID, E.DomainAccount, E.EmployeeType, E.Username, E.FirstName, E.LastName, E.Email, E.Password, " +
        //            "E.RegisterDate, E.LastLoginDate, E.Role, E.PhoneNumber, E.Location, E.ProjectName, E.Team, E.CustomerName, " +
        //            "E.WorkType, E.ReportingManager, C.CompanyID, C.CompanyName, C.Address AS CompanyAddress, C.PhoneNumber AS CompanyPhoneNumber, C.Industry " +
        //            "FROM Employees E " +
        //            "LEFT JOIN Company C ON E.CompanyID = C.CompanyID " +
        //            "WHERE E.Email = @Email";

        //            using (SqlCommand command = new SqlCommand(sql, connection))
        //            {
        //                command.Parameters.AddWithValue("@Email", model.Email);

        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    if (reader.Read())
        //                    {
        //                        string storedPlainTextPassword = reader.GetString(7); // Assumes plain text password
        //                        if (model.Password == storedPlainTextPassword)
        //                        {
        //                            // Passwords match, user is authenticated

        //                            // Update LastLoginDate in the database
        //                            //DateTime loginTime = DateTime.Now;
        //                            //string employeeId = reader.GetString(0);

        //                            var userData = new
        //                            {
        //                                EmployeeID = reader.GetString(0), // Retrieve EmployeeID from Employees table as a string
        //                                DomainAccount = reader.GetString(1),
        //                                EmployeeType = reader.GetString(2),
        //                                CompanyID = reader.GetString(18), // Retrieve CompanyID from Employees table as a string
        //                                Username = reader.GetString(3), // Retrieve Username as a string
        //                                FirstName = reader.GetString(4), // Retrieve FirstName as a string
        //                                LastName = reader.GetString(5), // Retrieve LastName as a strin
        //                                Email = reader.GetString(6), // Retrieve Email as a string
        //                                Role = reader.GetString(10), // Retrieve Role as a string
        //                                PhoneNumber = reader.GetString(11), // Retrieve PhoneNumber as a string
        //                                Location = reader.GetString(12), // Retrieve Location as a string
        //                                ProjectName = reader.GetString(13), // Retrieve ProjectName as a string
        //                                Team = reader.GetString(14), // Retrieve Team as a string
        //                                CustomerName = reader.GetString(15), // Retrieve CustomerName as a string
        //                                WorkType = reader.GetString(16), // Retrieve WorkType as a string
        //                                ReportingManager = reader.GetString(17), // Retrieve ReportingManager as a string
        //                                CompanyName = reader.GetString(19), // Retrieve CompanyName from Company table as a string
        //                                CompanyAddress = reader.GetString(20), // Retrieve CompanyAddress from Company table as a string
        //                                CompanyPhoneNumber = reader.GetString(21), // Retrieve CompanyPhoneNumber from Company table as a string
        //                                Industry = reader.GetString(22) // Retrieve Industry from Company table as a string
        //                            };

        //                            reader.Close();

        //                            DateTime loginTime = DateTime.Now;
        //                            string employeeId = userData.EmployeeID;

        //                            using (SqlConnection updateConnection = new SqlConnection(connectionString))
        //                            {
        //                                updateConnection.Open();
        //                                string updateLastLoginSql = "UPDATE Employees SET LastLoginDate = @LoginTime WHERE EmployeeID = @EmployeeID";
        //                                using (SqlCommand updateCommand = new SqlCommand(updateLastLoginSql, updateConnection))
        //                                {
        //                                    updateCommand.Parameters.AddWithValue("@LoginTime", loginTime);
        //                                    updateCommand.Parameters.AddWithValue("@EmployeeID", employeeId);

        //                                    // Execute the update command
        //                                    updateCommand.ExecuteNonQuery();
        //                                }
        //                            }

        //                            // Return the success status and user data to the client
        //                            return Ok(new { success = true, user = userData });
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception
        //        // For now, just returning a simple error message
        //        return InternalServerError(ex);
        //    }

        //    // Authentication failed
        //    return Ok(new { success = false });
        //}

        [HttpGet]
        [Route("api/asset-counts")]
        public IHttpActionResult GetAssetCounts()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT " +
                                   "(SELECT COUNT(*) FROM Assets WHERE Status = 'Active') AS ActiveCount, " +
                                   "(SELECT COUNT(*) FROM Assets WHERE Status = 'In-Stock') AS InStockCount, " +
                                   "(SELECT COUNT(*) FROM Assets WHERE Status = 'Repair') AS RepairCount, " +
                                   "(SELECT COUNT(*) FROM Assets WHERE Status = 'Damaged') AS DamagedCount, " +
                                   "(SELECT COUNT(*) FROM Assets WHERE Status = 'Sold') AS SoldCount, " +
                                   "(SELECT COUNT(*) FROM Assets WHERE Status = 'E-Waste') AS EWasteCount, " +
                                   "(SELECT COUNT(*) FROM Assets WHERE Status = 'Allocated') AS AllocatedCount";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var counts = new
                                {
                                    Active = (int)reader["ActiveCount"],
                                    InStock = (int)reader["InStockCount"],
                                    Repair = (int)reader["RepairCount"],
                                    Damaged = (int)reader["DamagedCount"],
                                    Sold = (int)reader["SoldCount"],
                                    EWaste = (int)reader["EWasteCount"],
                                    Allocated = (int)reader["AllocatedCount"]
                                };

                                return Ok(counts);
                            }
                        }
                    }
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/inventory/{keyword}")]
        public IHttpActionResult GetInventoryData(string keyword)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql;
                    List<InStockItem> inStockDataList = new List<InStockItem>();
                    List<RepairItem> repairDataList = new List<RepairItem>();
                    List<SoldItem> soldDataList = new List<SoldItem>();
                    List<DamagedItem> damagedDataList = new List<DamagedItem>();
                    List<EWasteItem> ewasteDataList = new List<EWasteItem>();
                    List<AllocateAssetRequest> allocatedDataList = new List<AllocateAssetRequest>();

                    switch (keyword)
                    {
                        case "In-Stock":
                            sql = "SELECT AssetID, AssetType, Make, Model, Status, RAM, Processor, OS, Serial_Number, IMEI_Number " +
                                  "FROM InStock";
                            break;
                        case "Repair":
                            sql = "SELECT AssetID, AssetType, Make, Model, Status, Vendor, SentDate, ReceiveDate, Repair_Cost, RepairStatus, Tracking, RepairNotes, DeliveredBy " +
                                  "FROM Repair";
                            break;
                        case "Sold":
                            sql = "SELECT AssetID, AssetType, Make, Processor, Status, Serial_Number, Purchase_Cost, Sold_Cost, Purchase_Year, Sold_Year, SoldNotes, SoldTo, Approvals " +
                                  "FROM Sold";
                            break;
                        case "Damaged":
                            sql = "SELECT AssetID, AssetType, Make, Status, Serial_Number, Purchase_Cost, Purchase_Year, DamagedNotes " +
                                  "FROM Damaged";
                            break;
                        case "E-Waste":
                            sql = "SELECT AssetID, AssetType, Make, Processor, Status, Serial_Number, Purchase_Year, EWaste_Vendor, EWasteNotes, EWasteApprovals " +
                                  "FROM EWaste";
                            break;
                        case "Allocated":
                            sql = "SELECT AssetID, AssetType, Make, Processor, RAM, Status, Serial_Number, AssignedToUserID, Email, Username, PhoneNumber, Location, ReportingManager " +
                                  "FROM Allocated";
                            break;
                        default:
                            // Invalid keyword, return an error
                            return BadRequest("Invalid keyword");
                    }

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (keyword == "In-Stock")
                                {
                                    InStockItem inStockData = new InStockItem
                                    {
                                        AssetID = reader.GetString(0),
                                        AssetType = reader.GetString(1),
                                        Make = reader.GetString(2),
                                        Model = reader.GetString(3),
                                        Status = reader.GetString(4),
                                        RAM = reader.GetString(5),
                                        Processor = reader.GetString(6),
                                        OS = reader.GetString(7),
                                        Serial_Number = reader.GetString(8),
                                        IMEI_Number = reader.GetString(9),
                                    };

                                    inStockDataList.Add(inStockData);
                                }
                                else if (keyword == "Repair")
                                {
                                    RepairItem repairItem = new RepairItem
                                    {
                                        AssetID = reader.GetString(0),
                                        AssetType = reader.GetString(1),
                                        Make = reader.GetString(2),
                                        Model = reader.GetString(3),
                                        Status = reader.GetString(4),
                                        Vendor = reader.GetString(5),
                                        SentDate = reader.GetDateTime(6),
                                        ReceiveDate = reader.GetDateTime(7),
                                        Repair_Cost = reader.GetString(8),
                                        RepairStatus = reader.GetString(9),
                                        Tracking = reader.GetString(10),
                                        RepairNotes = reader.GetString(11),
                                        DeliveredBy = reader.GetString(12)

                                    };

                                    repairDataList.Add(repairItem);
                                }
                                else if (keyword == "Sold")
                                {
                                    SoldItem soldItem = new SoldItem
                                    {
                                        AssetID = reader.GetString(0),
                                        AssetType = reader.GetString(1),
                                        Make = reader.GetString(2),
                                        Processor = reader.GetString(3),
                                        Status = reader.GetString(4),
                                        Serial_Number = reader.GetString(5),
                                        Purchase_Cost = reader.GetString(6),
                                        Sold_Cost = reader.GetString(7),
                                        Purchase_Year = reader.GetDateTime(8),
                                        Sold_Year = reader.GetDateTime(9),
                                        SoldNotes = reader.GetString(10),
                                        SoldTo = reader.GetString(11),
                                        Approvals = reader.GetString(12)

                                    };

                                    soldDataList.Add(soldItem);
                                }
                                else if (keyword == "Damaged")
                                {
                                    DamagedItem damagedItem = new DamagedItem
                                    {
                                        AssetID = reader.GetString(0),
                                        AssetType = reader.GetString(1),
                                        Make = reader.GetString(2),
                                        Status = reader.GetString(3),
                                        Serial_Number = reader.GetString(4),
                                        Purchase_Cost = reader.GetString(5),
                                        Purchase_Year = reader.GetDateTime(6),
                                        DamagedNotes = reader.GetString(7)
                                    };

                                    damagedDataList.Add(damagedItem);
                                }
                                else if (keyword == "E-Waste")
                                {
                                    EWasteItem ewasteItem = new EWasteItem
                                    {
                                        AssetID = reader.GetString(0),
                                        AssetType = reader.GetString(1),
                                        Make = reader.GetString(2),
                                        Processor = reader.GetString(3),
                                        Status = reader.GetString(4),
                                        Serial_Number = reader.GetString(5),
                                        Purchase_Year = reader.GetDateTime(6),
                                        EWaste_Vendor = reader.GetString(7),
                                        EWasteNotes = reader.GetString(8),
                                        EWasteApprovals = reader.GetString(9)
                                    };

                                    ewasteDataList.Add(ewasteItem);
                                }
                                else if (keyword == "Allocated")
                                {
                                    AllocateAssetRequest allocatedItem = new AllocateAssetRequest
                                    {
                                        AssetID = reader.GetString(0),
                                        AssetType = reader.GetString(1),
                                        Make = reader.GetString(2),
                                        Processor = reader.GetString(3),
                                        RAM = reader.GetString(4),
                                        Status = reader.GetString(5),
                                        Serial_Number = reader.GetString(6),
                                        AssignedToUserID = reader.GetString(7),
                                        Email = reader.GetString(8),
                                        Username = reader.GetString(9),
                                        PhoneNumber = reader.GetString(10),
                                        Location = reader.GetString(11),
                                        ReportingManager = reader.GetString(12)
                                    };

                                    allocatedDataList.Add(allocatedItem);
                                }
                            }
                        }

                        if (keyword == "In-Stock")
                        {
                            // Return the list of In-Stock data to the client
                            return Ok(new { success = true, data = inStockDataList });
                        }
                        else if (keyword == "Repair")
                        {
                            // Return the list of Repair data to the client
                            return Ok(new { success = true, data = repairDataList });
                        }
                        else if (keyword == "Sold")
                        {
                            return Ok(new { success = true, data = soldDataList });
                        }
                        else if (keyword == "Damaged")
                        {
                            return Ok(new { success = true, data = damagedDataList });
                        }
                        else if (keyword == "E-Waste")
                        {
                            return Ok(new { success = true, data = ewasteDataList });
                        }
                        else if (keyword == "Allocated")
                        {
                            return Ok(new { success = true, data = allocatedDataList });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                // For now, just return a simple error message
                return InternalServerError(ex);
            }

            // Data retrieval failed
            return Ok(new { success = false });
        }

        [HttpPost]
        [Route("api/assets/add")]
        public IHttpActionResult AddAsset(AssetList assetList, string userId)
        {
            if (assetList == null || assetList.Assets == null || assetList.Assets.Count == 0)
            {
                // Handle null or empty input gracefully
                return BadRequest("Invalid input data.");
            }

            string prefix = GetCustomAssetPrefixForUser(userId);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int nextAssetID = assetUtil.FindNextAssetID(connection, transaction);

                        foreach (Asset asset in assetList.Assets)
                        {
                            //string customAssetID = null;
                            string customAssetID = prefix + nextAssetID.ToString();

                            bool isUnique = false;

                            while (!isUnique)
                            {
                                //customAssetID = "ABC" + nextAssetID.ToString();

                                // Check if the generated AssetID already exists
                                if (!assetUtil.AssetIDExists(connection, transaction, customAssetID))
                                {
                                    isUnique = true;
                                }
                                else
                                {
                                    // If it's not unique, increment the nextAssetID and try again
                                    nextAssetID++;
                                    customAssetID = prefix + nextAssetID.ToString();
                                }
                            }

                            assetUtil.InsertAssetIntoDatabase(connection, transaction, asset, customAssetID);
                            // Increment the next asset ID for the next record

                            switch (asset.Status)
                            {
                                case "In-Stock":
                                    assetUtil.InsertIntoInStockTable(connection, transaction, asset, customAssetID);
                                    break;
                                case "Repair":
                                    assetUtil.InsertIntoRepairTable(connection, transaction, asset, customAssetID);
                                    break;
                                case "Damaged":
                                    assetUtil.InsertIntoDamagedTable(connection, transaction, asset, customAssetID);
                                    break;
                                case "Sold":
                                    assetUtil.InsertIntoSoldTable(connection, transaction, asset, customAssetID);
                                    break;
                                case "E-Waste":
                                    assetUtil.InsertIntoEWasteTable(connection, transaction, asset, customAssetID);
                                    break;
                            }
                        }

                        // Commit the transaction
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions and roll back the transaction if necessary
                        transaction.Rollback();
                        // Log the exception or take appropriate action.
                        return InternalServerError(ex);
                    }
                }
            }

            return Ok("Assets added successfully.");
        }

        private string GetCustomAssetPrefixForUser(string userId)
        {
            string customPrefix = "";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Use SQL to fetch the user's customized prefix from Employee_Configuration
                string query = "SELECT PreDefinedAssetID FROM Employee_Configuration WHERE ConfigurationID = (SELECT ConfigurationID FROM Employees WHERE EmployeeID = @UserId)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            customPrefix = reader["PreDefinedAssetID"].ToString();
                        }
                    }
                }
            }

            return customPrefix;
        }

        [HttpPost]
        [Route("api/assets/import")]
        public IHttpActionResult ImportAssetsFromExcel(string userId)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (!Request.Content.IsMimeMultipartContent())
            {
                return BadRequest("Request is not a valid multipart content.");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Get the last assetID in the table
                        string lastAssetID = assetUtil.GetLastAssetID(connection, transaction);
                        int lastAssetIDNumeric = int.Parse(lastAssetID.Substring(3)); // Assuming "ABC" prefix

                        string customPrefix = GetCustomAssetPrefixForUser(userId);

                        // Read data from the Excel file
                        List<Asset> assetsFromExcel = ReadAssetsFromExcel();

                        foreach (Asset asset in assetsFromExcel)
                        {
                            string customAssetID;
                            do
                            {
                                // Generate customAssetID based on the lastAssetIDNumeric
                                customAssetID = customPrefix + (lastAssetIDNumeric + 1).ToString();
                                lastAssetIDNumeric++;
                            }
                            while (assetUtil.AssetIDExists(connection, transaction, customAssetID));

                            // Insert the asset into the database
                            assetUtil.InsertAssetIntoDatabase(connection, transaction, asset, customAssetID);

                            switch (asset.Status)
                            {
                                case "In-Stock":
                                    assetUtil.InsertIntoInStockTable(connection, transaction, asset, customAssetID);
                                    break;
                                case "Repair":
                                    assetUtil.InsertIntoRepairTable(connection, transaction, asset, customAssetID);
                                    break;
                                case "Damaged":
                                    assetUtil.InsertIntoDamagedTable(connection, transaction, asset, customAssetID);
                                    break;
                                case "Sold":
                                    assetUtil.InsertIntoSoldTable(connection, transaction, asset, customAssetID);
                                    break;
                                case "E-Waste":
                                    assetUtil.InsertIntoEWasteTable(connection, transaction, asset, customAssetID);
                                    break;
                            }
                        }

                        // Commit the transaction
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions and roll back the transaction if necessary
                        transaction.Rollback();
                        // Log the exception or take appropriate action.
                        return InternalServerError(ex);
                    }
                }
            }

            return Ok("Assets imported successfully.");
        }

        public List<Asset> ReadAssetsFromExcel()
        {
            List<Asset> assets = new List<Asset>();

            using (var package = new ExcelPackage(Request.Content.ReadAsStreamAsync().Result))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    DateTime? sentDate = null;
                    DateTime? recDate = null;
                    decimal repairCost = 0;
                    decimal soldCost = 0;
                    DateTime? soldDate = null;

                    string sentDateText = worksheet.Cells[row, 16].Text;
                    string recDateText = worksheet.Cells[row, 17].Text;
                    string repairCostText = worksheet.Cells[row, 18].Text;
                    string soldCostText = worksheet.Cells[row, 23].Text;
                    string soldDateText = worksheet.Cells[row, 24].Text;

                    var purchaseYearNumeric = double.Parse(worksheet.Cells[row, 11].Text);
                    var recycleDateNumeric = double.Parse(worksheet.Cells[row, 13].Text);

                    if (!string.IsNullOrEmpty(sentDateText) && double.TryParse(sentDateText, out var sentDateValue))
                    {
                        sentDate = DateTime.FromOADate(sentDateValue);
                    }

                    if (!string.IsNullOrEmpty(recDateText) && double.TryParse(recDateText, out var recDateValue))
                    {
                        recDate = DateTime.FromOADate(recDateValue);
                    }

                    if (!string.IsNullOrEmpty(repairCostText) && decimal.TryParse(repairCostText, out var repairCostValue))
                    {
                        repairCost = repairCostValue;
                    }

                    if (!string.IsNullOrEmpty(soldCostText) && decimal.TryParse(soldCostText, out var soldCostValue))
                    {
                        soldCost = soldCostValue;
                    }

                    if (!string.IsNullOrEmpty(soldDateText) && double.TryParse(soldDateText, out var soldDateValue))
                    {
                        soldDate = DateTime.FromOADate(soldDateValue);
                    }

                    var purchaseYear = DateTime.FromOADate(purchaseYearNumeric);
                    var recycleDate = DateTime.FromOADate(recycleDateNumeric);
                    //var sentDate = DateTime.FromOADate(sentDateNumeric);
                    //var recDate = DateTime.FromOADate(recDateNumeric);

                    var asset = new Asset
                    {
                        AssetType = worksheet.Cells[row, 1].Text,
                        Make = worksheet.Cells[row, 2].Text,
                        Model = worksheet.Cells[row, 3].Text,
                        Status = worksheet.Cells[row, 4].Text,
                        RAM = worksheet.Cells[row, 5].Text,
                        Processor = worksheet.Cells[row, 6].Text,
                        OS = worksheet.Cells[row, 7].Text,
                        Serial_Number = worksheet.Cells[row, 8].Text,
                        IMEI_Number = worksheet.Cells[row, 9].Text,
                        Purchase_Cost = Convert.ToDecimal(worksheet.Cells[row, 10].Text),
                        Purchase_Year = purchaseYear,
                        MonthsInUse = worksheet.Cells[row, 12].Text,
                        NextRecycleDate = recycleDate,
                        AssignedToUserID = worksheet.Cells[row, 14].Text,
                        Vendor = worksheet.Cells[row, 15].Text,
                        SentDate = sentDate,
                        ReceiveDate = recDate,
                        Repair_Cost = repairCost,
                        RepairStatus = worksheet.Cells[row, 19].Text,
                        Tracking = worksheet.Cells[row, 20].Text,
                        RepairNotes = worksheet.Cells[row, 21].Text,
                        DeliveredBy = worksheet.Cells[row, 22].Text,
                        Sold_Cost = soldCost,
                        Sold_Year = soldDate,
                        SoldNotes = worksheet.Cells[row, 25].Text,
                        SoldTo = worksheet.Cells[row, 26].Text,
                        Approvals = worksheet.Cells[row, 27].Text,
                        DamagedNotes = worksheet.Cells[row, 28].Text,
                        EWaste_Vendor = worksheet.Cells[row, 29].Text,
                        EWasteNotes = worksheet.Cells[row, 30].Text,
                        EWasteApprovals = worksheet.Cells[row, 31].Text,

                    };

                    assets.Add(asset);
                }
            }

            return assets;
        }

        [HttpDelete]
        [Route("api/assets/delete")]
        public IHttpActionResult DeleteAsset(string assetID)
        {
            // Establish a database connection.
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Begin a database transaction.
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Check if the asset exists in any related tables (InStock, Repair, Damaged, Sold, E-Waste).
                        if (assetUtil.AssetExistsInRelatedTables(connection, transaction, assetID))
                        {
                            // If found, delete the related records first.
                            assetUtil.DeleteRelatedRecords(connection, transaction, assetID);
                        }

                        // Delete the asset from the Assets table.
                        using (SqlCommand command = new SqlCommand(
                            "DELETE FROM Assets WHERE AssetID = @AssetID",
                            connection, transaction))
                        {
                            command.Parameters.AddWithValue("@AssetID", assetID);
                            command.ExecuteNonQuery();
                        }

                        // Commit the transaction if everything was successful.
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions and roll back the transaction if necessary.
                        transaction.Rollback();
                        // Log the exception or take appropriate action.
                        return InternalServerError(ex);
                    }
                }
            }

            // Return a success message.
            return Ok("Asset and related records deleted successfully.");
        }

        [HttpGet]
        [Route("api/asset/search")]
        public IHttpActionResult SearchAssetById(string assetId)
        {
            try
            {
                Asset asset = null;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Define a list of tables to search in order of priority
                    string[] tablesToSearch = { "Assets", "InStock", "Damaged", "Sold", "Repair", "EWaste", "Allocated" };

                    foreach (string tableName in tablesToSearch)
                    {
                        // Build a dynamic SQL query based on the table name
                        string sql = $"SELECT * FROM {tableName} WHERE AssetID = @AssetID";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@AssetID", assetId);

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    if (asset == null)
                                    {
                                        asset = new Asset();
                                    }


                                    asset = assetUtil.MapAssetFromReader(reader, tableName, asset);
                                }
                            }
                        }
                    }

                    if (asset != null)
                    {
                        return Ok(asset);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            return BadRequest("Failed to search for the asset.");
        }

        [HttpGet]
        [Route("api/employee/search")]
        public IHttpActionResult SearchEmployeeById(string employeeId)
        {
            try
            {
                if (string.IsNullOrEmpty(employeeId))
                {
                    return BadRequest("EmployeeID cannot be empty.");
                }

                EmployeeDetails employee = null;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Assuming "Employees" is your table name
                    string sql = "SELECT * FROM Employees WHERE EmployeeID = @EmployeeID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeID", employeeId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                employee = new EmployeeDetails
                                {
                                    EmployeeID = reader["EmployeeID"].ToString(),
                                    DomainAccount = reader["DomainAccount"].ToString(),
                                    EmployeeType = reader["EmployeeType"].ToString(),
                                    Username = reader["Username"].ToString(),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    //RegisterDate = reader["RegisterDate"] != DBNull.Value ? (DateTime?)reader["RegisterDate"] : null,
                                    LastLoginDate = reader["LastLoginDate"].ToString(),
                                    Role = reader["Role"].ToString(),
                                    PhoneNumber = reader["PhoneNumber"].ToString(),
                                    Location = reader["Location"].ToString(),
                                    ProjectName = reader["ProjectName"].ToString(),
                                    Team = reader["Team"].ToString(),
                                    WorkType = reader["WorkType"].ToString(),
                                    ReportingManager = reader["ReportingManager"].ToString(),
                                    CompanyID = reader["CompanyID"].ToString(),
                                    ConfigurationID = reader["ConfigurationID"].ToString(),
                                };
                            }
                        }
                    }
                }

                if (employee != null)
                {
                    return Ok(employee);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/allocate-asset")]
        public HttpResponseMessage AllocateAsset([FromBody] AllocateAssetRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.AssetID) ||
                    string.IsNullOrEmpty(request.AssetType) ||
                    string.IsNullOrEmpty(request.Make) ||
                    string.IsNullOrEmpty(request.Processor) ||
                    string.IsNullOrEmpty(request.RAM) ||
                    string.IsNullOrEmpty(request.Status) ||
                    string.IsNullOrEmpty(request.Serial_Number) ||
                    string.IsNullOrEmpty(request.AssignedToUserID) ||
                    string.IsNullOrEmpty(request.Email) ||
                    string.IsNullOrEmpty(request.Username) ||
                    string.IsNullOrEmpty(request.PhoneNumber) ||
                    string.IsNullOrEmpty(request.Location) ||
                    string.IsNullOrEmpty(request.ReportingManager))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid request parameters.");
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Move the asset from InStock to Allocated
                    string moveSql = "INSERT INTO Allocated (AssetID, AssetType, Make, Processor, RAM, Status, Serial_Number, " +
                                     "AssignedToUserID, Email, Username, PhoneNumber, Location, ReportingManager) " +
                                     "SELECT AssetID, AssetType, Make, Processor, RAM, @Status AS Status, Serial_Number, " +
                                     "@AssignedToUserID AS AssignedToUserID, @Email AS Email, @Username AS Username, " +
                                     "@PhoneNumber AS PhoneNumber, @Location AS Location, @ReportingManager AS ReportingManager " +
                                     "FROM InStock WHERE AssetID = @AssetID";

                    using (SqlCommand moveCommand = new SqlCommand(moveSql, connection))
                    {
                        moveCommand.Parameters.AddWithValue("@Status", "Allocated");
                        moveCommand.Parameters.AddWithValue("@AssetID", request.AssetID);
                        moveCommand.Parameters.AddWithValue("@AssignedToUserID", request.AssignedToUserID);
                        moveCommand.Parameters.AddWithValue("@Email", request.Email);
                        moveCommand.Parameters.AddWithValue("@Username", request.Username);
                        moveCommand.Parameters.AddWithValue("@PhoneNumber", request.PhoneNumber);
                        moveCommand.Parameters.AddWithValue("@Location", request.Location);
                        moveCommand.Parameters.AddWithValue("@ReportingManager", request.ReportingManager);

                        int rowsAffected = moveCommand.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Asset not found in InStock table.");
                        }
                    }

                    // Delete the asset from InStock Table
                    string deleteSql = "DELETE FROM InStock WHERE AssetID = @AssetID";

                    using (SqlCommand deleteCommand = new SqlCommand(deleteSql, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@AssetID", request.AssetID);
                        deleteCommand.ExecuteNonQuery();
                    }

                    // Update the status in Assets Table
                    string updateStatusSql = "UPDATE Assets SET Status = @NewStatus, AssignedToUserID = @AssignedToUserID WHERE AssetID = @AssetID";

                    using (SqlCommand updateStatusCommand = new SqlCommand(updateStatusSql, connection))
                    {
                        updateStatusCommand.Parameters.AddWithValue("@NewStatus", "Allocated");
                        updateStatusCommand.Parameters.AddWithValue("@AssetID", request.AssetID);
                        updateStatusCommand.Parameters.AddWithValue("@AssignedToUserID", request.AssignedToUserID);

                        int rowsAffected = updateStatusCommand.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Asset not found in Assets table.");
                        }
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, "Asset allocated successfully.");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/searchByEmpID")]
        public IHttpActionResult GetAssetsByEmployeeId(string employeeId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT " +
                                   "AssetID, AssetType, Make, Processor, RAM, Status, Serial_Number, " +
                                   "AssignedToUserID, Email, Username, PhoneNumber, Location, ReportingManager " +
                                   "FROM Assets " +
                                   "JOIN Employees ON Assets.AssignedToUserID = Employees.EmployeeID " +
                                   "WHERE Employees.EmployeeID = @EmployeeID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeID", employeeId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                List<Asset> assets = new List<Asset>();

                                while (reader.Read())
                                {
                                    Asset asset = new Asset
                                    {
                                        AssetID = reader.GetString(reader.GetOrdinal("AssetID")),
                                        AssetType = reader.GetString(reader.GetOrdinal("AssetType")),
                                        Make = reader.GetString(reader.GetOrdinal("Make")),
                                        Processor = reader.GetString(reader.GetOrdinal("Processor")),
                                        RAM = reader.GetString(reader.GetOrdinal("RAM")),
                                        Status = reader.GetString(reader.GetOrdinal("Status")),
                                        Serial_Number = reader.GetString(reader.GetOrdinal("Serial_Number")),
                                        AssignedToUserID = reader.GetString(reader.GetOrdinal("AssignedToUserID")),
                                        Email = reader.GetString(reader.GetOrdinal("Email")),
                                        Username = reader.GetString(reader.GetOrdinal("Username")),
                                        PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                        Location = reader.GetString(reader.GetOrdinal("Location")),
                                        ReportingManager = reader.GetString(reader.GetOrdinal("ReportingManager")),
                                        expanded = false,
                                    };

                                    assets.Add(asset);
                                }

                                return Ok(assets);
                            }
                            else
                            {
                                return NotFound();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }

    }
}
