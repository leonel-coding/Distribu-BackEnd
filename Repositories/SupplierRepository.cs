using DTOLibrary.Dtos;
using DTOLibrary.Interfaces;
using System.Data.SqlClient;

namespace BackEnd_ADONET.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly string _connectionString;

        public SupplierRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<SupplierDto>> GetSuppliers()
        {
            List<SupplierDto> suppliers = new List<SupplierDto>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "EXEC GetSuppliers";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    SupplierDto product = new SupplierDto
                    {
                        SupplierId = Convert.ToInt32(reader["SupplierId"]),
                        SupplierName = reader["SupplierName"].ToString()
                    };
                    suppliers.Add(product);
                }
                connection.Close();
            }

            return suppliers.OrderBy(a => a.SupplierName).ToList();
        }
    }
}
