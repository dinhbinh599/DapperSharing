using Dapper;
using DapperSharing.Models;
using DapperSharing.Utils;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DapperSharing.Examples
{
    public static class E07_Parameters
    {
        public static async Task Run()
        {
            Console.WriteLine("=========== RUNNING E07_Parameters ===========");
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            using (var connection = new SqlConnection(Program.DBInfo.ConnectionString))
            {
                await SqlInjection(connection);

                await AnonymousParameters(connection);

                await DynamicParameters(connection);

                await StringParameters(connection);

                await WhereInParameters(connection);

                await OutputParameters(connection);
            }
        }

        static async Task SqlInjection(IDbConnection connection)
        {
            try
            {
                Console.Write("Search products: ");
                var search = Console.ReadLine();

                var sql = @$"SELECT * FROM production.products WHERE ProductName LIKE '%{search}%'";

                var products = await connection.QueryAsync<Product>(sql);

                DisplayHelper.PrintJson(products);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }

        static async Task AnonymousParameters(IDbConnection connection)
        {
            var search = "hello; SELECT * FROM dbo.users;";

            var sql = @"
SELECT * FROM production.products 
WHERE ProductName LIKE @Search OR model_year = @Year";

            var products = await connection.QueryAsync<Product>(sql, new
            {
                Search = $"%{search}%",
                Year = 2016
            });

            DisplayHelper.PrintJson(products);
        }

        static async Task DynamicParameters(IDbConnection connection)
        {
            var dynamicParameters = new DynamicParameters(new { ProductId = 1 });

            dynamicParameters.AddDynamicParams(new { NameContains = "%tele%", CategoryId = 2 });

            dynamicParameters.Add("@NameEquals", "Television", DbType.String, ParameterDirection.Input, 10);

            var sql = @"
SELECT * FROM production.products 
WHERE ProductName LIKE @NameContains
    OR ProductName = @NameEquals
    OR category_id = @CategoryId
    OR ProductId = @ProductId;";

            var products = await connection.QueryAsync<Product>(sql, dynamicParameters);

            DisplayHelper.PrintJson(products);
        }

        static async Task StringParameters(IDbConnection connection)
        {
            string sql = @"SELECT * FROM production.products WHERE ProductName LIKE @Name";

            var dbParams = new DbString()
            {
                Value = "%Trek %",
                IsAnsi = true,
                IsFixedLength = true,
                Length = 7
            };

            var firstProduct = await connection.QueryFirstOrDefaultAsync<Product>(sql,
                new
                {
                    Name = dbParams
                });

            DisplayHelper.PrintJson(firstProduct);
        }

        static async Task WhereInParameters(IDbConnection connection)
        {
            string sql = @"SELECT * FROM production.products WHERE ProductId IN @Ids";

            var products = await connection.QueryAsync<Product>(sql,
                new
                {
                    Ids = new[] { 1, 2, 4, 5 }
                });

            DisplayHelper.PrintJson(products);
        }

        static async Task OutputParameters(IDbConnection connection)
        {
            const string ProcName = "GetProductDetails";
            string createProcSql = @$"
CREATE OR ALTER PROC {ProcName}
   @ProductId          INT,
   @Name               NVARCHAR(Max)         OUTPUT,
   @ModelYear          INT                   OUTPUT
AS
   SELECT
      @Name=ProductName,
      @ModelYear=model_year FROM production.products
   WHERE ProductId=@ProductId
";

            await connection.ExecuteAsync(createProcSql);

            var parameters = new DynamicParameters(new
            {
                ProductId = 1
            });
            parameters.Add("@Name", null, dbType: DbType.String, direction: ParameterDirection.Output, size: 256);
            parameters.Add("@ModelYear", null, dbType: DbType.Int16, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(ProcName, parameters, commandType: CommandType.StoredProcedure);

            var name = parameters.Get<string>("@Name");
            var modelYear = parameters.Get<short>("@ModelYear");

            Console.WriteLine($"{name} - {modelYear}");
        }
    }
}
