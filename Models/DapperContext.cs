using System.Data;
using api_aspnetcore6.Helpers;
using Microsoft.Data.SqlClient;

namespace api_aspnetcore6.Models
{
    public static class DapperContext
    {
        public static IDbConnection CreateConnection()
        {
            var con = ConfigurationManagers.AppSetting["ConnectionStrings:ConnStr"];
            return new SqlConnection(con);
        }
    }
}