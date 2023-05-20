using Dapper;
using DapperSharing.Models;
using DapperSharing.Utils;
using Microsoft.Data.SqlClient;

namespace DapperSharing.Examples
{
    public static class E01_QuickStart
    {
        public static void Run()
        {
            Console.WriteLine("=========== RUNNING E01_QuickStart ===========");
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            IEnumerable<ProductEntity> results;

            using (var connection = new SqlConnection(Program.DBInfo.ConnectionString))
            {
                var sql = @"
SELECT * FROM production.products 
ORDER BY product_id
OFFSET 0 ROWS
FETCH NEXT 10 ROWS ONLY";

                results = connection.Query<ProductEntity>(sql).ToList();
            }

            DisplayHelper.PrintJson(results);
        }
    }
}
