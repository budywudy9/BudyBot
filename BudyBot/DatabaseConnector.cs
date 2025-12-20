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
            connectionString = "Server=localhost;Port=3306;Database=twitch_bot;Uid=root;Pwd=root;";
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

        public async Task<bool> UserExists(string username)
        {
            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            Console.WriteLine($"Looking for: {username}");
            string query = $"SELECT id FROM user WHERE name = @name;";
            await using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@name", username);
            await using var reader = await command.ExecuteReaderAsync();
            if (reader.HasRows)
                return true;
            return false;
        }

        public async Task AddUser(string username)
        {
            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            Console.WriteLine($"New user! adding {username} to list.");
            string query = $"INSERT INTO user (name) VALUES (@name);";
            await using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@name", username);
            int rowsAffected = await command.ExecuteNonQueryAsync();
            Console.WriteLine($"\nRows inserted: {rowsAffected}");
            
        }

        
    }
}
