using Dapper;
using DapperSharing.Models;
using DapperSharing.Utils;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DapperSharing.Examples
{
    public static class E06_Relationships
    {
        public static async Task Run()
        {
            Console.WriteLine("=========== RUNNING E06_Relationships ===========");
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            using (var connection = new SqlConnection(Program.DBInfo.ConnectionString))
            {
                await QueryOneToMany(connection);

                await QueryManyToMany(connection);

                await QueryMultipleRelationships(connection);
            }
        }

        static async Task QueryOneToMany(IDbConnection connection)
        {
            var sql = @"
SELECT 
    p.product_id, 
    p.product_name,
    c.category_id,
    c.category_name
FROM production.products p
INNER JOIN production.categories c ON p.category_id = c.category_id;";

            var result = await connection.QueryAsync<ProductEntity, CategoryEntity, ProductEntity>(sql,
                (product, category) =>
                {
                    product.Category = category;
                    return product;
                }, splitOn: "category_id");

            DisplayHelper.PrintJson(result);
        }

        static async Task QueryManyToMany(IDbConnection connection)
        {
            var sql = @"
SELECT 
    s.store_id,
    s.store_name,
    p.product_id, 
    p.product_name
FROM sales.stores s
INNER JOIN production.stocks st ON s.store_id = st.store_id
INNER JOIN production.products p ON st.product_id = p.product_id;";

            var storeMap = new Dictionary<int, StoreEntity>();

            var result = await connection.QueryAsync<StoreEntity, ProductEntity, StoreEntity>(sql,
                (store, product) =>
                {
                    if (!storeMap.TryGetValue(store.StoreId, out var cachedStore))
                    {
                        cachedStore = store;
                        cachedStore.Products ??= new List<ProductEntity>();
                        storeMap[cachedStore.StoreId] = cachedStore;
                    }
                    cachedStore.Products.Add(product);
                    return cachedStore;
                }, splitOn: "product_id");

            DisplayHelper.PrintJson(storeMap.Values);
        }

        static async Task QueryMultipleRelationships(IDbConnection connection)
        {
            var sql = @"
SELECT 
    s.store_id,
    s.store_name,
    p.product_id, 
    p.product_name,
    c.category_id,
    c.category_name
FROM sales.stores s
INNER JOIN production.stocks st ON s.store_id = st.store_id
INNER JOIN production.products p ON st.product_id = p.product_id
INNER JOIN production.categories c ON p.category_id = c.category_id;";

            var storeMap = new Dictionary<int, StoreEntity>();

            var result = await connection.QueryAsync<StoreEntity, ProductEntity, CategoryEntity, StoreEntity>(sql,
                (store, product, category) =>
                {
                    if (!storeMap.TryGetValue(store.StoreId, out var cachedStore))
                    {
                        cachedStore = store;
                        cachedStore.Products ??= new List<ProductEntity>();
                        storeMap[cachedStore.StoreId] = cachedStore;
                    }
                    product.Category = category;
                    cachedStore.Products.Add(product);
                    return cachedStore;
                }, splitOn: "product_id,category_id");

            DisplayHelper.PrintJson(storeMap.Values);
        }
    }
}
