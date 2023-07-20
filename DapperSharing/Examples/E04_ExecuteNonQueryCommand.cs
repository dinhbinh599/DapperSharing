using Dapper;
using DapperSharing.Models;
using DapperSharing.Utils;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DapperSharing.Examples
{
    public static class E04_ExecuteNonQueryCommand
    {
        public static async Task Run()
        {
            Console.WriteLine("=========== RUNNING E04_ExecuteNonQueryCommand ===========");
            DisplayHelper.PrintListOfMethods(typeof(E04_ExecuteNonQueryCommand));
            using (var connection = new SqlConnection(Program.DBInfo.ConnectionString))
            {
                var userInput = Console.ReadLine();
                switch (userInput)
                {
                    case "1":
                        await Insert(connection);
                        break;
                    case "2":
                        await Update(connection);
                        break;
                    case "3":
                        await MultipleCommands(connection);
                        break;
                }
            }
        }

        static async Task<int> Insert(IDbConnection connection)
        {
            var sql = @"
                INSERT INTO production.products
                    (ProductName, BrandId, CategoryId, ModelYear, ListPrice)
                VALUES 
                    (@ProductName, @BrandId, @CategoryId, @ModelYear, @ListPrice);";
                
            var count = await connection.ExecuteAsync(sql, new Product
            {
                BrandId = 1,
                CategoryId = 1,
                ListPrice = 150,
                ModelYear = 2023,
                ProductName = "My 2023 Product"
            });
            Console.WriteLine($"Affected rows: {count}");

            var id = await connection.ExecuteScalarAsync<int>(@"
                SELECT ProductId from production.products
                ORDER BY ProductId DESC");

            Console.WriteLine($"New product ID: {id}");

            return id;
        }

        static async Task Update(IDbConnection connection)
        {
            Console.Write("Update product id: ");
            int id = int.Parse(Console.ReadLine());
            var sql = @"
                UPDATE production.products 
                SET ProductName = @ProductName, ModelYear = @ModelYear
                WHERE ProductId = @ProductId;";

            var count = await connection.ExecuteAsync(sql, new
            {
                ProductId = id,
                ProductName = "My new product 2023 has been updated",
                ModelYear = 2023
            });

            Console.WriteLine($"Affected rows: {count}");

            var sq1l = @"SELECT * FROM production.products
                        WHERE ProductId = " + id;

            var entity = connection.QueryFirst<Product>(sq1l);
            DisplayHelper.PrintJson(entity);
        }

        static async Task MultipleCommands(IDbConnection connection)
        {
            var sql = @"
                UPDATE production.products 
                SET ProductName = ''
                WHERE ModelYear = @ModelYear;

                DELETE FROM production.products
                WHERE ProductId = @Id;";

            Console.Write("Delete product id: ");
            int id = int.Parse(Console.ReadLine());
            var count = await connection.ExecuteAsync(sql, new
            {
                ModelYear = 2023,
                Id = id
            });

            Console.WriteLine($"Affected rows: {count}");

            Console.WriteLine($"Products in 2023:");
            var sq1l = @"SELECT * FROM production.products
                        WHERE ModelYear = 2023";

            var entity = connection.Query<Product>(sq1l);
            DisplayHelper.PrintJson(entity);
        }
    }
}
