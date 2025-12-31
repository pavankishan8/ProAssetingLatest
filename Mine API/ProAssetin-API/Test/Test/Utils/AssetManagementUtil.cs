using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using System.Configuration;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static Test.Models.AssetManagementModel;
using System.Text.RegularExpressions;
using System.Data;

namespace Test.Utils
{
    public class AssetManagementUtil
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["Test"].ConnectionString;

        public bool AssetIDExists(SqlConnection connection, SqlTransaction transaction, string assetID)
        {
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Assets WHERE AssetID = @AssetID", connection, transaction))
            {
                command.Parameters.AddWithValue("@AssetID", assetID);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        public void InsertIntoInStockTable(SqlConnection connection, SqlTransaction transaction, Asset asset, string customAssetID)
        {
            using (SqlCommand inStockCommand = new SqlCommand(
                                        "INSERT INTO InStock (AssetID, AssetType, Make, Status, Model, RAM, Processor, OS, Serial_Number, IMEI_Number) " +
                                        "VALUES (@AssetID, @AssetType, @Make, @Status, @Model, @RAM, @Processor, @OS, @Serial_Number, @IMEI_Number)",
                                        connection, transaction))
            {
                inStockCommand.Parameters.AddWithValue("@AssetID", customAssetID);
                inStockCommand.Parameters.AddWithValue("@AssetType", asset.AssetType);
                inStockCommand.Parameters.AddWithValue("@Make", asset.Make);
                inStockCommand.Parameters.AddWithValue("@Status", asset.Status);
                inStockCommand.Parameters.AddWithValue("@Model", asset.Model);
                inStockCommand.Parameters.AddWithValue("@RAM", asset.RAM);
                inStockCommand.Parameters.AddWithValue("@Processor", asset.Processor);
                inStockCommand.Parameters.AddWithValue("@OS", asset.OS);
                inStockCommand.Parameters.AddWithValue("@Serial_Number", asset.Serial_Number);
                inStockCommand.Parameters.AddWithValue("@IMEI_Number", asset.IMEI_Number);
                inStockCommand.ExecuteNonQuery();
            }
        }

        public void InsertIntoRepairTable(SqlConnection connection, SqlTransaction transaction, Asset asset, string customAssetID)
        {
            using (SqlCommand repairCommand = new SqlCommand(
                                        "INSERT INTO Repair (AssetID, AssetType, Make, Status, Model, Vendor, SentDate, ReceiveDate, Repair_Cost, RepairStatus, Tracking, RepairNotes, DeliveredBy) " +
                                        "VALUES (@AssetID, @AssetType, @Make, @Status, @Model, @Vendor, @SentDate, @ReceiveDate, @Repair_Cost, @RepairStatus, @Tracking, @RepairNotes, @DeliveredBy)",
                                        connection, transaction))
            {
                repairCommand.Parameters.AddWithValue("@AssetID", customAssetID);
                repairCommand.Parameters.AddWithValue("@AssetType", asset.AssetType);
                repairCommand.Parameters.AddWithValue("@Make", asset.Make);
                repairCommand.Parameters.AddWithValue("@Status", asset.Status);
                repairCommand.Parameters.AddWithValue("@Model", asset.Model);
                repairCommand.Parameters.AddWithValue("@Vendor", asset.Vendor);
                repairCommand.Parameters.AddWithValue("@SentDate", asset.SentDate);
                repairCommand.Parameters.AddWithValue("@ReceiveDate", asset.ReceiveDate);
                repairCommand.Parameters.AddWithValue("@Repair_Cost", asset.Repair_Cost);
                repairCommand.Parameters.AddWithValue("@RepairStatus", asset.RepairStatus);
                repairCommand.Parameters.AddWithValue("@Tracking", asset.Tracking);
                repairCommand.Parameters.AddWithValue("@RepairNotes", asset.RepairNotes);
                repairCommand.Parameters.AddWithValue("@DeliveredBy", asset.DeliveredBy);
                repairCommand.ExecuteNonQuery();
            }
        }

        public void InsertIntoDamagedTable(SqlConnection connection, SqlTransaction transaction, Asset asset, string customAssetID)
        {
            using (SqlCommand damagedCommand = new SqlCommand(
                                         "INSERT INTO Damaged (AssetID, AssetType, Make, Status, Serial_Number, Purchase_Cost, Purchase_Year, DamagedNotes) " +
                                         "VALUES (@AssetID, @AssetType, @Make, @Status, @Serial_Number, @Purchase_Cost, @Purchase_Year, @DamagedNotes)",
                                         connection, transaction))
            {
                damagedCommand.Parameters.AddWithValue("@AssetID", customAssetID);
                damagedCommand.Parameters.AddWithValue("@AssetType", asset.AssetType);
                damagedCommand.Parameters.AddWithValue("@Make", asset.Make);
                damagedCommand.Parameters.AddWithValue("@Status", asset.Status);
                damagedCommand.Parameters.AddWithValue("@Serial_Number", asset.Serial_Number);
                damagedCommand.Parameters.AddWithValue("@Purchase_Cost", asset.Purchase_Cost);
                damagedCommand.Parameters.AddWithValue("@Purchase_Year", asset.Purchase_Year);
                damagedCommand.Parameters.AddWithValue("@DamagedNotes", asset.DamagedNotes);
                damagedCommand.ExecuteNonQuery();
            }
        }

        public void InsertIntoSoldTable(SqlConnection connection, SqlTransaction transaction, Asset asset, string customAssetID)
        {
            using (SqlCommand soldCommand = new SqlCommand(
                                         "INSERT INTO Sold (AssetID, AssetType, Make, Processor, Status, Serial_Number, Purchase_Cost, Sold_Cost, Purchase_Year, Sold_Year, SoldNotes, SoldTo, Approvals) " +
                                         "VALUES (@AssetID, @AssetType, @Make, @Processor, @Status, @Serial_Number, @Purchase_Cost, @Sold_Cost, @Purchase_Year, @Sold_Year, @SoldNotes, @SoldTo, @Approvals)",
                                         connection, transaction))
            {
                soldCommand.Parameters.AddWithValue("@AssetID", customAssetID);
                soldCommand.Parameters.AddWithValue("@AssetType", asset.AssetType);
                soldCommand.Parameters.AddWithValue("@Make", asset.Make);
                soldCommand.Parameters.AddWithValue("@Processor", asset.Processor);
                soldCommand.Parameters.AddWithValue("@Status", asset.Status);
                soldCommand.Parameters.AddWithValue("@Serial_Number", asset.Serial_Number);
                soldCommand.Parameters.AddWithValue("@Purchase_Cost", asset.Purchase_Cost);
                soldCommand.Parameters.AddWithValue("@Sold_Cost", asset.Sold_Cost);
                soldCommand.Parameters.AddWithValue("@Purchase_Year", asset.Purchase_Year);
                soldCommand.Parameters.AddWithValue("@Sold_Year", asset.Sold_Year);
                soldCommand.Parameters.AddWithValue("@SoldNotes", asset.SoldNotes);
                soldCommand.Parameters.AddWithValue("@SoldTo", asset.SoldTo);
                soldCommand.Parameters.AddWithValue("@Approvals", asset.Approvals);
                soldCommand.ExecuteNonQuery();
            }
        }

        public void InsertIntoEWasteTable(SqlConnection connection, SqlTransaction transaction, Asset asset, string customAssetID)
        {
            using (SqlCommand ewasteCommand = new SqlCommand(
                                          "INSERT INTO EWaste (AssetID, AssetType, Make, Processor, Status, Serial_Number, Purchase_Year, EWaste_Vendor, EWasteNotes, EWasteApprovals) " +
                                          "VALUES (@AssetID, @AssetType, @Make, @Processor, @Status, @Serial_Number, @Purchase_Year, @EWaste_Vendor, @EWasteNotes, @EWasteApprovals)",
                                           connection, transaction))
            {
                ewasteCommand.Parameters.AddWithValue("@AssetID", customAssetID);
                ewasteCommand.Parameters.AddWithValue("@AssetType", asset.AssetType);
                ewasteCommand.Parameters.AddWithValue("@Make", asset.Make);
                ewasteCommand.Parameters.AddWithValue("@Processor", asset.Processor);
                ewasteCommand.Parameters.AddWithValue("@Status", asset.Status);
                ewasteCommand.Parameters.AddWithValue("@Serial_Number", asset.Serial_Number);
                ewasteCommand.Parameters.AddWithValue("@Purchase_Year", asset.Purchase_Year);
                ewasteCommand.Parameters.AddWithValue("@EWaste_Vendor", asset.EWaste_Vendor);
                ewasteCommand.Parameters.AddWithValue("@EWasteNotes", asset.EWasteNotes);
                ewasteCommand.Parameters.AddWithValue("@EWasteApprovals", asset.EWasteApprovals);
                ewasteCommand.ExecuteNonQuery();
            }
        }

        public void InsertAssetIntoDatabase(SqlConnection connection, SqlTransaction transaction, Asset asset, string customAssetID)
        {
            using (SqlCommand command = new SqlCommand(
                "INSERT INTO Assets (AssetID, AssetType, Make, Model, Status, RAM, Processor, OS, Serial_Number, IMEI_Number, Purchase_Cost, Purchase_Year, MonthsInUse, NextRecycleDate) " +
                "VALUES (@AssetID, @AssetType, @Make, @Model, @Status, @RAM, @Processor, @OS, @Serial_Number, @IMEI_Number, @Purchase_Cost, @Purchase_Year, @MonthsInUse, @NextRecycleDate)",
                connection, transaction))
            {
                command.Parameters.AddWithValue("@AssetID", customAssetID);
                command.Parameters.AddWithValue("@AssetType", asset.AssetType);
                command.Parameters.AddWithValue("@Make", asset.Make);
                command.Parameters.AddWithValue("@Model", asset.Model);
                command.Parameters.AddWithValue("@Status", asset.Status);
                command.Parameters.AddWithValue("@RAM", asset.RAM);
                command.Parameters.AddWithValue("@Processor", asset.Processor);
                command.Parameters.AddWithValue("@OS", asset.OS);
                command.Parameters.AddWithValue("@Serial_Number", asset.Serial_Number);
                command.Parameters.AddWithValue("@IMEI_Number", asset.IMEI_Number);
                command.Parameters.AddWithValue("@Purchase_Cost", asset.Purchase_Cost);
                command.Parameters.AddWithValue("@Purchase_Year", asset.Purchase_Year);
                command.Parameters.AddWithValue("@MonthsInUse", asset.MonthsInUse);
                command.Parameters.AddWithValue("@NextRecycleDate", asset.NextRecycleDate);
                command.ExecuteNonQuery();
            }
        }

        public string GetLastAssetID(SqlConnection connection, SqlTransaction transaction)
        {
            using (SqlCommand command = new SqlCommand("SELECT TOP 1 AssetID FROM Assets ORDER BY AssetID DESC", connection, transaction))
            {
                string lastAssetID = command.ExecuteScalar() as string;
                if (string.IsNullOrEmpty(lastAssetID))
                {
                    return "ABC10000000"; // Default if no records are found
                }

                // Check if the text part of the last asset ID matches a certain pattern
                if (lastAssetID.StartsWith("ABC"))
                {
                    return lastAssetID;
                }
                else
                {
                    return "ABC10000001"; // Reset the numeric part to 10000001
                }
            }
        }

        public int FindNextAssetID(SqlConnection connection, SqlTransaction transaction)
        {
            using (SqlCommand command = new SqlCommand("SELECT MAX(CAST(SUBSTRING(AssetID, 4, LEN(AssetID) - 3) AS INT)) FROM Assets", connection, transaction))
            {
                object result = command.ExecuteScalar();
                if (result == DBNull.Value)
                {
                    return 10000001; // If no records exist, start from 1
                }

                int maxNumericPart = (int)result;

                // Check if the last asset ID text part matches a certain pattern
                if (GetLastAssetID(connection, transaction).StartsWith("ABC"))
                {
                    return maxNumericPart + 1;
                }
                else
                {
                    return 10000001; // Reset the numeric part to 10000001
                }
            }
        }

        public bool AssetExistsInRelatedTables(SqlConnection connection, SqlTransaction transaction, string assetID)
        {
            // Check if the asset exists in any of the related tables.
            return AssetExistsInTable(connection, transaction, "InStock", assetID) ||
                AssetExistsInTable(connection, transaction, "Repair", assetID) ||
                AssetExistsInTable(connection, transaction, "Damaged", assetID) ||
                AssetExistsInTable(connection, transaction, "Sold", assetID) ||
                AssetExistsInTable(connection, transaction, "EWaste", assetID);
        }

        public bool AssetExistsInTable(SqlConnection connection, SqlTransaction transaction, string tableName, string assetID)
        {
            // Check if the asset exists in a specific related table.
            using (SqlCommand command = new SqlCommand(
                $"SELECT COUNT(*) FROM {tableName} WHERE AssetID = @AssetID",
                connection, transaction))
            {
                command.Parameters.AddWithValue("@AssetID", assetID);
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        public void DeleteRelatedRecords(SqlConnection connection, SqlTransaction transaction, string assetID)
        {
            // Delete related records from each related table.
            DeleteRecordsFromTable(connection, transaction, "InStock", assetID);
            DeleteRecordsFromTable(connection, transaction, "Repair", assetID);
            DeleteRecordsFromTable(connection, transaction, "Damaged", assetID);
            DeleteRecordsFromTable(connection, transaction, "Sold", assetID);
            DeleteRecordsFromTable(connection, transaction, "EWaste", assetID);
        }

        public void DeleteRecordsFromTable(SqlConnection connection, SqlTransaction transaction, string tableName, string assetID)
        {
            // Delete related records from a specific related table.
            using (SqlCommand command = new SqlCommand(
                $"DELETE FROM {tableName} WHERE AssetID = @AssetID",
                connection, transaction))
            {
                command.Parameters.AddWithValue("@AssetID", assetID);
                command.ExecuteNonQuery();
            }
        }
        
        public int? GetNullableInt32(SqlDataReader reader, int ordinal)
        {
            if (!reader.IsDBNull(ordinal))
            {
                int value;
                if (int.TryParse(reader.GetString(ordinal), out value))
                {
                    return value;
                }
            }
            return null;
        }
       
        public Asset MapAssetFromReader(SqlDataReader reader, string tableName, Asset asset)
        {
            //Asset asset = new Asset();

            asset.AssetID = reader.GetString(0);

            // Set properties specific to the table
            switch (tableName)
            {
                case "Assets":
                    asset.AssetID = reader.GetString(0);
                    asset.AssetType = reader.GetString(1);
                    asset.Make = reader.GetString(2);
                    asset.Model = reader.GetString(3);
                    asset.Serial_Number = reader.GetString(4);
                    asset.RAM = reader.GetString(5);
                    asset.Processor = reader.GetString(6);
                    asset.OS = reader.GetString(7);
                    if (!reader.IsDBNull(8))
                    {
                        if (decimal.TryParse(reader[8].ToString(), out decimal purchaseCost))
                        {
                            asset.Purchase_Cost = purchaseCost;
                        }
                        else
                        {
                            // Handle the case where the value is not a valid decimal
                        }
                    }

                    asset.Purchase_Year = reader.GetDateTime(9);
                    asset.MonthsInUse = reader.GetString(10);
                    asset.NextRecycleDate = reader.GetDateTime(11);
                    asset.IMEI_Number = reader.GetString(12);
                    asset.Status = reader.GetString(13);
                    int columnIndex = 14;
                    if (!reader.IsDBNull(columnIndex))
                    {
                        asset.AssignedToUserID = reader.GetString(columnIndex);
                    }
                    else
                    {
                        // Handle the case where the value is NULL, e.g., assign a default value or do nothing
                        asset.AssignedToUserID = null; // or assign a default value
                    }
                    break;
                case "InStock":
                    if (asset.AssetID == null)
                    {
                        asset.AssetID = reader.GetString(0);
                    }
                    if (asset.AssetType == null)
                    {
                        asset.AssetType = reader.GetString(1);
                    }
                    if (asset.Make == null)
                    {
                        asset.Make = reader.GetString(2);
                    }
                    if (asset.Model == null)
                    {
                        asset.Model = reader.GetString(3);
                    }
                    if (asset.Status == null)
                    {
                        asset.Status = reader.GetString(4);
                    }
                    if (asset.RAM == null)
                    {
                        asset.RAM = reader.GetString(5);
                    }
                    if (asset.Processor == null)
                    {
                        asset.Processor = reader.GetString(6);
                    }
                    if (asset.OS == null)
                    {
                        asset.OS = reader.GetString(7);
                    }
                    if (asset.Serial_Number == null)
                    {
                        asset.Serial_Number = reader.GetString(8);
                    }
                    if (asset.IMEI_Number == null)
                    {
                        asset.IMEI_Number = reader.GetString(9);
                    }
                    break;
                case "Damaged":
                    if (asset.AssetID == null)
                    {
                        asset.AssetID = reader.GetString(0);
                    }
                    if (asset.AssetType == null)
                    {
                        asset.AssetType = reader.GetString(1);
                    }
                    if (asset.Make == null)
                    {
                        asset.Make = reader.GetString(2);
                    }
                    if (asset.Status == null)
                    {
                        asset.Status = reader.GetString(3);
                    }
                    if (asset.Serial_Number == null)
                    {
                        asset.Serial_Number = reader.GetString(4);
                    }
                    if (asset.Purchase_Cost == null)
                    {
                        if (!reader.IsDBNull(5))
                        {
                            if (decimal.TryParse(reader[5].ToString(), out decimal purchaseCost))
                            {
                                asset.Purchase_Cost = purchaseCost;
                            }
                            else
                            {
                                // Handle the case where the value is not a valid decimal
                            }
                        }
                    }
                    if (asset.Purchase_Year == null)
                    {
                        asset.Purchase_Year = reader.GetDateTime(6);
                    }
                    if (asset.DamagedNotes == null)
                    {
                        asset.DamagedNotes = reader.GetString(7);
                    }
                    break;
                case "Repair":
                    if (asset.AssetID == null)
                    {
                        asset.AssetID = reader.GetString(0);
                    }
                    if (asset.AssetType == null)
                    {
                        asset.AssetType = reader.GetString(1);
                    }
                    if (asset.Make == null)
                    {
                        asset.Make = reader.GetString(2);
                    }
                    if (asset.Model == null)
                    {
                        asset.Model = reader.GetString(3);
                    }
                    if (asset.Status == null)
                    {
                        asset.Status = reader.GetString(4);
                    }
                    if (asset.Vendor == null)
                    {
                        asset.Vendor = reader.GetString(5);
                    }
                    if (asset.SentDate == null)
                    {
                        asset.SentDate = reader.GetDateTime(6);
                    }
                    if (asset.ReceiveDate == null)
                    {
                        asset.ReceiveDate = reader.GetDateTime(7);
                    }
                    if (asset.Repair_Cost == null)
                    {
                        if (!reader.IsDBNull(8))
                        {
                            if (decimal.TryParse(reader[8].ToString(), out decimal repairCost))
                            {
                                asset.Repair_Cost = repairCost;
                            }
                            else
                            {
                                // Handle the case where the value is not a valid decimal
                            }
                        }
                    }
                    if (asset.RepairStatus == null)
                    {
                        asset.RepairStatus = reader.GetString(9);
                    }
                    if (asset.Tracking == null)
                    {
                        asset.Tracking = reader.GetString(10);
                    }
                    if (asset.RepairNotes == null)
                    {
                        asset.RepairNotes = reader.GetString(11);
                    }
                    if (asset.DeliveredBy == null)
                    {
                        asset.DeliveredBy = reader.GetString(12);
                    }
                    break;
                case "Sold":
                    if (asset.AssetID == null)
                    {
                        asset.AssetID = reader.GetString(0);
                    }
                    if (asset.AssetType == null)
                    {
                        asset.AssetType = reader.GetString(1);
                    }
                    if (asset.Make == null)
                    {
                        asset.Make = reader.GetString(2);
                    }
                    if (asset.Processor == null)
                    {
                        asset.Processor = reader.GetString(3);
                    }
                    if (asset.Status == null)
                    {
                        asset.Status = reader.GetString(4);
                    }
                    if (asset.Serial_Number == null)
                    {
                        asset.Serial_Number = reader.GetString(5);
                    }
                    if (asset.Purchase_Cost == null)
                    {
                        if (!reader.IsDBNull(6))
                        {
                            if (decimal.TryParse(reader[6].ToString(), out decimal purchaseCost))
                            {
                                asset.Purchase_Cost = purchaseCost;
                            }
                            else
                            {
                                // Handle the case where the value is not a valid decimal
                            }
                        }
                    }
                    if (asset.Sold_Cost == null)
                    {
                        if (!reader.IsDBNull(7))
                        {
                            if (decimal.TryParse(reader[7].ToString(), out decimal soldCost))
                            {
                                asset.Sold_Cost = soldCost;
                            }
                            else
                            {
                                // Handle the case where the value is not a valid decimal
                            }
                        }
                    }
                    if (asset.Purchase_Year == null)
                    {
                        asset.Purchase_Year = reader.GetDateTime(8);
                    }
                    if (asset.Sold_Year == null)
                    {
                        asset.Sold_Year = reader.GetDateTime(9);
                    }
                    if (asset.SoldNotes == null)
                    {
                        asset.SoldNotes = reader.GetString(10);
                    }
                    if (asset.SoldTo == null)
                    {
                        asset.SoldTo = reader.GetString(11);
                    }
                    if (asset.Approvals == null)
                    {
                        asset.Approvals = reader.GetString(12);
                    }
                    break;
                case "EWaste":
                    if (asset.AssetID == null)
                    {
                        asset.AssetID = reader.GetString(0);
                    }
                    if (asset.AssetType == null)
                    {
                        asset.AssetType = reader.GetString(1);
                    }
                    if (asset.Make == null)
                    {
                        asset.Make = reader.GetString(2);
                    }
                    if (asset.Processor == null)
                    {
                        asset.Processor = reader.GetString(3);
                    }
                    if (asset.Status == null)
                    {
                        asset.Status = reader.GetString(4);
                    }
                    if (asset.Serial_Number == null)
                    {
                        asset.Serial_Number = reader.GetString(5);
                    }
                    if (asset.Purchase_Year == null)
                    {
                        asset.Purchase_Year = reader.GetDateTime(6);
                    }
                    if (asset.EWaste_Vendor == null)
                    {
                        asset.EWaste_Vendor = reader.GetString(7);
                    }
                    if (asset.EWasteNotes == null)
                    {
                        asset.EWasteNotes = reader.GetString(8);
                    }
                    if (asset.EWasteApprovals == null)
                    {
                        asset.EWasteApprovals = reader.GetString(9);
                    }
                    break;
                case "Allocated":
                    if (asset.AssetID == null)
                    {
                        asset.AssetID = reader.GetString(0);
                    }
                    if (asset.AssetType == null)
                    {
                        asset.AssetType = reader.GetString(1);
                    }
                    if (asset.Make == null)
                    {
                        asset.Make = reader.GetString(2);
                    }
                    if (asset.Processor == null)
                    {
                        asset.Processor = reader.GetString(3);
                    }
                    if (asset.RAM == null)
                    {
                        asset.RAM = reader.GetString(4);
                    }
                    if (asset.Status == null)
                    {
                        asset.Status = reader.GetString(5);
                    }
                    if (asset.Serial_Number == null)
                    {
                        asset.Serial_Number = reader.GetString(6);
                    }
                    if (asset.AssignedToUserIDString == null)
                    {
                        asset.AssignedToUserIDString = reader.GetString(7);

                    }
                    if (asset.Email == null)
                    {
                        asset.Email = reader.GetString(8);
                    }
                    if (asset.Username == null)
                    {
                        asset.Username = reader.GetString(9);
                    }
                    if (asset.PhoneNumber == null)
                    {
                        asset.PhoneNumber = reader.GetString(10);
                    }
                    if (asset.Location == null)
                    {
                        asset.Location = reader.GetString(11);
                    }
                    if (asset.ReportingManager == null)
                    {
                        asset.ReportingManager = reader.GetString(12);
                    }
                    break;
            }

            return asset;
        }
    }
}
