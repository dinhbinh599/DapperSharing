using Dapper;
using DapperSharing.Models;
using DapperSharing.Utils;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace DapperSharing.Examples
{
    public static class E02_QueryData
    {
        public static async Task Run()
        {
            Console.WriteLine("=========== RUNNING E02_QueryData ===========");
            DisplayHelper.PrintListOfMethods(typeof(E02_QueryData));
            //Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            using (var connection = new SqlConnection(Program.DBInfo.ConnectionString))
            {
                var userInput = Console.ReadLine();
                switch (userInput)
                {
                    case "1":
                        await QueryScalar(connection);
                        break;
                    case "2":
                        await QuerySingleRow(connection);
                        break;
                    case "3":
                        await QueryMultipleRows(connection);
                        break;
                    case "4":
                        await QueryMultiResults(connection);
                        break;
                    case "5":
                        await QuerySpecificColumns(connection);
                        break;
                }
            }
        }

        static async Task QueryScalar(IDbConnection connection)
        {
            var sql = @"SELECT COUNT(*) FROM production.products";

            var count = await connection.ExecuteScalarAsync<int>(sql);

            Console.WriteLine($"Product count: {count}");
        }

        static async Task QuerySingleRow(IDbConnection connection)
        {
            var sql = @"SELECT * FROM production.products WHERE ProductId=1";

            var entity = await connection.QueryFirstOrDefaultAsync<Product>(sql);

            DisplayHelper.PrintJson(entity);
        }

        static async Task QueryMultipleRows(IDbConnection connection)
        {
            var sql = @"SELECT * FROM production.products WHERE ModelYear=2016";

            var results = await connection.QueryAsync<Product>(sql);

            DisplayHelper.PrintJson(results);
        }

        static async Task QueryMultiResults(IDbConnection connection)
        {
            var sql = @"
SELECT * FROM production.products WHERE ProductId=1;
SELECT * FROM production.products WHERE ModelYear=2016;";

            using (var multi = await connection.QueryMultipleAsync(sql))
            {
                var entity = await multi.ReadFirstOrDefaultAsync<Product>();

                var results = await multi.ReadAsync<Product>();

                DisplayHelper.PrintJson(entity);

                DisplayHelper.PrintJson(results);
            }
        }

        static async Task QuerySpecificColumns(IDbConnection connection)
        {
            var sql = @"SELECT ProductId, ProductName FROM production.products WHERE ProductId=1";

            var entity = await connection.QueryFirstOrDefaultAsync(sql);

            DisplayHelper.PrintJson(entity);
        }
    }
}
