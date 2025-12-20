using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;

namespace BudyBot
{
    internal class DatabaseConnector
    {
        string connectionString;

        public DatabaseConnector() 
        {
            connectionString = "connection";
            Task.WaitAll(connect());
        }

        private async Task connect()
        {
            Console.WriteLine("Connecting to MariaDb...");

            try
            {
                await using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();
                Console.WriteLine("Connected to MariaDB!");

            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
