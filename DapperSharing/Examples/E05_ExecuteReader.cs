using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DapperSharing.Examples
{
    public static class E05_ExecuteReader
    {
        public static async Task Run()
        {
            Console.WriteLine("=========== RUNNING E05_ExecuteReader ===========");
            using (var connection = new SqlConnection(Program.DBInfo.ConnectionString))
            {
                await QueryProducts(connection);
            }
        }

        static async Task QueryProducts(IDbConnection connection)
        {
            var sql = @"SELECT * FROM production.products";

            var dataReader = await connection.ExecuteReaderAsync(sql);

            var datatable = new DataTable();
            datatable.Load(dataReader);
        }
    }
}
