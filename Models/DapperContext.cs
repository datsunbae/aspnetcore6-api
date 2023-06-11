using System.Data;
using Microsoft.Data.SqlClient;

namespace api_aspnetcore6.Models
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IDbConnection CreateConnection()
          => new SqlConnection(_configuration.GetConnectionString("ConnStr"));
    }
}