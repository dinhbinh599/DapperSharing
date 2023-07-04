using Dapper;
using DapperSharing.Utils;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Transactions;

namespace DapperSharing.Examples
{
    public static class E08_Others
    {
        public static async Task Run()
        {
            Console.WriteLine("=========== RUNNING E08_Others ===========");
            DisplayHelper.PrintListOfMethods(typeof(E08_Others));
            //Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            using (var connection = new SqlConnection(Program.DBInfo.ConnectionString))
            {
                var userInput = Console.ReadLine();
                switch (userInput)
                {
                    case "1":
                        await TransactionCommit(connection);
                        break;
                    case "2":
                        await TransactionRollback(connection);
                        break;
                    case "3":
                        await TransactionScopeRollBack(connection);
                        break;
                }
            }
        }

        static async Task TransactionCommit(IDbConnection connection)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var transaction = connection.BeginTransaction();

            try
            {
                var sql = @$"DELETE FROM production.products WHERE ProductName = @Name";

                await connection.ExecuteAsync(sql, new
                {
                    Name = "TransactionTest"
                }, transaction: transaction);

                //await connection.ExecuteAsync(sql, new
                //{
                //    Id = 1000
                //}, transaction: transaction);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                transaction.Rollback();
            }
        }

        static async Task TransactionRollback(IDbConnection connection)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using var transaction = connection.BeginTransaction();

            try
            {
                var sql = @$"DELETE FROM production.products WHERE ProductName = @Name";

                await connection.ExecuteAsync(sql, new
                {
                    Name = "TransactionTest"
                }, transaction: transaction);

                await connection.ExecuteAsync(sql, new
                {
                    Id = 1000
                }, transaction: transaction);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                transaction.Rollback();
            }
        }

        static async Task TransactionScopeRollBack(IDbConnection connection)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {

                try
                {
                    var sql = @$"DELETE FROM production.products WHERE ProductName = @Name";

                    await connection.ExecuteAsync(sql, new
                    {
                        Name = "TransactionTest"
                    });

                    await connection.ExecuteAsync(sql, new
                    {
                        Id = 1000
                    });

                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
            }
        }
    }
}
