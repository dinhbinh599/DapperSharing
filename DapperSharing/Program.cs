using Dapper;
using DapperSharing.Models;
using DapperSharing.Utils;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace DapperSharing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            IEnumerable<ProductEntity> results;

            using (var connection = new SqlConnection(DBInfo.ConnectionString))
            {
                var sql = @"
SELECT * FROM production.products
ORDER BY product_id
OFFSET 0 ROWS
FETCH NEXT 5 ROWS ONLY";

                results = connection.Query<ProductEntity>(sql);
            }
            DisplayHelper.PrintJson(results);
        }

        static partial class DBInfo
        {
            public const string ConnectionString = "Server=TUANPHAM;Database=BikeStores;Trusted_Connection=True;Encrypt=False";
        }
    }
}