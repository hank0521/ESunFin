using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using FinancialProducts.Models;

namespace FinancialProducts.Models
{
    // 資料庫訪問層
    public class DataAccess
    {
        private readonly string _connectionString;

        public DataAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // 執行一個存儲過程並返回一個數據集
        private async Task<DataSet> ExecuteStoredProcedureAsync(string storedProcedure, Dictionary<string, object> parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    var dataSet = new DataSet();
                    await connection.OpenAsync();

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataSet);
                    }

                    return dataSet;
                }
            }
        }

        // 執行一個存儲過程並返回影響的行數
        private async Task<int> ExecuteNonQueryAsync(string storedProcedure, Dictionary<string, object> parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    await connection.OpenAsync();
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        // 執行一個存儲過程並返回新插入的ID
        private async Task<int> ExecuteScalarAsync(string storedProcedure, Dictionary<string, object> parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    SqlParameter outputParam = new SqlParameter("@NewProductNo", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputParam);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    return (int)outputParam.Value;
                }
            }
        }

        // 執行帶有事務的多個操作

        private async Task<bool> ExecuteTransactionAsync(List<SqlCommand> commands)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var command in commands)
                        {
                            command.Connection = connection;
                            command.Transaction = transaction;
                            await command.ExecuteNonQueryAsync();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        #region 使用者相關方法
        // 獲取所有使用者
        public async Task<List<User>> GetAllUsersAsync()
        {
            var dataSet = await ExecuteStoredProcedureAsync("sp_GetAllUsers");
            var users = new List<User>();

            if (dataSet.Tables.Count > 0)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    users.Add(new User
                    {
                        UserID = row["UserID"].ToString(),
                        UserName = row["UserName"].ToString(),
                        Email = row["Email"].ToString(),
                        Account = row["Account"].ToString()
                    });
                }
            }

            return users;
        }

        // 根據ID獲取使用者
        public async Task<User> GetUserByIdAsync(string userId)
        {
            var parameters = new Dictionary<string, object> { { "@UserID", userId } };
            var dataSet = await ExecuteStoredProcedureAsync("sp_GetUserById", parameters);

            if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                var row = dataSet.Tables[0].Rows[0];
                return new User
                {
                    UserID = row["UserID"].ToString(),
                    UserName = row["UserName"].ToString(),
                    Email = row["Email"].ToString(),
                    Account = row["Account"].ToString()
                };
            }

            return null;
        }

        // 新增使用者
        public async Task<bool> InsertUserAsync(User user)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@UserID", user.UserID },
                { "@UserName", user.UserName },
                { "@Email", user.Email },
                { "@Account", user.Account }
            };

            var result = await ExecuteNonQueryAsync("sp_InsertUser", parameters);
            return result > 0;
        }
        #endregion

        #region 產品相關方法
        // 獲取所有產品
        public async Task<List<Product>> GetAllProductsAsync()
        {
            var dataSet = await ExecuteStoredProcedureAsync("sp_GetAllProducts");
            var products = new List<Product>();

            if (dataSet.Tables.Count > 0)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    products.Add(new Product
                    {
                        No = Convert.ToInt32(row["No"]),
                        ProductName = row["ProductName"].ToString(),
                        Price = Convert.ToDecimal(row["Price"]),
                        FeeRate = Convert.ToDecimal(row["FeeRate"])
                    });
                }
            }

            return products;
        }

        // 根據編號獲取產品
        public async Task<Product> GetProductByNoAsync(int no)
        {
            var parameters = new Dictionary<string, object> { { "@No", no } };
            var dataSet = await ExecuteStoredProcedureAsync("sp_GetProductByNo", parameters);

            if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                var row = dataSet.Tables[0].Rows[0];
                return new Product
                {
                    No = Convert.ToInt32(row["No"]),
                    ProductName = row["ProductName"].ToString(),
                    Price = Convert.ToDecimal(row["Price"]),
                    FeeRate = Convert.ToDecimal(row["FeeRate"])
                };
            }

            return null;
        }

        // 新增產品
        public async Task<int> InsertProductAsync(Product product)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@ProductName", product.ProductName },
                { "@Price", product.Price },
                { "@FeeRate", product.FeeRate },
                { "@NewProductNo", ParameterDirection.Output }
            };

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_InsertProduct", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProductName", product.ProductName);
                    command.Parameters.AddWithValue("@Price", product.Price);
                    command.Parameters.AddWithValue("@FeeRate", product.FeeRate);

                    var outputParam = command.Parameters.Add("@NewProductNo", SqlDbType.Int);
                    outputParam.Direction = ParameterDirection.Output;

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    return (int)outputParam.Value;
                }
            }
        }

        // 更新產品
        public async Task<bool> UpdateProductAsync(Product product)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@No", product.No },
                { "@ProductName", product.ProductName },
                { "@Price", product.Price },
                { "@FeeRate", product.FeeRate }
            };

            var result = await ExecuteNonQueryAsync("sp_UpdateProduct", parameters);
            return result > 0;
        }

        // 刪除產品
        public async Task<bool> DeleteProductAsync(int no)
        {
            var parameters = new Dictionary<string, object> { { "@No", no } };

            try
            {
                var result = await ExecuteNonQueryAsync("sp_DeleteProduct", parameters);
                return result > 0;
            }
            catch
            {
                // 如果產品存在於喜好清單中，刪除操作會失敗
                return false;
            }
        }
        #endregion

        #region 喜好清單相關方法
        // 獲取使用者喜好清單
        public async Task<List<LikeListItem>> GetUserLikeListAsync(string userId)
        {
            var parameters = new Dictionary<string, object> { { "@UserID", userId } };
            var dataSet = await ExecuteStoredProcedureAsync("sp_GetUserLikeList", parameters);
            var likeList = new List<LikeListItem>();

            if (dataSet.Tables.Count > 0)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    likeList.Add(new LikeListItem
                    {
                        SN = Convert.ToInt32(row["SN"]),
                        UserID = row["UserID"].ToString(),
                        ProductNo = Convert.ToInt32(row["ProductNo"]),
                        OrderQty = Convert.ToInt32(row["OrderQty"]),
                        Account = row["Account"].ToString(),
                        TotalFee = Convert.ToDecimal(row["TotalFee"]),
                        TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                        ProductName = row["ProductName"].ToString(),
                        Price = Convert.ToDecimal(row["Price"]),
                        FeeRate = Convert.ToDecimal(row["FeeRate"]),
                        Email = row["Email"].ToString()
                    });
                }
            }

            return likeList;
        }

        // 根據序號獲取喜好清單項目
        public async Task<LikeListItem> GetLikeListBySnAsync(int sn)
        {
            var parameters = new Dictionary<string, object> { { "@SN", sn } };
            var dataSet = await ExecuteStoredProcedureAsync("sp_GetLikeListBySN", parameters);

            if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                var row = dataSet.Tables[0].Rows[0];
                return new LikeListItem
                {
                    SN = Convert.ToInt32(row["SN"]),
                    UserID = row["UserID"].ToString(),
                    ProductNo = Convert.ToInt32(row["ProductNo"]),
                    OrderQty = Convert.ToInt32(row["OrderQty"]),
                    Account = row["Account"].ToString(),
                    TotalFee = Convert.ToDecimal(row["TotalFee"]),
                    TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                    ProductName = row["ProductName"].ToString(),
                    Price = Convert.ToDecimal(row["Price"]),
                    FeeRate = Convert.ToDecimal(row["FeeRate"]),
                    Email = row["Email"].ToString()
                };
            }

            return null;
        }

        // 新增喜好清單項目
        public async Task<int> InsertLikeListAsync(LikeList likeList)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_InsertLikeList", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", likeList.UserID);
                    command.Parameters.AddWithValue("@ProductNo", likeList.ProductNo);
                    command.Parameters.AddWithValue("@OrderQty", likeList.OrderQty);
                    command.Parameters.AddWithValue("@Account", likeList.Account);

                    var outputParam = command.Parameters.Add("@NewSN", SqlDbType.Int);
                    outputParam.Direction = ParameterDirection.Output;

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    return (int)outputParam.Value;
                }
            }
        }

        // 更新喜好清單項目
        public async Task<bool> UpdateLikeListAsync(LikeList likeList)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@SN", likeList.SN },
                { "@ProductNo", likeList.ProductNo },
                { "@OrderQty", likeList.OrderQty },
                { "@Account", likeList.Account }
            };

            var result = await ExecuteNonQueryAsync("sp_UpdateLikeList", parameters);
            return result > 0;
        }

        // 刪除喜好清單項目
        public async Task<bool> DeleteLikeListAsync(int sn)
        {
            var parameters = new Dictionary<string, object> { { "@SN", sn } };
            var result = await ExecuteNonQueryAsync("sp_DeleteLikeList", parameters);
            return result > 0;
        }
        #endregion
    }
}