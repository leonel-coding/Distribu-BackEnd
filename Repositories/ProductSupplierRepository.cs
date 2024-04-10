using DTOLibrary.Dtos;
using DTOLibrary.Interfaces;
using System.Data.SqlClient;
using System.Data;

namespace BackEnd_ADONET.Repositories
{
    public class ProductSupplierRepository : IProductSupplierRepository
    {
        private readonly string _connectionString;

        public ProductSupplierRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task DeleteProductSupplier(int productSupplierId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("DisableProductSupplier", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@productSupplierId", productSupplierId);

                    connection.Open();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<ProductSupplierDto> GetProductSupplierById(int productSupplierId)
        {
            ProductSupplierDto productSupplier = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetProductSupplierById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@productSupplierId", productSupplierId);

                    await connection.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            productSupplier = new ProductSupplierDto
                            {
                                ProductSupplierId = Convert.ToInt32(reader["ProductSupplierId"]),
                                ProductId = Convert.ToInt32(reader["ProductId"]),
                                SupplierId = Convert.ToInt32(reader["SupplierId"]),
                                ProductSupplierCode = reader["ProductSupplierCode"].ToString(),
                                ProductSupplierUnitPrice = Convert.ToDecimal(reader["ProductSupplierUnitPrice"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            };
                        }
                    }
                }
            }

            return productSupplier;
        }

        public Task<ProductSupplierDto> GetSupplierByProductId(int productSupplierId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductSupplierDto>> GetSuppliersByProductId()
        {
            throw new NotImplementedException();
        }

        public async Task<ProductSupplierDto> InsertProductSupplier(ProductSupplierDto productSupplier)
        {
            int productSupplierId = 0;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("InsertProductSupplier", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProductId", productSupplier.ProductId);
                    command.Parameters.AddWithValue("@SupplierId", productSupplier.SupplierId);
                    command.Parameters.AddWithValue("@ProductSupplierCode", productSupplier.ProductSupplierCode);
                    command.Parameters.AddWithValue("@ProductSupplierUnitPrice", productSupplier.ProductSupplierUnitPrice);
                    command.Parameters.AddWithValue("@DateRecord", DateTime.Now);
                    command.Parameters.AddWithValue("@IsActive", true);

                    SqlParameter outputParameter = new SqlParameter("@NewProductSupplierId", SqlDbType.Int);
                    outputParameter.Direction = ParameterDirection.Output;
                    command.Parameters.Add(outputParameter);

                    connection.Open();
                    command.ExecuteNonQuery();

                    productSupplierId = Convert.ToInt32(command.Parameters["@NewProductSupplierId"].Value);

                    productSupplier.ProductSupplierId = productSupplierId;
                }
            }

            return productSupplier;
        }

        public async Task UpdateProductSupplier(ProductSupplierDto productSupplier)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("UpdateProductSupplier", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProductSupplierId", productSupplier.ProductSupplierId);
                    command.Parameters.AddWithValue("@ProductSupplierCode", productSupplier.ProductSupplierCode);
                    command.Parameters.AddWithValue("@ProductSupplierUnitPrice", productSupplier.ProductSupplierUnitPrice);

                    connection.Open();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
