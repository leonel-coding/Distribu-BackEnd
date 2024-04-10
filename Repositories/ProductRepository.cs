using DTOLibrary.Dtos;
using DTOLibrary.Interfaces;
using DTOLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BackEnd_ADONET.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(IConfiguration configuration) 
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task DeleteProduct(int productId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("DisableProductAndRelations", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProductId", productId);

                    connection.Open();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<ProductDto> GetProductById(int productId)
        {
            ProductDto product = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetProductById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProductId", productId);

                    await connection.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            product = new ProductDto
                            {
                                ProductId = Convert.ToInt32(reader["ProductId"]),
                                ProductCode = reader["ProductCode"].ToString(),
                                ProductName = reader["ProductName"].ToString(),
                                TypeProductId = Convert.ToInt32(reader["TypeProductId"]),
                                UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                PathImage = reader["PathImage"] != DBNull.Value ? reader["PathImage"].ToString() : null
                            };
                        }
                    }
                }
            }

            return product;
        }

        public async Task<IEnumerable<ProductDto>> GetProducts(bool IsActive = true)
        {
            List<ProductDto> products = new List<ProductDto>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "EXEC GetProducts";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ProductDto product = new ProductDto
                    {
                        ProductId = Convert.ToInt32(reader["ProductId"]),
                        ProductName = reader["ProductName"].ToString(),
                        ProductCode = reader["ProductCode"].ToString(),
                        UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        TypeProductId = Convert.ToInt32(reader["TypeProductId"])
                    };
                    products.Add(product);
                }
                connection.Close();
            }

            return products.Where(a => a.IsActive).ToList();
        }

        public ProductViewModel GetProductVm(int productId)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductViewModel> GetProductWithRelationsSuppliersByProductId(int productId)
        {
            ProductViewModel productViewModel = new ProductViewModel();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("GetProductWithRelationsSuppliersByProductId", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ProductId", productId);

                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                productViewModel = await MapToProductViewModel(reader);
                            }
                            else
                            {
                                // No se encontraron resultados
                                productViewModel = null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que ocurra
                Console.WriteLine($"Error al obtener el ProductViewModel: {ex.Message}");
                throw;
            }

            return productViewModel;
        }

        private async Task<ProductViewModel> MapToProductViewModel(SqlDataReader reader)
        {
            ProductViewModel productModel = new ProductViewModel();

            productModel.ProductId = reader.GetInt32(reader.GetOrdinal("ProductId"));
            productModel.ProductCode = reader.GetString(reader.GetOrdinal("ProductCode"));
            productModel.ProductName = reader.GetString(reader.GetOrdinal("ProductName"));
            productModel.TypeProductId = reader.GetInt32(reader.GetOrdinal("TypeProductId"));
            productModel.UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice"));
            productModel.IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));
            productModel.PathImage = reader.IsDBNull(reader.GetOrdinal("PathImage")) ? null : reader.GetString(reader.GetOrdinal("PathImage"));

            productModel.TypeProduct = new TypeProductDto
            {
                TypeProductId = reader.GetInt32(reader.GetOrdinal("TypeProduct_TypeProductId")),
                TypeProductDesc = reader.GetString(reader.GetOrdinal("TypeProduct_TypeProductDesc"))
            };

            productModel.ProductSuppliers = new List<ProductSupplierViewModel>();

            // Mapear las filas de ProductSupplier
            do 
            {
                int? productSupplierId = reader.IsDBNull(reader.GetOrdinal("ProductSupplierId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ProductSupplierId"));


                if (productSupplierId == null)
                {
                    continue;
                }

                ProductSupplierViewModel productSupplierViewModel = new ProductSupplierViewModel();

                productSupplierViewModel.ProductSupplierId = productSupplierId.Value;
                productSupplierViewModel.SupplierId = reader.GetInt32(reader.GetOrdinal("SupplierId"));
                productSupplierViewModel.DateRecord = reader.GetDateTime(reader.GetOrdinal("ProductSupplier_DateRecord"));
                productSupplierViewModel.IsActive = reader.GetBoolean(reader.GetOrdinal("ProductSupplier_IsActive"));
                productSupplierViewModel.ProductSupplierCode = reader.GetString(reader.GetOrdinal("ProductSupplierCode"));
                productSupplierViewModel.ProductSupplierUnitPrice = reader.GetDecimal(reader.GetOrdinal("ProductSupplierUnitPrice"));

                productSupplierViewModel.Supplier = new SupplierDto
                {
                    SupplierId = reader.GetInt32(reader.GetOrdinal("Supplier_SupplierId")),
                    SupplierName = reader.GetString(reader.GetOrdinal("SupplierName"))
                };

                productModel.ProductSuppliers.Add(productSupplierViewModel);
            }
            while(reader.Read());

            if (productModel.ProductSuppliers.Count == 0)
                productModel.ProductSuppliers = null;

            return productModel;
        }

        public async Task<ProductDto> InsertProduct(ProductDto product)
        {
            int productId = 0;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("InsertProduct", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@ProductCode", SqlDbType.VarChar).Value = product.ProductCode;
                    command.Parameters.Add("@ProductName", SqlDbType.VarChar).Value = product.ProductName;
                    command.Parameters.Add("@TypeProductId", SqlDbType.Int).Value = product.TypeProductId;
                    command.Parameters.Add("@UnitPrice", SqlDbType.Decimal).Value = product.UnitPrice;
                    command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = true;

                    if (product.PathImage != null)
                    {
                        command.Parameters.AddWithValue("@PathImage", product.PathImage);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@PathImage", DBNull.Value);
                    }

                    SqlParameter outputParameter = new SqlParameter("@NewProductId", SqlDbType.Int);
                    outputParameter.Direction = ParameterDirection.Output;
                    command.Parameters.Add(outputParameter);

                    connection.Open();
                    command.ExecuteNonQuery();

                    productId = Convert.ToInt32(command.Parameters["@NewProductId"].Value);

                    product.ProductId = productId;
                }
            }

            return product;
        }

        public async Task UpdateProduct(ProductDto product)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("UpdateProduct", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProductId", product.ProductId);
                    command.Parameters.AddWithValue("@ProductCode", product.ProductCode);
                    command.Parameters.AddWithValue("@ProductName", product.ProductName);
                    command.Parameters.AddWithValue("@TypeProductId", product.TypeProductId);
                    command.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);

                    connection.Open();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
