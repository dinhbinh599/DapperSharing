using Dapper;
using DapperSharing.Models;
using DapperSharing.Utils;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DapperSharing.Examples
{
    public static class E09_Extensions
    {
        public static async Task Run()
        {
            Console.WriteLine("=========== RUNNING E09_Extensions ===========");
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            using (var connection = new SqlConnection(Program.DBInfo.ConnectionString))
            {
                await SqlBuilder(connection);
            }
        }

        static async Task SqlBuilder(IDbConnection connection)
        {
            var builder = new SqlBuilder()
                .Select("p.*")
                .OrderBy("p.model_year DESC, p.product_name ASC");
            var types = new List<Type>
            {
                typeof(ProductEntity)
            };
            var splitOns = new List<string>();

            Console.Write("Input search: ");
            var search = Console.ReadLine();

            Console.Write("Input category: ");
            var category = Console.ReadLine();

            Console.Write("Input model year: ");
            int.TryParse(Console.ReadLine(), out var modelYear);

            if (!string.IsNullOrWhiteSpace(search))
            {
                builder.Where("p.product_name LIKE @Search", new
                {
                    Search = $"%{search}%"
                });
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                builder.InnerJoin("production.categories as c ON p.category_id = c.category_id")
                    .Select("c.category_id, c.category_name")
                    .Where("c.category_name LIKE @CategorySearch", new
                    {
                        CategorySearch = $"%{category}%"
                    });

                types.Add(typeof(CategoryEntity));
                splitOns.Add("category_id");
            }

            if (modelYear > 0)
            {
                builder.Where("p.model_year = @Year", new
                {
                    Year = modelYear
                });
            }

            var template = builder.AddTemplate(@$"
SELECT
    /**select**/ 
FROM production.products AS p
    /**innerjoin**/
    /**where**/ 
    /**orderby**/");

            var categoryIdx = types.IndexOf(typeof(CategoryEntity));
            var products = await connection.QueryAsync(template.RawSql, types: types.ToArray(), map: (data) =>
            {
                var product = data[0] as ProductEntity;

                if (categoryIdx > -1)
                {
                    var category = data[categoryIdx] as CategoryEntity;
                    product.Category = category;
                }

                return product;
            }, param: template.Parameters, splitOn: string.Join(',', splitOns));

            DisplayHelper.PrintJson(products);
        }
    }
}
