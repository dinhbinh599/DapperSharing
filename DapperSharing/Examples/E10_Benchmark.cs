using BenchmarkDotNet.Attributes;
using Dapper;
using DapperSharing.Models;
using DapperSharing.Utils;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Transactions;

namespace DapperSharing.Examples
{
    [MemoryDiagnoser]
    public class E10_Benchmark
    {
        [Benchmark]
        public void Buffered()
        {
            using (var connection = new SqlConnection(Program.DBInfo.ConnectionString))
            {     
                var sql = "Select * from production.products";

                var customers = connection.Query<Customer>(sql);

                foreach (var customer in customers)
                {
                    Console.WriteLine($"{customer.CustomerId} {customer.FirstName}");
                }
            }
        }
        [Benchmark]
        public void Unbuffered()
        {
            using (var connection = new SqlConnection(Program.DBInfo.ConnectionString))
            {
                var sql = "Select * from production.products";

                var customers = connection.Query<Customer>(sql, buffered: false);

                foreach (var customer in customers)
                {
                    Console.WriteLine($"{customer.CustomerId} {customer.FirstName}");
                }
            }
        }
        [Benchmark]
        public async Task BufferedAsync()
        {
            using (var connection = new SqlConnection(Program.DBInfo.ConnectionString))
            {
                var sql = "Select * from production.products";

                var customers = await connection.QueryAsync<Customer>(sql);

                foreach (var customer in customers)
                {
                    Console.WriteLine($"{customer.CustomerId} {customer.FirstName}");
                }
            }
        }
        [Benchmark]
        public async Task UnbufferedAsync()
        {
            using (var connection = new SqlConnection(Program.DBInfo.ConnectionString))
            {
                var sql = "Select * from production.products";

                var customers = connection.QueryUnbufferedAsync<Customer>(sql);

                await foreach (var customer in customers)
                {
                    Console.WriteLine($"{customer.CustomerId} {customer.FirstName}");
                }
            }
        }
    }
}
