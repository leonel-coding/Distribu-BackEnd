using DTOLibrary.Dtos;
using DTOLibrary.Interfaces;
using System.Data.SqlClient;

namespace BackEnd_ADONET.Repositories
{
    public class TypeProductRepository : ITypeProductRepository
    {
        private readonly string _connectionString;

        public TypeProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public void DeleteTypeProduct(int typeProductId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<TypeProductDto>> GetTypeProduct()
        {
            List<TypeProductDto> typeProducts = new List<TypeProductDto>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "EXEC GetTypesOfProduct";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    TypeProductDto typeProduct = new TypeProductDto
                    {
                        TypeProductId = Convert.ToInt32(reader["TypeProductId"]),
                        TypeProductDesc = reader["TypeProductDesc"].ToString()

                    };
                    typeProducts.Add(typeProduct);
                }
                connection.Close();
            }

            return typeProducts;
        }

        public void UpdateTypeProduct(TypeProductDto product)
        {
            throw new NotImplementedException();
        }

        int ITypeProductRepository.InsertTypeProduct(TypeProductDto product)
        {
            throw new NotImplementedException();
        }
    }
}
