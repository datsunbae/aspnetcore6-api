using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_aspnetcore6.Migrations
{
    public partial class StoreProcedureDailyRevenue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetDailyRevenue
                AS
                BEGIN
                    DECLARE @StartDate DATETIME = DATEADD(day, -1, GETDATE());
                    DECLARE @EndDate DATETIME = @StartDate + 1;

                    SELECT SUM(TotalAmount) AS DailyRevenue
                    FROM Orders
                    WHERE OrderDate BETWEEN @StartDate AND @EndDate;
                END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE GetDailyRevenue");
        }
    }
}
